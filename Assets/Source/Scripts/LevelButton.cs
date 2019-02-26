using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class LevelButton : MonoBehaviour {

    public GameObject level;
    public static float activeLevelPositionY;
	// Use this for initialization
	void Start () {
		
	}
    public void SetActive()
    {
        /*
        foreach(Transform lvlButton in GetComponentInParent<BuildingPopup>().transform)
        {
            LevelSwithcer swithcer = lvlButton.GetComponent<LevelSwithcer>();
            if (swithcer != null)
            {
                swithcer.GetComponentInChildren<Text>().color = Color.white;
                swithcer.level.SetActive(false);
            }
        }
        GetComponentInChildren<Text>().color = new Color(0.204f, 0.663f, 0.537f, 1.0f);
        level.SetActive(true);
        activeLevelPositionY = level.transform.position.y;
        */
        
        GetComponentInParent<BuildingPopup>().SetActiveLevel(level);
        GetComponentInChildren<Text>().color = new Color(1f, 0.92f, 0.16f, 1.0f);
        activeLevelPositionY = level.transform.position.y;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
