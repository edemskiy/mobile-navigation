using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelsController : MonoBehaviour {

    [SerializeField]
    GameObject[] levels;
    GameObject[] levelButtons;
    public GameObject activeLevel;
    public static Vector3 activeLevelPosition;
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
        // EventManager.TriggerEvent(AppUtils.floorChanged);
    }

    public void SetActive(int levelNum)
    {
        // установка цвета кнопки
        // levelButtons[levelNum - 1].GetComponentInChildren<Text>().color = AppUtils.LightYellowColor;

        for (int i=0; i < levels.Length; i++)
        {
            levels[i].SetActive(i == levelNum-1);
        }
        activeLevel = levels[levelNum-1];
        activeLevelPosition = activeLevel.transform.position;
        EventManager.TriggerEvent(AppUtils.floorChanged);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
