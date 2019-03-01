using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabelsButtonsStorage : MonoBehaviour
{
    public GameObject buttonPrefab;
    public LabelInfoWindow labelInfoWindow;

    public void AddLabelButton(string name)
    {
        GameObject newButton = GameObject.Instantiate(buttonPrefab);
        newButton.name = name;
        newButton.GetComponent<LabelButton>().SetText(name);
        newButton.transform.SetParent(this.transform);
        newButton.transform.localScale = Vector3.one;
    }
    public void OpenLabelInfoWindow(JSONObject label)
    {
        labelInfoWindow.gameObject.SetActive(true);
        labelInfoWindow.Init(label);
    }
    
    public void OnLabelButtonClick(JSONObject label)
    {
        /*
         * Вопрос: сразу устанавливать как точку отправления/назначения,
         * если до этого была нажата соответствующая кнопка
         * или всегда при нажатии открывать меню с информацией?
        */

        OpenLabelInfoWindow(label);
    }

    public void ClearSearchInput(InputField inputField)
    {
        inputField.text = "";
    }
}
