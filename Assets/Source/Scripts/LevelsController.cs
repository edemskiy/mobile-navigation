﻿using UnityEngine;
using UnityEngine.UI;

public class LevelsController : MonoBehaviour {

    [SerializeField]
    GameObject[] levels;

    [SerializeField]
    GameObject[] levelButtons;

    public GameObject activeLevel;
    public static Vector3 activeLevelPosition;
    public static int activeLabelNumber;
    // Use this for initialization
    void Start () {

	}


    public void SetActive(GameObject newActiveLevel)
    {
        foreach (GameObject level in levels)
        {
            level.SetActive(level == newActiveLevel);
        }
        activeLevel = newActiveLevel;
        activeLevelPosition = activeLevel.transform.position;
        EventManager.TriggerEvent(AppUtils.floorChanged);
    }

    public void SetActive(int levelNum)
    {
        for (int i=0; i < levels.Length; i++)
        {
            levelButtons[i].GetComponentInChildren<Text>().color = AppUtils.DefaultLabelColor;
            levels[i].SetActive(i == levelNum-1);
        }
        levelButtons[levelNum - 1].GetComponentInChildren<Text>().color = AppUtils.LightYellowColor;
        activeLevel = levels[levelNum-1];
        activeLevelPosition = activeLevel.transform.position;
        activeLabelNumber = levelNum;
        EventManager.TriggerEvent(AppUtils.floorChanged);
    }

}
