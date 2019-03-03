using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QRValidator : MonoBehaviour
{
    public Text infoText;
    public Button showMapButton;
    private UnityAction<string> qrDetectedListener;
    private string infoString;
    private string labelJSON;
    private bool buttonActive;

    void Start()
    {
        infoString = "";
        buttonActive = false;
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
            infoString = info.GetField(AppUtils.JSON_NAME).str;
            buttonActive = true;
        }
        else
        {
            infoString = "Неверный формат маркера";
        }
    }

    public void ShowMapButtonOnClick()
    {
        PlayerPrefs.SetString(AppUtils.JSON_QR, labelJSON);
        SceneManager.LoadScene(AppUtils.NavigationSceneName);
    }

    void Update()
    {
        infoText.text = infoString;
        showMapButton.gameObject.SetActive(buttonActive);
    }
}
