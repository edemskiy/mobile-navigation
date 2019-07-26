using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QRValidator : MonoBehaviour
{
    public GameObject serviceWindow; // окно для вывода ошибок
    public LabelInfoWindow labelInfoWindow; // окно с информацией о помещении

    private UnityAction<string> qrDetectedListener; 
    private string nameString, fullNameString, aboutString;
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

    // обработчик qr-метки
    private void OnQrDetected(string s)
    {
        // проверка валидности маркера
        JSONObject info = new JSONObject(s);
        if (info.HasField(AppUtils.JSON_NAME) && 
            info.HasField(AppUtils.JSON_LOCATION) &&
            info.HasField(AppUtils.JSON_FLOOR))
        {
            labelJSON = s;

            // прямое присваивание в текстовые поля интерфейса не работает;
            // скорее всего из-за асинхронности обработки qr-метки
            nameString = info.GetField(AppUtils.JSON_NAME).str;
            fullNameString = info.GetField(AppUtils.JSON_FULLNAME).str;
            aboutString = info.GetField(AppUtils.JSON_INFO).str;
            qrFound = true;
        }
        else
        {
            // если метка не валидна то выводим сообщение с ошибкой
            serviceWindow.SetActive(true);
        }
    }

    // обработчик для кнопок перехода на сцену с навигацией
    public void ShowMapButtonOnClick()
    {
        /* если маркер найден, то записываем инфу в PlayerPrefs,
         * что бы потом ее можно было считать в другой сцене.
         * Иначе - очищаем все поля в PlayerPrefs
         */
    
        if (qrFound)
        {
            PlayerPrefs.SetString(AppUtils.JSON_QR, labelJSON);
        }
        else
        {
            PlayerPrefs.DeleteAll();
        }
        
        // переходим на сцену навигации
        SceneManager.LoadScene(AppUtils.NavigationSceneName);
    }

    public void CloseButtonOnClick()
    {
        qrFound = false;
        labelInfoWindow.gameObject.SetActive(false);
    }

    void Update()
    {
        labelInfoWindow.audienceName.text = nameString;
        labelInfoWindow.audienceFullName.text = fullNameString;
        labelInfoWindow.audienceInfo.text = aboutString;
        
        labelInfoWindow.gameObject.SetActive(qrFound);
    }
}
