using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabelButton : MonoBehaviour {

    public Text text;
    private string number;

    public void SetParams(string number, string name)
    {
        text.text = $"{number} – {name}";
        this.number = number;
    }

    public void OnClick()
    {
        GetComponentInParent<LabelsButtonsStorage>()
            .OnLabelButtonClick(LabelsList.self.getLabel(number));
    }
}
