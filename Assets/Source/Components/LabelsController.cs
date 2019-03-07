using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class LabelsController : MonoBehaviour
{
    public GameObject buttonPrefab;
    public GameObject markerPrefab;
    public GameObject content;
    public LabelsButtonsStorage labelsButtonsStorage;
    private GameObject markersStore;

    private UnityAction<string> floorChangeListener;

    private string dataPath;
    private Dictionary<string, GameObject> labelsStorage;

    // для хранения точки касания экрана
    private Vector3 touchPoint;

    void Start()
    {
        ShowOnlyActiveMarkers();
    }

    private void Awake()
    {
        markersStore = new GameObject("Markers");
        labelsStorage = new Dictionary<string, GameObject>();
        dataPath = Path.Combine(Application.persistentDataPath, AppUtils.labelsLocalFileName);
        LoadLabels();
        floorChangeListener = new UnityAction<string>(OnFloorChange);
    }

    private void OnEnable()
    {
        EventManager.StartListening(AppUtils.floorChanged, floorChangeListener);
    }

    private void OnDisable()
    {
        EventManager.StopListening(AppUtils.floorChanged, floorChangeListener);
    }

    private void OnFloorChange(string s)
    {
        ShowOnlyActiveMarkers();
    }

    void Update()
    {
        // если нажатие происходит поверх интерфейса: выход
        if (AppUtils.IsPointerOverUIObject())
        {
            return;
        }

        // нажатие левой кнопкой мыши (пальца)
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                // точка на карте в которой произошло касание
                touchPoint = hit.point;
            }
        }

        // поднятие левой кнопки мыши (пальца)
        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Label label = hit.transform.gameObject.GetComponent<Label>();
                // если попали в объект метки, и отпустили палец в той же точке, где и нажали,
                // то показываем инфо о метке
                if (label != null && touchPoint == hit.point)
                {
                    labelsButtonsStorage.OpenLabelInfoWindow(LabelsList.self.getLabel(label.labelName));
                }
            }
        }
    }

    public void LoadLabels()
    {
        // если на устройстве есть файл с сохраненными метками то считываем из него
        if (File.Exists(dataPath))
        {
            using (StreamReader streamReader = File.OpenText(dataPath))
            {
                LoadedLabelsListHandler(streamReader.ReadToEnd());                
            }
        }
        // иначе загружаем с сервера
        else
        {
            if (!AppUtils.isOnline())
            {
                Debug.Log("MainScreen: Нет подключения к интернету!");
                return;
            }
            StartCoroutine(LoadLabelsFromServer(AppUtils.labelsURL));
        }
    }

    // фильтрация меток по имени или информации
    public void SearchLabels(string s)
    {
        foreach (Transform obj in content.transform)
        {
            if(s.Length < 2)
            {
                obj.gameObject.SetActive(obj.name.StartsWith(s));
            }
            else
            {
                obj.gameObject.SetActive(obj.name.Contains(s));
            }            
        }
    }

    // выделение метки (изменение цвета)
    public void HighlightLabel(string labelName, Color color)
    {
        GameObject labelObj = null;
        labelsStorage.TryGetValue(labelName, out labelObj);
        if(labelObj != null)
        {
            labelObj.GetComponent<TextMeshPro>().color = color;
        }
    }

    // постобработка загруженных меток
    private void LoadedLabelsListHandler(string labelsList)
    {
        JSONObject labelsListJSON = new JSONObject(labelsList);
        if(labelsListJSON.list == null)
        {
            Debug.Log("labelsListJSON.list == null");
            return;
        }

        // перебираем все метки
        for (int i = 0; i < labelsListJSON.list.Count; i++)
        {
            JSONObject item = labelsListJSON.list[i];

            if (item != null)
            {
                string name = Regex.Unescape(item.GetField(AppUtils.JSON_NAME).str);
                LabelsList.self.update(name, item);

                // создаем метку на карте
                GameObject newLabel = GameObject.Instantiate(markerPrefab);
                newLabel.transform.position = AppUtils.stringToVector3(
                    item.GetField(AppUtils.JSON_LOCATION).str) + (Vector3.up * 0.5f);
                newLabel.transform.SetParent(markersStore.transform);
                newLabel.GetComponent<Label>().SetName(name);
                
                // добавляем в хранилище меток
                labelsStorage.Add(name, newLabel);

                // создаем кнопку в меню поиска
                labelsButtonsStorage.AddLabelButton(name);
            }
            else
            {
                Debug.Log("MainScreen: labelsListJSON.list item == null ");
            }
        }
    }

    public void ShowOnlyActiveMarkers()
    {
        foreach (Transform marker in markersStore.transform)
        {
            marker.gameObject.SetActive((marker.gameObject.transform.position.y < LevelsController.activeLevelPosition.y + 1f) &&
                (marker.gameObject.transform.position.y > LevelsController.activeLevelPosition.y - 5f));
            // float markerPos = MyUtils.stringToVector3(LabelsList.self.getLabel(marker.GetComponent<Label>().GetName()).GetField(MyUtils.JSON_LOCATION).str).y;
            // content.transform.Find(marker.GetComponent<Label>().GetName()).gameObject.SetActive(marker.gameObject.activeSelf);
        }
    }

    IEnumerator LoadLabelsFromServer(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log(": Error: " + webRequest.error);
            }
            else
            {                
                using (StreamWriter streamWriter = File.CreateText(dataPath))
                {
                    streamWriter.Write(webRequest.downloadHandler.text);
                }
                LoadedLabelsListHandler(webRequest.downloadHandler.text);
            }
        }
    }
       
}
