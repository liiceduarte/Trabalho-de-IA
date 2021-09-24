using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour, IPathFinder
{
    WayPointGrid mapGrid;
    List<Node> visiteds;
    List<Node> prepared;

    private void Start()
    {
        mapGrid = FindObjectOfType<WayPointGrid>();
        Node.mapGrid = mapGrid;

        if (mapGrid == null)
        {
            Debug.LogError("ERRO! Não foi encontrado um WayPointGrid na cena");
        }
    }


    public List<Vector3> GetPathFromTo(int originIndex, int destinationIndex)
    {
        List<Vector3> path = new List<Vector3>();
        visiteds = new List<Node>();
        prepared = new List<Node>();
        bool found = false;
        int gridPow = mapGrid.GridSize * mapGrid.GridSize;

        if (!mapGrid) return path; // se não há um mapGrid, cancela a operação

        // insere o nó atual na lista de preparados
        Node node = new Node();
        node.index = originIndex;
        node.parentNode = -1;
        prepared.Add(node);

        // visiteds < gridPow é flag para quebrar em caso de erro de código
        while (!found && prepared.Count > 0 && visiteds.Count < gridPow)
        {
            prepared.Sort(); // ordena os nós de acordo com o peso

            // busca o nó de menor peso
            node = prepared[0];

            // remove dos preparados e insere nos visitados
            prepared.Remove(node);
            visiteds.Add(node);

            // se é o nó destino, encerra a busca
            if (node.index == destinationIndex)
            {
                found = true;
                Debug.Log("PATH FOUND");
            }
            else
            {



                // busca os nós vizinhos


                for (int i = 0; i < gridPow; i++)
                {

                    // se existe uma aresta entre os nós
                    if (mapGrid.Edges[node.index, i])
                    {

                        // verifica se ja foi visitado, se foi, continua
                        if (!IsVisited(i))
                        {

                            Node noPreparado = IsPrepared(i);

                            if (noPreparado is null)
                            {
                                noPreparado = new Node();
                                noPreparado.travelLength = node.travelLength + mapGrid.GetWeight(node.index, i);
                                noPreparado.weight = noPreparado.travelLength + Manhatam(i, destinationIndex);
                                noPreparado.index = i;
                                noPreparado.parentNode = node.index;
                                prepared.Add(noPreparado);


                            }
                            else
                            {
                                // se o nó ja foi preparado, verifica se o peso atual é menor do que o ja existente
                                if (node.weight + mapGrid.GetWeight(node.index, i) + Manhatam(i, destinationIndex) < noPreparado.weight)
                                {

                                    // caso seja, atualiza o peso e seta este nó como pai e atualiza os nós preparados
                                    prepared.Remove(noPreparado);
                                    noPreparado.travelLength = node.travelLength + mapGrid.GetWeight(node.index, i);
                                    noPreparado.weight = noPreparado.travelLength + Manhatam(i, destinationIndex);
                                    noPreparado.parentNode = node.index;
                                    prepared.Add(noPreparado);
                                }
                            }
                        }
                    }
                }
            }
        }

        // se encontoru um caminho, monta retorno
        if (found)
        {
            path = GetPath(visiteds[visiteds.Count - 1]);
        }


        return path;
    }

    /// <summary>
    /// Verifica se o index descoberto ja foi olhado
    /// </summary>
    /// <param name="index"></param>
    /// <param name="prepared"></param>
    /// <returns></returns>
    private bool IsVisited(int index)
    {
        foreach (Node n in visiteds)
        {
            if (n.index == index)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Verifica se o index ja foi preparado
    /// </summary>
    /// <param name="index"></param>
    /// <param name="visited"></param>
    /// <returns>Node caso encontre, null caso não encontre</returns>
    private Node IsPrepared(int index)
    {
        foreach (Node n in prepared)
        {
            if (n.index == index)
            {
                return n;
            }
        }
        return null;
    }

    /// <summary>
    /// Retorna o nó pai do nó visitado
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private Node GetVisitedNode(int index)
    {
        foreach (Node n in visiteds)
        {
            if (n.index == index)
            {
                return n;
            }
        }

        return null;
    }

    /// <summary>
    /// retorna o caminho até o nó destino de forma recursiva
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    private List<Vector3> GetPath(Node n)
    {
        Node parentNode = GetVisitedNode(n.parentNode);
        List<Vector3> path;
        if (parentNode is null)
        {
            path = new List<Vector3>();
        }
        else
        {
            path = GetPath(parentNode);
        }

        path.Add(n.GetPosition());

        return path;
    }

    private float Manhatam(int origin, int destination)
    {
        Vector3 originPos = mapGrid.GetVertexPosition(origin % mapGrid.GridSize, origin / mapGrid.GridSize);
        Vector3 destinationPos = mapGrid.GetVertexPosition(destination % mapGrid.GridSize, destination / mapGrid.GridSize);

        float distance = Mathf.Abs(destinationPos.x - originPos.x) + Mathf.Abs(destinationPos.z - originPos.z);

        return distance;
    }

    // define o mapgrid
    public void UpdateMapGridInfo(WayPointGrid mapGrid)
    {
        this.mapGrid = mapGrid;
    }
}
