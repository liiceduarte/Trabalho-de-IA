using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadthFirst : MonoBehaviour, IPathFinder
{
    WayPointGrid mapGrid;
    List<Node> visiteds;
    Queue<Node> notVisiteds;

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
        notVisiteds = new Queue<Node>();
        bool found = false;
        int gridPow = mapGrid.GridSize * mapGrid.GridSize;

        if (!mapGrid) return path; // se não há um mapGrid, cancela a operação

        // insere o nó atual na fila de não visitado
        Node node = new Node();
        node.index = originIndex;
        node.parentNode = -1;
        notVisiteds.Enqueue(node);
        
        // visiteds < gridPow é flag para quebrar em caso de erro de código
        while (!found && notVisiteds.Count > 0 && visiteds.Count < gridPow)
        {
            // remove dos preparados e insere nos visitados
            node = notVisiteds.Dequeue();
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

                        if (!IsVisited(i))
                        {

                            Node noPreparado = IsPrepared(i);

                            if (noPreparado is null)
                            {
                                noPreparado = new Node();
                                noPreparado.index = i;
                                noPreparado.parentNode = node.index;
                                notVisiteds.Enqueue(noPreparado);
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
        foreach (Node n in notVisiteds)
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

    // define o mapgrid
    public void UpdateMapGridInfo(WayPointGrid mapGrid)
    {
        this.mapGrid = mapGrid;
    }
}
