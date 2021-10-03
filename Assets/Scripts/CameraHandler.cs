using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] private List<CinemachineVirtualCamera> cameraSpots;
    private int currentCameraSpot;

    private void Awake()
    {
        currentCameraSpot = 0;
    }

    private void Start()
    {
        SwapCamera(0);
    }

    /// <summary>
    /// Troca o spot atual da c�mera para a posi��o sugerida
    /// </summary>
    /// <param name="newCameraSpot"></param>
    public void SwapCamera(int newCameraSpot)
    {
        cameraSpots[currentCameraSpot].Priority = 0;
        cameraSpots[newCameraSpot].Priority = 5;
        currentCameraSpot = newCameraSpot;
    }

    /// <summary>
    /// Movimenta a c�mera para a pr�xima posi��o
    /// </summary>
    public void NextCamera()
    {
        int nextCamera = currentCameraSpot + 1;
        if(nextCamera >= cameraSpots.Count)
        {
            nextCamera = 0;
        }

        SwapCamera(nextCamera);
    }

    /// <summary>
    /// Movimenta a c�mera para a posi��o anterior
    /// </summary>
    public void PrevCamera()
    {
        int nextCamera = currentCameraSpot - 1;
        if (nextCamera < 0)
        {
            nextCamera = cameraSpots.Count - 1;
        }

        SwapCamera(nextCamera);
    }

    /// <summary>
    /// Acesso a quantidade de c�meras
    /// </summary>
    public int CameraSpotsCount
    {
        get { return cameraSpots.Count; }
    }

    /// <summary>
    /// Retorna o index da c�mera atual
    /// </summary>
    public int CurrentCamera
    {
        get { return currentCameraSpot; }
    }
}
