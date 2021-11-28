using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CameraButton : MonoBehaviour
{
    [SerializeField] Text cameraText;
    [SerializeField] Button cameraButton;
    [SerializeField] TMP_Text cameraNumber;
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
        cameraNumber.text = (index + 1).ToString();
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
