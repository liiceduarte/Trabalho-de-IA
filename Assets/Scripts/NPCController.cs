using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    private IPathFinder pathFinder;
    enum State{IDDLE, MOVING, CLEANING};
    private WayPointGrid mapGrid;
    private State state;
    private List<Vector3> path;
    private int currentTargetIndex;

    // Start is called before the first frame update
    void Start()
    {
        // busca o componente de encontrar caminho
        pathFinder = GetComponent<IPathFinder>();

        // busca grid de mapas
        mapGrid = FindObjectOfType<WayPointGrid>();
        state = State.IDDLE;
    }

    // Update is called once per frame
    void Update()
    {
        switch(state){
            case State.IDDLE:
                // busca novo caminho aleatório
                GetRandomDestination();
                break;
        }
    }

    // busca posição aleatória no grid
    private void GetRandomDestination(){
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
}
