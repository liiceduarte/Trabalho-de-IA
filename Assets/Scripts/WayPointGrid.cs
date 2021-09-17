using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointGrid : MonoBehaviour
{
    // Estilo de exibição do grid de navegação
    public enum EDITOR_SHOW_STYLE
    {
        NONE,
        EDGE,
        CELL_GRID
    };

    // Estilo atual de exibição do grid
    public EDITOR_SHOW_STYLE EditorStyle = EDITOR_SHOW_STYLE.EDGE;

    // Distancia que sepera um vértice do outro
    public float DistanceBetweenVertices = 0.0f;

    // Tamanho em X e Z do grid de navegação
    public int GridSize = -1;

    // Matrizes que representam o grafo
    public Vector3 [,] VertexPositions = null;
    public bool [,] VertexAvailability = null;

    // Matriz de adjacencia
    public bool [,] Edges = null;

    // Layers para atualização do grid em relação a colisão e terreno
    public LayerMask LayerMaskCollision;
    public LayerMask LayerMaskUpdateHeight;
    public float WayPointHeightOffSet = 0.0f;

    // Prefab do waypoit
    public Transform WayPointPrefab;

    // Instancia do Grid de Navegação (Singleton)
    static WayPointGrid gInstance = null;

    // Retorna a instancia do grid de navegação
    public static WayPointGrid GetInstance()
    {
        return gInstance;
    }

    // Marca objeto atual como a instancia única do grid de navegação
    void Awake()
    {
        gInstance = this;
    }

    // Cria o grid de navegação
    public void CreateWaypointGrid(int gridSize, float distanceBetweenVertices)
    {
        // Pega o objeto auxiliar
        Transform gridEditorAux = transform.GetChild(0);

        // Cria as matrizes do grid de navegação
        DistanceBetweenVertices = distanceBetweenVertices;
        GridSize = gridSize;
        VertexPositions = new Vector3[GridSize, GridSize];
        VertexAvailability = new bool[GridSize, GridSize];

        // Calculando a posição do vértice inicial do grid
        float offset = gridSize * distanceBetweenVertices * 0.5f;
        Vector3 currentPosition = transform.position - new Vector3(offset, 0.0f, offset);

        // Cria todos os vértices do grid de navegação
        for (int z = 0; z < GridSize; z++)
        {
            currentPosition.x = transform.position.x - offset;

            for (int x = 0; x < GridSize; x++)
            {
                // Iniciando o valores do vértice
                VertexPositions[x, z] = currentPosition + transform.position;
                VertexAvailability[x, z] = true;

                // Calculando o indice para vetor
                int index = ((z * GridSize) + x);

                GameObject child = null;

                // Verifica se o vértice é um vértice novo
                if (index >= gridEditorAux.childCount)
                {
                    // Se é novo então cria a vértice
                    child = Instantiate(WayPointPrefab, gridEditorAux).gameObject;
                    child.name = index.ToString();
                    child.transform.parent = gridEditorAux;
                }
                else
                {
                    // Pega o vértice já criado
                    child = gridEditorAux.GetChild(index).gameObject;
                }

                // Atualiza as informaçõe do vértice no GameObject
                child.transform.position = VertexPositions[x, z];
                child.SetActive(true);

                // Atualizando a posição em X
                currentPosition.x += distanceBetweenVertices;
            }

            // Atualiza a posição em Z
            currentPosition.z += distanceBetweenVertices;
        }
    }

    // Carrega os dados do grid de navegação
    public void LoadGrid()
    {
        Transform gridEditorAux = transform.GetChild(0);

        // Calculo de quantos vértices X por Z o grid possui
        GridSize = (int) Mathf.Sqrt((float) gridEditorAux.childCount);

        // Calculando a distância entre os vértices
        DistanceBetweenVertices = Vector3.Distance(gridEditorAux.GetChild(0).position,
                                                   gridEditorAux.GetChild(GridSize - 1).position) / (float) GridSize;

        // Cria o grid de vértices
        VertexPositions = new Vector3[GridSize, GridSize];
        VertexAvailability = new bool[GridSize, GridSize];

        // Percorre todos os GameObjects (Waypoints)
        for (int i = 0; i < gridEditorAux.childCount; i++)
        {
            // Atualizar as tabelas de vértices baseado no GameObject do Waypoint
            Transform waypoint = gridEditorAux.GetChild(i);

            // Transforma o nome no índice do vértice
            int index = int.Parse(waypoint.name);

            // Atualiza vértice
            UpdateVertex(index, waypoint.position, waypoint.gameObject.activeSelf);
        }

        // Cria a matriz de adjacencia
        Edges = new bool[GridSize * GridSize, GridSize * GridSize];
        CalculateAdjacentEdgeMatrix();
    }

    // Atualiza as informações do vértice
    public void UpdateVertex(int index, Vector3 position, bool enabled)
    {
        // Convertendo indice de vetor para matriz
        int xIndex = index % GridSize;
        int zIndex = index / GridSize;

        // Atualiza dados
        VertexPositions[xIndex, zIndex] = position;
        VertexAvailability[xIndex, zIndex] = enabled;
    }

    // Atualiza a disponibilidade de navegação do vértice
    public void UpdateVertexAvailability(int index, bool enabled)
    {
        // Convertendo indice de vetor para matriz
        int xIndex = index % GridSize;
        int zIndex = index / GridSize;

        // Atualiza dados
        VertexAvailability[xIndex, zIndex] = enabled;
    }

    // Atualiza a posição do vértice
    public void UpdateVertexPosition(int index, Vector3 position)
    {
        // Convertendo indice de vetor para matriz
        int xIndex = index % GridSize;
        int zIndex = index / GridSize;

        // Atualiza dados
        VertexPositions[xIndex, zIndex] = position;
    }

    // Calcula o peso de deslocamento entre o vértice (fromIndex) e o vértice (toIndex)
    public float GetWeight(int fromIndex, int toIndex)
    {
        // Calcula o indice dos vértice em modelo matricial
        int fromXIndex = fromIndex % GridSize;
        int fromZIndex = fromIndex / GridSize;

        int toXIndex = toIndex % GridSize;
        int toZIndex = toIndex / GridSize;

        // Pega a posição dos vértices
        Vector3 fromPos = VertexPositions[fromXIndex, fromZIndex];
        Vector3 toPos = VertexPositions[toXIndex, toZIndex];

        // Calcula distância entre cada um dos vértices
        return Vector3.Distance(fromPos, toPos);
    }

    // Retorna disponibiliza de navegação no vértice
    public bool GetVertexAvailabity(int xIndex, int zIndex)
    {
        return VertexAvailability[xIndex, zIndex];
    }

    // Retorna a posição do vértice
    public Vector3 GetVertexPosition(int xIndex, int zIndex)
    {
        return VertexPositions[xIndex, zIndex];
    }

    // Atualiza o grid removendo os vértices que estão colisão com objetos, além atualizar o y (vértice) baseado na altera do terreno
    public void UpdateWayPointGridCollision()
    {
        Transform gridEditorAux = transform.GetChild(0);

        // Percorre todos os vértices procurando por objetos no caminho
        for (int z = 0; z < GridSize; z++)
        {
            for (int x = 0; x < GridSize; x++)
            {
                Vector3 position = transform.position + VertexPositions[x, z];

                // Usa o OverlapBox para verificar se o vértice está em algum obstáculo
                float halfSize = DistanceBetweenVertices * 0.5f;
                Collider[] colliders = Physics.OverlapBox(position, new Vector3(halfSize, halfSize, halfSize),
                                                          Quaternion.identity, LayerMaskCollision, QueryTriggerInteraction.Ignore);

                // Caso colida com algum obstáculo marca com falso, senão atualizar o y (do vértice), baseado no terreno
                if (colliders.Length > 0)
                {
                    VertexAvailability[x, z] = false;
                }
                else
                {
                    VertexAvailability[x, z] = true;

                    RaycastHit hit;
                    
                    // Envia raycast para baixo
                    bool collided = Physics.Raycast(transform.position + VertexPositions[x, z] + new Vector3(0.0f, 1000.0f, 0.0f),
                                                     Vector3.down,
                                                     out hit,
                                                     2000.0f,
                                                     LayerMaskUpdateHeight);

                    if (collided)
                        VertexPositions[x, z] = hit.point + new Vector3(0.0f, WayPointHeightOffSet, 0.0f);
                    else
                        VertexPositions[x, z].y = transform.position.y;
                }

                int index = (z * GridSize) + x;
                gridEditorAux.GetChild(index).gameObject.SetActive(VertexAvailability[x, z]);
                gridEditorAux.GetChild(index).position = VertexPositions[x, z];

            }
        }
    }

    // Verifica se um vértice corrente possui adjacencia com outro vertice
    public bool HasAdjacency(int currentIndex, int otherIndex)
    {
        return Edges[currentIndex, otherIndex];
    }

    // Verifica se o vértice adjacente (baseado nos indices) é um vértice valido
    bool IsValidAdjacency(int currentIndex, int otherIndexX, int otherIndexZ)
    {
        // Verifica se os índices são válidos
        if (otherIndexX < 0 || otherIndexX >= GridSize)
            return false;

        if (otherIndexZ < 0 || otherIndexZ >= GridSize)
            return false;

        // Verifica se o vértice está disponível
        return VertexAvailability[otherIndexX, otherIndexZ];
    }

    // Retorna todas as adjacencias do vértice corrente        
    int [] GetAdjacentVertices(int indexX, int indexZ)
    {
        int currentIndex = (indexZ * GridSize) + indexX;
        List<int> adjacents = new List<int>();

        for (int x = indexX - 1; x <= indexX + 1; x++)
        {
            for (int z = indexZ - 1; z <= indexZ + 1; z++)
            {
                if (x == indexZ && z == indexZ)
                    continue;

                if (IsValidAdjacency(currentIndex, x, z))
                    adjacents.Add((z * GridSize) + x);
            }
        }

        return adjacents.ToArray();
    }

    // Calcula matriz de adjacencia.
    public void CalculateAdjacentEdgeMatrix()
    {
        // Iniciando adjacencia
        for (int i = 0; i < GridSize * GridSize; i++)
        {
            for (int j = 0; j < GridSize * GridSize; j++)
                Edges[i, j] = false;
        }

        // Marcando os vértices adjacentes
        for (int z = 0; z < GridSize; z++)
        {
            for (int x = 0; x < GridSize; x++)
            {
                int index = (z * GridSize) + x;
                int [] adjacentsVertices = GetAdjacentVertices(x, z);

                for (int i = 0; i < adjacentsVertices.Length; i++)
                    Edges[index, adjacentsVertices[i]] = true;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (VertexPositions == null)
            return;

        if (EditorStyle == EDITOR_SHOW_STYLE.CELL_GRID)
        {
            for (int x = 0; x < GridSize; x++)
            {
                for (int z = 0; z < GridSize; z++)
                {
                    if (VertexAvailability[x, z] == false)
                        continue;

                    int index = (z * GridSize) + x;

                    if (z % 2 == 0)
                        index++;

                    Color gizmoColor = Color.white;
                    if (index % 2 == 0)
                        gizmoColor = Color.black;

                    gizmoColor.a = 0.5f;
                    Gizmos.color = gizmoColor;

                    float cubeSize = DistanceBetweenVertices + 0.1f;
                    Gizmos.DrawCube(transform.position + VertexPositions[x, z], new Vector3(cubeSize, cubeSize * 0.01f, cubeSize));
                }
            }

            return;
        }

        if (EditorStyle == EDITOR_SHOW_STYLE.EDGE)
        {
            Gizmos.color = Color.white;

            for (int x = 0; x < GridSize; x++)
            {
                for (int z = 0; z < GridSize; z++)
                {
                    if (x + 1 < GridSize)
                    {
                        if (VertexAvailability[x, z] && VertexAvailability[x + 1, z])
                        {
                            // Draw horizon edges
                            Vector3 A = transform.position + VertexPositions[x, z];
                            Vector3 B = transform.position + VertexPositions[x + 1, z];

                            Gizmos.DrawLine(A, B);
                        }
                    }

                    // Draw vertical edges
                    if (z + 1 < GridSize)
                    {
                        if (VertexAvailability[x, z] && VertexAvailability[x, z + 1])
                        {
                            Vector3 A = transform.position + VertexPositions[x, z];
                            Vector3 B = transform.position + VertexPositions[x, z + 1];

                            Gizmos.DrawLine(A, B);
                        }
                    }

                    // Draw first diagonal
                    if (x + 1 < GridSize && z + 1 < GridSize)
                    {
                        if (VertexAvailability[x, z] && VertexAvailability[x + 1, z + 1])
                        {
                            Vector3 A = transform.position + VertexPositions[x, z];
                            Vector3 B = transform.position + VertexPositions[x + 1, z + 1];

                            Gizmos.DrawLine(A, B);
                        }
                    }

                    // Draw first diagonal
                    if (x + 1 < GridSize && z + 1 < GridSize)
                    {
                        if (VertexAvailability[x + 1, z] && VertexAvailability[x, z + 1])
                        {
                            Vector3 A = transform.position + VertexPositions[x + 1, z];
                            Vector3 B = transform.position + VertexPositions[x, z + 1];

                            Gizmos.DrawLine(A, B);
                        }
                    }
                }
            }
        }
    }
}