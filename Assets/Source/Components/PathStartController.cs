using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PathStartController : MonoBehaviour
{
    public LabelInfoWindow labelInfoWindow;
    public LevelsController levelsController;
    private string activeLabelFromName, activeLabelToName;
    public LabelsController labelsController;
    public NavMeshController navMeshController;
    public Button pathFromButton, pathToButton;
    private JSONObject labelInfo;

    void Start()
    {
        TouchScreenKeyboard.hideInput = true;

        activeLabelFromName = "";
        activeLabelToName = "";

        string qrJSONString = PlayerPrefs.GetString(AppUtils.JSON_QR, "NaN");
        if (qrJSONString != "NaN")
        {
            Debug.Log(qrJSONString);
            labelInfo = new JSONObject(qrJSONString);
            
            /*
             * удалить, когда в метках будет инфа об этаже!
             */
            int levelNum = 0;
            int.TryParse(labelInfo.GetField(AppUtils.JSON_FLOOR).str, out levelNum);
            levelsController.SetActive(levelNum);
            Vector3 location = AppUtils.stringToVector3(labelInfo.GetField(AppUtils.JSON_LOCATION).str);
            Camera.main.transform.position = new Vector3(location.x, Camera.main.transform.position.y, location.z);
            /* ---------------------------- */

            SetUpCamera();
            OnRouteFromClick();
        }
        else
        {
            levelsController.SetActive(4);
        }
    }

    public void SetPathStart(string labelName)
    {
        labelsController.HighlightLabel(
           activeLabelFromName,
           AppUtils.DefaultLabelColor
           );

        navMeshController.SetSource(AppUtils.stringToVector3(
                LabelsList.self.getLabel(labelName)
                .GetField(AppUtils.JSON_LOCATION).str
                ));

        pathFromButton.GetComponentInChildren<Text>().text = labelName;

        labelsController.HighlightLabel(labelName, AppUtils.LightRedColor);
        activeLabelFromName = labelName;
    }

    public void SetPathEndpoint(string labelName)
    {
        labelsController.HighlightLabel(
            activeLabelToName,
            AppUtils.DefaultLabelColor
            );
        
        navMeshController.SetDestination(AppUtils.stringToVector3(
                LabelsList.self.getLabel(labelName)
                .GetField(AppUtils.JSON_LOCATION).str
                ));

        pathToButton.GetComponentInChildren<Text>().text = labelName;

        labelsController.HighlightLabel(labelName, AppUtils.LightBlueColor);
        activeLabelToName = labelName;
    }

    public void ClearPath()
    {
        navMeshController.ResetPath();

        labelsController.HighlightLabel(
            pathFromButton.GetComponentInChildren<Text>().text,
            AppUtils.DefaultLabelColor
            );

        labelsController.HighlightLabel(
            pathToButton.GetComponentInChildren<Text>().text,
            AppUtils.DefaultLabelColor
            );

        activeLabelFromName = activeLabelToName = "";

        pathFromButton.GetComponentInChildren<Text>().text = AppUtils.ButtonFrom_DefaultName;
        pathToButton.GetComponentInChildren<Text>().text = AppUtils.ButtonTo_DefaultName;
    }       

    public void OnRouteFromClick()
    {
        SetPathStart(labelInfo.GetField(AppUtils.JSON_NAME).str);
        SetUpCamera();
        labelInfoWindow.gameObject.SetActive(false);
    }

    public void OnRouteToClick()
    {
        SetPathEndpoint(labelInfo.GetField(AppUtils.JSON_NAME).str);
        SetUpCamera();
        labelInfoWindow.gameObject.SetActive(false);
    }

    private void SetUpCamera()
    {
        // Раскомментировать, когда в метках будет инфа об этаже!
        /*
        int levelNum = 0;
        int.TryParse(labelInfo.GetField(AppUtils.JSON_FLOOR).str, out levelNum);
        levelsController.SetActive(levelNum);
        Vector3 location = AppUtils.stringToVector3(labelInfo.GetField(AppUtils.JSON_LOCATION).str);
        Camera.main.transform.position = new Vector3(location.x, Camera.main.transform.position.y, location.z);
        */
    }

    public void ShowInfoWindow(JSONObject label)
    {
        labelInfo = label;
        labelInfoWindow.audienceName.text = labelInfo.GetField(AppUtils.JSON_NAME).str;
        /* 
         * В информации о помещении пока стоит заглушка
         * так как сейчас нет информации об аудиториях
        
        string info = label.GetField(AppUtils.JSON_INFO).str;
        Debug.Log(info);
        if(info != "")
        {
            labelInfoWindow.audienceInfo.text = label.GetField(AppUtils.JSON_INFO).str;
        }        
        */
        labelInfoWindow.gameObject.SetActive(true);
    }
    
    public void ShowScaner()
    {
        SceneManager.LoadScene(AppUtils.ScanerSceneName);
    }
}
