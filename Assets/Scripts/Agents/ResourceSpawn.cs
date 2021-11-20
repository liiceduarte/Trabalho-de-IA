using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawn : MonoBehaviour
{
    [SerializeField] private Resource.ResourceType resourceType;

    public Resource SpawnResource(){
        return new Resource(resourceType);
    }

    public Resource.ResourceType GetResourceType(){
        return resourceType;
    }
}
