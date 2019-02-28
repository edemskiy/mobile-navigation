﻿using System.Collections;
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
        }        
    }

    public void Clear()
    {
        navMeshController.ResetPath();
        GameObject labelFrom = labelsController
            .GetLabelObjectByName(ButtonFrom.GetComponentInChildren<Text>().text);
        GameObject labelTo = labelsController
            .GetLabelObjectByName(ButtonTo.GetComponentInChildren<Text>().text);

        if(labelFrom != null)
        {
            labelFrom.GetComponent<TextMeshPro>().color = Color.white;
            
        }
        if (labelTo != null)
        {
            labelTo.GetComponent<TextMeshPro>().color = Color.white;
        }

        ButtonFrom.GetComponentInChildren<Text>().text = AppUtils.ButtonFrom_DefaultName;
        ButtonTo.GetComponentInChildren<Text>().text = AppUtils.ButtonTo_DefaultName;
        
    }
}
