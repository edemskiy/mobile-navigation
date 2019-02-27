using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabelInfoWindow : MonoBehaviour
{
    public Text text;
    public NavMeshController navMeshController;
    private JSONObject labelInfo;
    // Start is called before the first frame update
    void Start()
    {
        text.text = "";
    }

    public void Init(JSONObject label)
    {
        labelInfo = label;
        text.text = label.ToString();
    }

    public void OnRouteFromClick()
    {
        Debug.Log(labelInfo.GetField(AppUtils.JSON_LOCATION).str);
        Debug.Log(labelInfo.GetField(AppUtils.JSON_NAME).str);
        //navMeshController.SetSource(AppUtils.stringToVector3(labelInfo.GetField(AppUtils.JSON_LOCATION).str));
    }

    public void OnRouteToClick()
    {
        Debug.Log(labelInfo.GetField(AppUtils.JSON_LOCATION).str);
        Debug.Log(labelInfo.GetField(AppUtils.JSON_NAME).str);
        //navMeshController.SetDestination(AppUtils.stringToVector3(labelInfo.GetField(AppUtils.JSON_LOCATION).str));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
