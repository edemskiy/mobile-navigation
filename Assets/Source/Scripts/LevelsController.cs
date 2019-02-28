using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsController : MonoBehaviour {

    [SerializeField]
    GameObject[] levels;
    public GameObject activeLevel;
    public static Vector3 activeLevelPosition;
    // Use this for initialization
    void Start () {
        SetActive(levels[0]);
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
	
	// Update is called once per frame
	void Update () {
		
	}
}
