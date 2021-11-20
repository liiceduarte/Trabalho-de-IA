using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource
{
    public enum ResourceType{
        NONE,A,B,C,D,E
    };

    private ResourceType type;

    public Resource(){
        type = ResourceType.NONE;
    }

    public Resource(ResourceType type){
        this.type = type;
    }

    public ResourceType GetResourceType(){
        return type;
    }
}
