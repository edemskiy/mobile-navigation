using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;

public class PathStartController : MonoBehaviour
{
    public LabelInfoWindow labelInfoWindow;
    public LevelsController levelsController;
    private string activeLabelFromName, activeLabelToName;
    public LabelsController labelsController;
    public NavMeshController navMeshController;
    public Button pathFromButton, pathToButton;
    private JSONObject labelInfo;

    private UnityAction<string> labelsLoadListener;

    private void Awake()
    {
        labelsLoadListener = new UnityAction<string>(OnLabelsLoad);
    }

    void Start()
    {
        TouchScreenKeyboard.hideInput = true;
        activeLabelFromName = "";
        activeLabelToName = "";
    }

    private void OnEnable()
    {
        EventManager.StartListening(AppUtils.labelsLoaded, labelsLoadListener);
    }

    private void OnDisable()
    {
        EventManager.StopListening(AppUtils.labelsLoaded, labelsLoadListener);
    }

    private void OnLabelsLoad(string s)
    {
        string qrJSONString = PlayerPrefs.GetString(AppUtils.JSON_QR, "NaN");
        if (qrJSONString != "NaN")
        {
            labelInfo = new JSONObject(qrJSONString);
            OnRouteFromClick();
        }
        else
        {
            levelsController.SetActive(AppUtils.DefaultFloorNumber);
        }
    }

    public void SetPathStart(string labelName)
    {
        // меняем цвет старого отправного маркера на белый
        labelsController.HighlightLabel(
           activeLabelFromName,
           AppUtils.DefaultLabelColor
           );

        // устанавливаем новый отправной маркер как начальную точку пути
        navMeshController.SetSource(AppUtils.stringToVector3(
                LabelsList.self.getLabel(labelName)
                .GetField(AppUtils.JSON_LOCATION).str
                ));

        // указываем имя нового отправного маркера на кнопке "откуда"
        pathFromButton.GetComponentInChildren<Text>().text = labelName;

        // меняем цвет нового отправного маркера на красный
        labelsController.HighlightLabel(labelName, AppUtils.LightRedColor);

        // сохраняем имя новго отправного маркера
        activeLabelFromName = labelName;
    }

    public void SetPathEndpoint(string labelName)
    {
        // меняем цвет старого конечного маркера на белый
        labelsController.HighlightLabel(
            activeLabelToName,
            AppUtils.DefaultLabelColor
            );

        // устанавливаем новый конечный маркер как конечную точку пути
        navMeshController.SetDestination(AppUtils.stringToVector3(
                LabelsList.self.getLabel(labelName)
                .GetField(AppUtils.JSON_LOCATION).str
                ));

        // указываем имя нового конечного маркера на кнопке "куда"
        pathToButton.GetComponentInChildren<Text>().text = labelName;

        // меняем цвет нового конечного маркера на голубой
        labelsController.HighlightLabel(labelName, AppUtils.LightBlueColor);

        // сохраняем имя новго конечного маркера
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
        SetPathStart(labelInfo.GetField(AppUtils.JSON_NUMBER).str);
        SetUpCamera();        
        labelInfoWindow.gameObject.SetActive(false);
    }

    public void OnRouteToClick()
    {
        SetPathEndpoint(labelInfo.GetField(AppUtils.JSON_NUMBER).str);
        SetUpCamera();
        labelInfoWindow.gameObject.SetActive(false);
    }

    private void SetUpCamera()
    {
        int levelNumber = 0;
        int.TryParse(labelInfo.GetField(AppUtils.JSON_FLOOR).str, out levelNumber);
        if (levelNumber != LevelsController.activeLabelNumber)
        {
            Vector3 newCamLocation = AppUtils.stringToVector3(labelInfo.GetField(AppUtils.JSON_LOCATION).str);
            levelsController.SetActive(levelNumber);

            Camera.main.transform.position = new Vector3(
                newCamLocation.x, Camera.main.transform.position.y, newCamLocation.z
                );
        }        
    }

    public void ShowInfoWindow(JSONObject label)
    {
        labelInfo = label;
        labelInfoWindow.audienceName.text = $"{labelInfo[AppUtils.JSON_NUMBER].str}" +
            $" – {labelInfo[AppUtils.JSON_NAME].str}";
        labelInfoWindow.audienceFullName.text = labelInfo[AppUtils.JSON_FULLNAME].str;
        labelInfoWindow.audienceInfo.text = labelInfo[AppUtils.JSON_INFO].str;
        
        labelInfoWindow.gameObject.SetActive(true);
    }
    
    public void ShowScaner()
    {
        PlayerPrefs.DeleteKey(AppUtils.JSON_QR);
        LevelsController.activeLabelNumber = 0;
        SceneManager.LoadScene(AppUtils.ScanerSceneName);
    }
}
