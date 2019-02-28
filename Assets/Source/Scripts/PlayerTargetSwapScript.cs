using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTargetSwapScript : MonoBehaviour
{
    public NavMeshController navMeshController;
    public Button ButtonFrom;
    public Button ButtonTo;

    public void Swap()
    {
        if(navMeshController.IsPathBuilt())
        {
            navMeshController.SwitchRoutePoints();
            string tmp = ButtonFrom.GetComponentInChildren<Text>().text;

            ButtonFrom.GetComponentInChildren<Text>().text = ButtonTo.GetComponentInChildren<Text>().text;
            ButtonTo.GetComponentInChildren<Text>().text = tmp;
        }        
    }

    public void Clear()
    {
        navMeshController.ResetPath();
        ButtonFrom.GetComponentInChildren<Text>().text = AppUtils.ButtonFrom_DefaultName;
        ButtonTo.GetComponentInChildren<Text>().text = AppUtils.ButtonTo_DefaultName;
    }
}
