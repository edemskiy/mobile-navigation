using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabelsButtonsStorage : MonoBehaviour
{
    public GameObject buttonPrefab;
    public PathStartController pathStartController;

    public void AddLabelButton(string number, string name)
    {
        GameObject newButton = Instantiate(buttonPrefab);
        newButton.name = number;
        newButton.GetComponent<LabelButton>().SetParams(number, name);
        newButton.transform.SetParent(this.transform);
        newButton.transform.localScale = Vector3.one;
    }
    public void OpenLabelInfoWindow(JSONObject label)
    {
        pathStartController.ShowInfoWindow(label);
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
