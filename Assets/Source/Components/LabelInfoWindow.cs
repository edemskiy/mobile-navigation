using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabelInfoWindow : MonoBehaviour
{
    public Text text;
    // Start is called before the first frame update
    void Start()
    {
        text.text = "";
    }

    public void Init(JSONObject label)
    {
        text.text = label.str;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
