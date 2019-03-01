using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LabelInfoWindow : MonoBehaviour
{
    public Text audienceName;
    public Text audienceInfo;
    public NavMeshController navMeshController;
    public Button buttonFrom, buttonTo;
    public LabelsController labelsController;
    private JSONObject labelInfo;

    private string activeLabelFromName, activeLabelToName;

    private void Start()
    {
        activeLabelFromName = "";
        activeLabelToName = "";
    }

    public void Init(JSONObject label)
    {
        labelInfo = label;
        audienceName.text = labelInfo.GetField(AppUtils.JSON_NAME).str;

        /* 
         * В информации о помещении пока стоит заглушка
         * так как сейчас нет информации об аудиториях
        */
        // audienceInfo.text = label.GetField(AppUtils.JSON_INFO).str;
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

        buttonFrom.GetComponentInChildren<Text>().text = activeLabelFromName;

        labelsController.HighlightLabel(
            activeLabelFromName,
            AppUtils.LightRedColor
            );

        this.gameObject.SetActive(false);
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

        buttonTo.GetComponentInChildren<Text>().text = activeLabelToName;

        labelsController.HighlightLabel(
            activeLabelToName,
             AppUtils.LightBlueColor
             );

        this.gameObject.SetActive(false);
    }
}
