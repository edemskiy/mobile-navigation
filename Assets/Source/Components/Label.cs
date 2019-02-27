using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Label : MonoBehaviour {
    public string labelName;

	// Use this for initialization
	void Start () {
        
    }

    public void SetName(string newName)
    {
        labelName = newName;
        GetComponent<TextMeshPro>().text = newName;
        GetComponent<TextMeshPro>().raycastTarget = true;
        Debug.Log(GetComponent<TextMeshPro>().raycastTarget);
    }

    public string GetName()
    {
        return labelName;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
