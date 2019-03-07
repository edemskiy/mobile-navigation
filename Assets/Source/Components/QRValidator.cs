using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QRValidator : MonoBehaviour
{
    public Text nameText;
    public Text aboutText;
    public GameObject serviceWindow;
    public GameObject qrLabelInfo;
    private UnityAction<string> qrDetectedListener;
    private string nameString, aboutString;
    private string labelJSON;
    private bool qrFound;

    void Start()
    {
        nameString = "";
        aboutString = "";
        qrFound = false;
    }

    private void Awake()
    {
        qrDetectedListener = new UnityAction<string>(OnQrDetected);
    }

    private void OnEnable()
    {
        EventManager.StartListening(AppUtils.qrDetected, qrDetectedListener);
    }

    private void OnDisable()
    {
        EventManager.StopListening(AppUtils.qrDetected, qrDetectedListener);
    }

    private void OnQrDetected(string s)
    {
        // проверка валидности маркера
        JSONObject info = new JSONObject(s);
        if (info.HasField(AppUtils.JSON_NAME) && 
            info.HasField(AppUtils.JSON_LOCATION) &&
            info.HasField(AppUtils.JSON_FLOOR))
        {
            labelJSON = s;
            nameString = info.GetField(AppUtils.JSON_NAME).str;
            aboutString = info.GetField(AppUtils.JSON_INFO).str;
            qrFound = true;
        }
        else
        {
            serviceWindow.SetActive(true);
        }
    }

    public void ShowMapButtonOnClick()
    {
        if (qrFound)
        {
            PlayerPrefs.SetString(AppUtils.JSON_QR, labelJSON);
        }
        else
        {
            PlayerPrefs.DeleteAll();
        }

        SceneManager.LoadScene(AppUtils.NavigationSceneName);
    }

    public void CloseButtonOnClick()
    {
        qrFound = false;
        qrLabelInfo.gameObject.SetActive(false);
    }
    void Update()
    {
        nameText.text = nameString;
        aboutText.text = aboutString;
        qrLabelInfo.gameObject.SetActive(qrFound);
    }
}
