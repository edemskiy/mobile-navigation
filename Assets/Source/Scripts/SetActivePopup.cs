using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActivePopup : MonoBehaviour
{
    public void Switch()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
    
    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
