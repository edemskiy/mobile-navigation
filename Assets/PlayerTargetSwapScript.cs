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
        navMeshController.SwitchRoutePoints();
        string tmp = ButtonFrom.GetComponentInChildren<Text>().text;

        ButtonFrom.GetComponentInChildren<Text>().text = ButtonTo.GetComponentInChildren<Text>().text;
        ButtonTo.GetComponentInChildren<Text>().text = tmp;
    }
}
