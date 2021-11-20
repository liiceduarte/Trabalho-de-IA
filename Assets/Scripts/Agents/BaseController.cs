using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    [SerializeField] List<ResourceSpawn> resourceSpawns;
    private int[] resourceAmount;

    private void Awake() {
        resourceAmount = new int[6];
        for(int i = 0; i < 6; i++){
            resourceAmount[i] = 0;
        }
    }

    public ResourceSpawn GetResourceSpawnOfType(Resource.ResourceType type){
        List<ResourceSpawn> spawns = new List<ResourceSpawn>();
        foreach(ResourceSpawn s in resourceSpawns){
            if(s.GetResourceType() == type){
                spawns.Add(s);
            }
        }

        if(spawns.Count > 0){
            return spawns[Random.Range(0, spawns.Count)];
        }else{
            return null;
        }
    }

    public Resource.ResourceType GetResourceInNeed(){
        // TODO implementar lógica para definir qual recurso precisa mais
        return Resource.ResourceType.A;
    }

    public void ReceiveResource(Resource resource){
        resourceAmount[(int)resource.GetResourceType()]++;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
