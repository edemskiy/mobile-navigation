using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LabelInfoWindow : MonoBehaviour
{
    public Text text;
    public NavMeshController navMeshController;
    public Button buttonFrom, buttonTo;
    public LabelsController labelsController;
    private JSONObject labelInfo;

    public void Init(JSONObject label)
    {
        labelInfo = label;
        text.text = label.ToString();
    }

    public void OnRouteFromClick()
    {
        navMeshController.SetSource(
            AppUtils.stringToVector3(labelInfo.GetField(AppUtils.JSON_LOCATION).str)
            );
        buttonFrom.GetComponentInChildren<Text>().text = labelInfo.GetField(AppUtils.JSON_NAME).str;
        GameObject activeLabelObj = labelsController.GetLabelObjectByName(labelInfo.GetField(AppUtils.JSON_NAME).str);
        if(activeLabelObj != null)
        {
            activeLabelObj.GetComponent<TextMeshPro>().color = AppUtils.LightRedColor;
        }
        this.gameObject.SetActive(false);
    }

    public void OnRouteToClick()
    {
        navMeshController.SetDestination(
            AppUtils.stringToVector3(labelInfo.GetField(AppUtils.JSON_LOCATION).str)
            );
        buttonTo.GetComponentInChildren<Text>().text = labelInfo.GetField(AppUtils.JSON_NAME).str;
        GameObject activeLabelObj = labelsController.GetLabelObjectByName(labelInfo.GetField(AppUtils.JSON_NAME).str);
        if (activeLabelObj != null)
        {
            activeLabelObj.GetComponent<TextMeshPro>().color = AppUtils.LightBlueColor;
        }
        this.gameObject.SetActive(false);
    }
}
