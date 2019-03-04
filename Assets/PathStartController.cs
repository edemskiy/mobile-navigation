using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathStartController : MonoBehaviour
{
    public LabelInfoWindow labelInfoWindow;
    private string activeLabelFromName, activeLabelToName;
    public LabelsController labelsController;
    public NavMeshController navMeshController;
    private JSONObject labelInfo;
    // Start is called before the first frame update
    void Start()
    {
        activeLabelFromName = "";
        activeLabelToName = "";

        string qrJSONString = PlayerPrefs.GetString(AppUtils.JSON_QR, "NaN");
        if (qrJSONString != "NaN")
        {
            Debug.Log(qrJSONString);
            labelInfo = new JSONObject(qrJSONString);
            OnRouteFromClick();
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

        labelInfoWindow.gameObject.SetActive(false);
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
