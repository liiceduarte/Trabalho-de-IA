using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControler : MonoBehaviour
{
    [SerializeField] RectTransform cameraButtonsRectTransform;
    [SerializeField] GameObject cameraButtonPrefab;
    [SerializeField] CameraHandler cameraHandler;
    private List<CameraButton> cameraButtonsList;
    // Start is called before the first frame update
    void Start()
    {
        cameraButtonsList = new List<CameraButton>();
        InitButtons();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateButtons()
    {
        for (int i = 0; i < cameraHandler.CameraSpotsCount; i++)
        {
            cameraButtonsList[i].UpdateInfo("Camera " + (i + 1) , cameraHandler.CurrentCamera != i);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(cameraButtonsRectTransform);
    }

    private void InitButtons()
    {
        for(int i = 0; i < cameraHandler.CameraSpotsCount; i++)
        {
            GameObject buttonObject = Instantiate(cameraButtonPrefab);
            buttonObject.GetComponent<RectTransform>().parent = cameraButtonsRectTransform;

            CameraButton button = buttonObject.GetComponent<CameraButton>();
            button.SetCameraId(i);
            cameraButtonsList.Add(button);
        }

        UpdateButtons();
    }

    public void NextCamera()
    {
        cameraHandler.NextCamera();
        UpdateButtons();
    }

    public void PrevCamera()
    {
        cameraHandler.PrevCamera();
        UpdateButtons();
    }
}
