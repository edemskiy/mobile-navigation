using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsController : MonoBehaviour {

    [SerializeField]
    GameObject[] levels;
    public GameObject activeLevel;
    public LabelsController labelsController;
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
        labelsController.ShowOnlyActiveMarkers();
    }
    public Vector3 getActiveLevelPosition()
    {
        return activeLevel.transform.position;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
