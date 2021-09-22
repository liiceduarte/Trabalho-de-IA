using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class WayPointAux : MonoBehaviour
{
    // Carrega os waypoint ao iniciar
    void Awake()
    {
        Debug.Log("Load Grid");

        LoadWayPoints();

        // Se está executando o jogo então desabilita os objetos de waypoint
        if (!Application.isEditor || EditorApplication.isPlaying)
            gameObject.SetActive(false);
    }

    public void LoadWayPoints()
    {
        if (transform.childCount == 0)
            return;

        transform.parent.GetComponent<WayPointGrid>().LoadGrid();
    }
}
