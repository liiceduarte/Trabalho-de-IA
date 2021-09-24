using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    [SerializeField] private IPathFinder pathFinder;
    enum State{IDDLE, MOVING, CLEANING};

    // Variáveis de configuração
    [Range(0.5f, 10)][SerializeField] private float cleaningDelay = 2;
    [SerializeField] private GameObject dustParticles;
    [SerializeField] float distanceOffset = 0.1f;
    [SerializeField] float moveSpeed = 2;

    //--------------------------------------------------------------

    private float cleaningTimer = 0;

    private WayPointGrid mapGrid;
    private State state;
    private List<Vector3> path;
    private int currentTargetIndex;
    private int currentWaypointIndex;
    private GameObject dust;

    // Start is called before the first frame update
    void Awake()
    {
        // busca o componente de encontrar caminho
        pathFinder = GetComponent<IPathFinder>();
        currentTargetIndex = 0;

        // busca grid de mapas
        mapGrid = FindObjectOfType<WayPointGrid>();
        state = State.IDDLE;
    }

    // Update is called once per frame
    void Update()
    {
        switch(state){
            case State.IDDLE:
                break;

            case State.MOVING:
                FollowPath();
                break;

            case State.CLEANING:
                ClearFloor();
                break;
        }
    }

    /// <summary>
    /// Define o mapGrid a ser utilizado
    /// </summary>
    /// <param name="mapGrid"></param>
    public void SetMapGrid(WayPointGrid mapGrid)
    {
        this.mapGrid = mapGrid;
        Node.mapGrid = this.mapGrid;
        pathFinder.UpdateMapGridInfo(mapGrid);

        currentWaypointIndex = ((int)transform.position.z * mapGrid.GridSize) + (int)transform.position.x;
    }

    /// <summary>
    /// Define o index de waypoint inicial do robo
    /// </summary>
    /// <param name="index"></param>
    public void SetStartingWaypointIndex(int index)
    {
        currentWaypointIndex = index;
    }

    /// <summary>
    /// Define um destino para este NPC
    /// </summary>
    /// <param name="targetIndex"></param>
    public void SetDestination(int targetIndex, GameObject dust)
    {
        currentTargetIndex = 0;
        if(pathFinder is null)
        {
            pathFinder = GetComponent<IPathFinder>();
        }

        path = pathFinder.GetPathFromTo(currentWaypointIndex, targetIndex);
        this.dust = dust;

        Debug.Log("From : " + currentWaypointIndex + " , To: " + targetIndex);
        state = State.MOVING;
    }

    /// <summary>
    /// Retorna a poeira que o npc está perseguindo
    /// </summary>
    /// <returns></returns>
    public GameObject GetDust()
    {
        return dust;
    }

    public int GetCurrentWaypointIndex()
    {
        return currentWaypointIndex;
    }

    // busca posição aleatória no grid
    public void GetRandomDestination(){
        int currentIndex = ((int)transform.position.z * mapGrid.GridSize) + (int)transform.position.x;
        int destinationIndex = Random.Range(0, mapGrid.GridSize * mapGrid.GridSize); ;

        Vector2 indexes = Vector2.zero;
        indexes.x = destinationIndex % mapGrid.GridSize;
        indexes.y = destinationIndex / mapGrid.GridSize;
        
        int tries = 100;
        while(!mapGrid.GetVertexAvailabity((int)indexes.x, (int)indexes.y) && tries > 0){
            destinationIndex = Random.Range(0, mapGrid.GridSize * mapGrid.GridSize);
            indexes.x = destinationIndex % mapGrid.GridSize;
            indexes.y = destinationIndex / mapGrid.GridSize;
            tries--;
        }

        path = pathFinder.GetPathFromTo(currentIndex, destinationIndex);
        currentIndex = 0;

        Debug.Log("From : " + currentIndex + " , To: " + destinationIndex + " , steps :" + path.Count);
        state = State.MOVING;
    }


    /// <summary>
    /// Percorre o caminho encontrado até o final
    /// </summary>
    private void FollowPath()
    {
        if(path.Count <= 0)
        {
            SceneController.instance.RobotFinishedClearing(this);
            return;
        }
        // se está próximo o suficiente, passa a seguir para o próximo ponto
        if(Vector3.Distance(transform.position, path[currentTargetIndex]) < distanceOffset)
        {
            currentTargetIndex++;

            if(currentTargetIndex >= path.Count)
            {
                cleaningTimer = 0;
                state = State.CLEANING;
            }
        }
        else
        {
            transform.position += (path[currentTargetIndex] - transform.position).normalized * moveSpeed * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation((path[currentTargetIndex] - transform.position).normalized, transform.up);
        }

    }

    private void ClearFloor()
    {
        cleaningTimer += Time.deltaTime;
        if(cleaningTimer >= cleaningDelay)
        {
            state = State.IDDLE;
            SceneController.instance.RobotFinishedClearing(this);
        }
    }
}
