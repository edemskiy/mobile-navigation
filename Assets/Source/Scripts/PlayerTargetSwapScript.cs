using UnityEngine;
using UnityEngine.UI;

public class PlayerTargetSwapScript : MonoBehaviour
{
    public NavMeshController navMeshController;
    public LabelsController labelsController;
    public PathStartController pathStartController;
    public Button ButtonFrom;
    public Button ButtonTo;

    public void Swap()
    {
        string buttonFromName = ButtonFrom.GetComponentInChildren<Text>().text;
        string buttonToName = ButtonTo.GetComponentInChildren<Text>().text;

        pathStartController.ClearPath();

        if (buttonFromName != AppUtils.ButtonFrom_DefaultName)
        {
            pathStartController.SetPathEndpoint(buttonFromName);

            /*
            navMeshController.SetDestination(AppUtils.stringToVector3(
                LabelsList.self.getLabel(buttonFromName)
                .GetField(AppUtils.JSON_LOCATION).str
                ));
            labelsController.HighlightLabel(buttonFromName, AppUtils.LightBlueColor);
            ButtonTo.GetComponentInChildren<Text>().text = buttonFromName;
            */
        }
        else
        {
            ButtonTo.GetComponentInChildren<Text>().text = AppUtils.ButtonTo_DefaultName;
            navMeshController.SetDestination(Vector3.zero);
        }
        
        
        if(buttonToName != AppUtils.ButtonTo_DefaultName)
        {
            pathStartController.SetPathStart(buttonToName);            
            /*
            navMeshController.SetSource(AppUtils.stringToVector3(
                LabelsList.self.getLabel(buttonToName)
                .GetField(AppUtils.JSON_LOCATION).str
                ));
            labelsController.HighlightLabel(buttonToName, AppUtils.LightRedColor);
            ButtonFrom.GetComponentInChildren<Text>().text = buttonToName;
            */
        }
        else
        {
            ButtonFrom.GetComponentInChildren<Text>().text = AppUtils.ButtonFrom_DefaultName;
            navMeshController.SetSource(Vector3.zero);
        }
    }
}
