using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabelInfoWindow : MonoBehaviour
{
    public Text text;
    public NavMeshController navMeshController;
    public Button buttonFrom, buttonTo;
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
        this.gameObject.SetActive(false);
    }

    public void OnRouteToClick()
    {
        navMeshController.SetDestination(
            AppUtils.stringToVector3(labelInfo.GetField(AppUtils.JSON_LOCATION).str)
            );
        buttonTo.GetComponentInChildren<Text>().text = labelInfo.GetField(AppUtils.JSON_NAME).str;
        this.gameObject.SetActive(false);
    }
}
