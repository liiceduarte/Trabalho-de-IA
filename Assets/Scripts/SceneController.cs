using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    [SerializeField] List<GameObject> npcPrefabs;
    [SerializeField] List<GameObject> dustPrefabs;
    [Range(1, 200)][SerializeField] int maxNPCAmount = 10;
    [SerializeField] Transform gridAux;
    [SerializeField] WayPointGrid mapGrid;
    [SerializeField] Transform robotContainer;
    public static SceneController instance;

    private List<NPCController> npcs;
    private List<GameObject> dusts;

    // Start is called before the first frame update
    void Start()
    {
        npcs = new List<NPCController>();
        dusts = new List<GameObject>();
        instance = this;

        mapGrid.CreateWaypointGrid(mapGrid.GridSize, mapGrid.DistanceBetweenVertices);
        mapGrid.UpdateWayPointGridCollision();

        // spawna a quantidade desejada de robos
        for(int i = 0; i < maxNPCAmount; i++)
        {
            SpawnRobot(npcPrefabs[Random.Range(0, npcPrefabs.Count)]);
        }

        // inicializa cada robo para uma posição diferente do mapa
        foreach(NPCController robot in npcs)
        {
            UpdateRobotDust(robot);
        }
    }


    // Limpa a poeira, atualiza a posição destino e instancia uma nova poeira
    private void UpdateRobotDust(NPCController npc)
    {
        GameObject npcDust = npc.GetDust();


        Transform waypoint = GetValidWaypoint();
        int waypointIndex = Mathf.Abs(int.Parse(waypoint.gameObject.name));

        GameObject dust = Instantiate(dustPrefabs[Random.Range(0, dustPrefabs.Count)], waypoint.position, Quaternion.identity);
        dusts.Add(dust);

        npc.SetDestination(waypointIndex, dust);

        if (npcDust)
        {
            dusts.Remove(npcDust);
            Destroy(npcDust.gameObject);
        }

    }

    /// <summary>
    /// Spawna um robo em uma posição aleatória
    /// </summary>
    /// <param name="prefab"></param>
    private void SpawnRobot(GameObject prefab)
    {
        Transform randomWaypoint = GetValidWaypoint();

        NPCController npc = Instantiate(prefab, randomWaypoint.position, Quaternion.identity).GetComponent<NPCController>();
        npc.SetMapGrid(mapGrid);
        npc.SetStartingWaypointIndex(int.Parse(randomWaypoint.gameObject.name));

        npcs.Add(npc);

        npc.transform.parent = robotContainer;
    }
    
    /// <summary>
    /// Busca um waypoint aleatório do grid
    /// </summary>
    /// <returns></returns>
    private Transform GetRandomWaypoint()
    {
        
        return gridAux.GetChild(Random.Range(0, gridAux.childCount));
    }

    /// <summary>
    /// Retorna um waypoint garantido de não estar dentro da parede
    /// </summary>
    /// <returns></returns>
    private Transform GetValidWaypoint()
    {
        Transform waypoint = GetRandomWaypoint();
        int waypointIndex = Mathf.Abs(int.Parse(waypoint.gameObject.name));
        while (!mapGrid.GetVertexAvailabity(waypointIndex % mapGrid.GridSize, waypointIndex / mapGrid.GridSize))
        {
            waypoint = GetRandomWaypoint();
            waypointIndex = Mathf.Abs(int.Parse(waypoint.gameObject.name));
        }

        return waypoint;
    }

    public void RobotFinishedClearing(NPCController robot)
    {
        UpdateRobotDust(robot);
    }
}
