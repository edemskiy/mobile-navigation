using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathStartController : MonoBehaviour
{
    public LabelInfoWindow labelInfoWindow;
    public LevelsController levelsController;
    private string activeLabelFromName, activeLabelToName;
    public LabelsController labelsController;
    public NavMeshController navMeshController;
    private JSONObject labelInfo;

    void Start()
    {
        TouchScreenKeyboard.hideInput = true;

        activeLabelFromName = "";
        activeLabelToName = "";

        string qrJSONString = PlayerPrefs.GetString(AppUtils.JSON_QR, "NaN");
        if (qrJSONString != "NaN")
        {
            Debug.Log(qrJSONString);
            labelInfo = new JSONObject(qrJSONString);
            
            /*
             * удалить, когда в метках будет инфа об этаже!
             */
            int levelNum = 0;
            int.TryParse(labelInfo.GetField(AppUtils.JSON_FLOOR).str, out levelNum);
            levelsController.SetActive(levelNum);
            Vector3 location = AppUtils.stringToVector3(labelInfo.GetField(AppUtils.JSON_LOCATION).str);
            Camera.main.transform.position = new Vector3(location.x, Camera.main.transform.position.y, location.z);
            /* ---------------------------- */

            SetUpCamera();
            OnRouteFromClick();
        }
        else
        {
            levelsController.SetActive(4);
        }
    }

    public void OnRouteFromClick()
    {
        labelsController.HighlightLabel(
            activeLabelFromName,
            AppUtils.DefaultLabelColor
            );

        activeLabelFromName = labelInfo.GetField(AppUtils.JSON_NAME).str;

        navMeshController.SetSource(
            AppUtils.stringToVector3(labelInfo.GetField(AppUtils.JSON_LOCATION).str)
            );

        labelInfoWindow.buttonFrom.GetComponentInChildren<Text>().text = activeLabelFromName;

        labelsController.HighlightLabel(
            activeLabelFromName,
            AppUtils.LightRedColor
            );

        SetUpCamera();

        labelInfoWindow.gameObject.SetActive(false);
    }

    public void OnRouteToClick()
    {
        labelsController.HighlightLabel(
            activeLabelToName,
            AppUtils.DefaultLabelColor
            );

        activeLabelToName = labelInfo.GetField(AppUtils.JSON_NAME).str;

        navMeshController.SetDestination(
            AppUtils.stringToVector3(labelInfo.GetField(AppUtils.JSON_LOCATION).str)
            );

        labelInfoWindow.buttonTo.GetComponentInChildren<Text>().text = activeLabelToName;

        labelsController.HighlightLabel(
            activeLabelToName,
             AppUtils.LightBlueColor
             );
        SetUpCamera();
        labelInfoWindow.gameObject.SetActive(false);
    }

    private void SetUpCamera()
    {
        // Раскомментировать, когда в метках будет инфа об этаже!
        /*
        int levelNum = 0;
        int.TryParse(labelInfo.GetField(AppUtils.JSON_FLOOR).str, out levelNum);
        levelsController.SetActive(levelNum);
        Vector3 location = AppUtils.stringToVector3(labelInfo.GetField(AppUtils.JSON_LOCATION).str);
        Camera.main.transform.position = new Vector3(location.x, Camera.main.transform.position.y, location.z);
        */

    }
    public void ShowInfoWindow(JSONObject label)
    {
        labelInfo = label;
        labelInfoWindow.audienceName.text = labelInfo.GetField(AppUtils.JSON_NAME).str;
        /* 
         * В информации о помещении пока стоит заглушка
         * так как сейчас нет информации об аудиториях
        */
        // audienceInfo.text = label.GetField(AppUtils.JSON_INFO).str;

        labelInfoWindow.gameObject.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
