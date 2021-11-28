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
        return SelectResourceTarget();
    }

    public void ReceiveResource(Resource resource){
        resourceAmount[(int)resource.GetResourceType()]++;
    }

    private Resource.ResourceType SelectResourceTarget(){
        Resource.ResourceType selected = Resource.ResourceType.A;
        float sum = 0;
        float[] prob = new float[resourceAmount.Length];
        float probSum = 0;
        for(int i = 1; i < resourceAmount.Length; i++){
            sum += resourceAmount[i] + 1;
        }
        for(int i = 1; i < resourceAmount.Length; i++){
            prob[i] = 1 - ((resourceAmount[i]+1)/sum);
            probSum += prob[i];
        }
        float rand = Random.Range(0, probSum);
        float currentBase = 0;
        for(int i = 1; i < prob.Length; i++){
            if(rand > currentBase && rand <= currentBase + prob[i]){
                selected = (Resource.ResourceType)i;
            }
            currentBase += prob[i];
        }

        return selected;
    }
}
