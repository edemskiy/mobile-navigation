using UnityEngine;
using UnityEngine.UI;

public class PlayerTargetSwapScript : MonoBehaviour
{
    public NavMeshController navMeshController;
    public LabelsController labelsController;
    public Button ButtonFrom;
    public Button ButtonTo;

    public void Swap()
    {
        string buttonFromName = ButtonFrom.GetComponentInChildren<Text>().text;
        string buttonToName = ButtonTo.GetComponentInChildren<Text>().text;

        if (buttonFromName != AppUtils.ButtonFrom_DefaultName)
        {
            navMeshController.SetDestination(AppUtils.stringToVector3(
                LabelsList.self.getLabel(buttonFromName)
                .GetField(AppUtils.JSON_LOCATION).str
                ));
            labelsController.HighlightLabel(buttonFromName, AppUtils.LightBlueColor);
            ButtonTo.GetComponentInChildren<Text>().text = buttonFromName;
        }
        else
        {
            ButtonTo.GetComponentInChildren<Text>().text = AppUtils.ButtonTo_DefaultName;
        }


        if(buttonToName != AppUtils.ButtonTo_DefaultName)
        {
            navMeshController.SetSource(AppUtils.stringToVector3(
                LabelsList.self.getLabel(buttonToName)
                .GetField(AppUtils.JSON_LOCATION).str
                ));
            labelsController.HighlightLabel(buttonToName, AppUtils.LightRedColor);
            ButtonFrom.GetComponentInChildren<Text>().text = buttonToName;
        }
        else
        {
            ButtonFrom.GetComponentInChildren<Text>().text = AppUtils.ButtonFrom_DefaultName;
        }
    }

    public void Clear()
    {
        navMeshController.ResetPath();

        labelsController.HighlightLabel(
            ButtonFrom.GetComponentInChildren<Text>().text,
            AppUtils.DefaultLabelColor
            );

        labelsController.HighlightLabel(
            ButtonTo.GetComponentInChildren<Text>().text,
            AppUtils.DefaultLabelColor
            );

        ButtonFrom.GetComponentInChildren<Text>().text = AppUtils.ButtonFrom_DefaultName;
        ButtonTo.GetComponentInChildren<Text>().text = AppUtils.ButtonTo_DefaultName;        
    }
}
