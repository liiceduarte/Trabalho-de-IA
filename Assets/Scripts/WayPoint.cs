using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WayPoint : MonoBehaviour
{
    Vector3 LastPosition = Vector3.zero;

    private void Awake()
    {
        LastPosition = transform.position;
    }

    public void OnEnable()
    {
        int index = 0;
        if (int.TryParse(name, out index) == false)
            return;

        Debug.Log("Vertice Habilitado");

        transform.parent.parent.GetComponent<WayPointGrid>().UpdateVertexAvailability(index, true);
    }

    public void OnDisable()
    {
        int index = 0;
        if (int.TryParse(name, out index) == false)
            return;

        Debug.Log("Vertice desabilitado");

        transform.parent.parent.GetComponent<WayPointGrid>().UpdateVertexAvailability(index, false);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.hasChanged == false || LastPosition == transform.position)
            return;

        int index = 0;
        if (int.TryParse(name, out index) == false)
            return;

        LastPosition = transform.position;

        Debug.Log("Posição do vertice modificado");

        transform.parent.parent.GetComponent<WayPointGrid>().UpdateVertexPosition(index, transform.position);
    }
}
