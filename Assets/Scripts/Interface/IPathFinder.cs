using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPathFinder
{
    /// <summary>
    /// Retorna para o usuário o caminho encontrado para determinado waypoint
    /// </summary>
    /// <param name="destination"></param>
    /// <returns></returns>
    Vector3[] GetPathTo(WayPoint destination);
}
