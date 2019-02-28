using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class LevelButton : MonoBehaviour {

    public GameObject level;
    public static float activeLevelPositionY;

    public void SetActive()
    {     
        GetComponentInParent<BuildingPopup>().SetActiveLevel(level);
        GetComponentInChildren<Text>().color = new Color(1f, 0.92f, 0.16f, 1.0f);
        activeLevelPositionY = level.transform.position.y;
        EventManager.TriggerEvent(AppUtils.floorChanged);
    }

}
