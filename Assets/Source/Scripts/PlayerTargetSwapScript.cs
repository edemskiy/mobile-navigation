using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerTargetSwapScript : MonoBehaviour
{
    public NavMeshController navMeshController;
    public LabelsController labelsController;
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

            labelsController.HighlightLabel(ButtonFrom.GetComponentInChildren<Text>().text, AppUtils.LightRedColor);
            labelsController.HighlightLabel(ButtonTo.GetComponentInChildren<Text>().text, AppUtils.LightBlueColor);
        }        
    }

    public void Clear()
    {
        navMeshController.ResetPath();

        labelsController.HighlightLabel(
            ButtonFrom.GetComponentInChildren<Text>().text,
            AppUtils.DefaultLabelColor
            );

        labelsController.HighlightLabel(
            ButtonTo.GetComponentInChildren<Text>().text,
            AppUtils.DefaultLabelColor
            );

        ButtonFrom.GetComponentInChildren<Text>().text = AppUtils.ButtonFrom_DefaultName;
        ButtonTo.GetComponentInChildren<Text>().text = AppUtils.ButtonTo_DefaultName;        
    }
}
