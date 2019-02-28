using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabelButton : MonoBehaviour {
    public Text text;
    LabelsController labelsController;

    public void SetText(string newText)
    {
        text.text = newText;
    }

    public void SetLabelsController(LabelsController newLabelsController)
    {
        labelsController = newLabelsController;
    }

    public void OnClick()
    {
        GetComponentInParent<LabelsButtonsStorage>()
            .OnLabelButtonClick(LabelsList.self.getLabel(text.text));
    }
}
