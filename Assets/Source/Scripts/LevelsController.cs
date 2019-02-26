using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsController : MonoBehaviour {

    [SerializeField]
    GameObject[] levels;
    private GameObject activeLevel;
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
    }
    public Vector3 getActiveLevelPosition()
    {
        return activeLevel.transform.position;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
