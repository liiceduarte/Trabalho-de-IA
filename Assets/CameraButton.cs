using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraButton : MonoBehaviour
{
    [SerializeField] Text cameraText;
    [SerializeField] Button cameraButton;
    private int cameraIndex;
    private UIControler uiControler;
    private CameraHandler cameraHandler;

    private void Start()
    {
        uiControler = FindObjectOfType<UIControler>();
        cameraHandler = FindObjectOfType<CameraHandler>();
    }

    public void SetCameraId(int index)
    {
        this.cameraIndex = index;
    }
    public void UpdateInfo(string text, bool available)
    {
        cameraText.text = text;
        cameraButton.interactable = available;
    }

    public void SwapCameraOnClick()
    {
        cameraHandler.SwapCamera(cameraIndex);
        uiControler.UpdateButtons();
    }
}
