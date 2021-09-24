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
    List<Vector3> GetPathFromTo(int originIndex, int destinationIndex);
    void UpdateMapGridInfo(WayPointGrid mapGrid);
}
