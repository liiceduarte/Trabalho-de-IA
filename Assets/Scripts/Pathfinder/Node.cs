using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Node : MonoBehaviour, IComparable<Node>
{
    public float weight; // peso para comparações
    public float travelLength; // distância percorrida até o momento
    public int parentNode; // posição do nó anterior
    public int index;
    public static WayPointGrid mapGrid;

    public Node(){
        weight = 0;
        travelLength = 0;
        parentNode = -1;
        index = -1;
    }

    public int CompareTo(Node other)
    {
        if(weight < other.weight) return -1;
        else if(weight > other.weight) return 1;
        else return 0;
    }

    public Vector2 GetIndexes(){
        Vector2 indexes = Vector2.zero;
        indexes.x = index % mapGrid.GridSize;
        indexes.y = index / mapGrid.GridSize;
        return indexes;
    }

    public Vector3 GetPosition(){
        Vector2 indexes = GetIndexes();
        return mapGrid.GetVertexPosition((int)indexes.x, (int)indexes.y);
    }

    public static int GetIndex(Vector3 pos){
        return ((int)pos.z * mapGrid.GridSize) + (int)pos.x;
    }
}
