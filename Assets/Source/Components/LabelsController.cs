using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
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

    private string dataPath, hashPath;
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
        hashPath = Path.Combine(Application.persistentDataPath, AppUtils.hashFileName);
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
        if (File.Exists(dataPath) && File.Exists(hashPath))
        {
            LoadLabelsFromFile();

            if (AppUtils.isOnline())
            {
                StartCoroutine(CheckHash());
            }
        }
        // иначе загружаем с сервера
        else
        {
            if (!AppUtils.isOnline())
            {
                Debug.Log("Необходимо подключение к интернету");
                return;
            }
            StartCoroutine(LoadLabelsFromServer());
        }
    }

    private void LoadLabelsFromFile()
    {
        Debug.Log("Reading labels from file");
        using (StreamReader streamReader = File.OpenText(dataPath))
        {
            LoadedLabelsListHandler(streamReader.ReadToEnd());
        }
    }

    IEnumerator CheckHash()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(AppUtils.hashURL))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log(": Error: " + webRequest.error);
            }
            else
            {
                JSONObject response = new JSONObject(webRequest.downloadHandler.text);
                string downloadedHash = response[AppUtils.JSON_DATA].ToString();

                using (StreamReader streamReader = File.OpenText(hashPath))
                {
                    string localHash = streamReader.ReadToEnd();
                    if(localHash != downloadedHash)                    
                    {
                        Debug.Log("hashes don't match!");
                        StartCoroutine(LoadLabelsFromServer());
                    }
                }
            }
        }
    }

    IEnumerator LoadLabelsFromServer()
    {
        Debug.Log("Download labels from server");
        using (UnityWebRequest webRequest = UnityWebRequest.Get(AppUtils.hashURL))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log(": Error: " + webRequest.error);
            }
            else
            {
                JSONObject hashResponse = new JSONObject(webRequest.downloadHandler.text);
                string hashData = hashResponse[AppUtils.JSON_DATA].ToString();
                using (StreamWriter streamWriter = File.CreateText(hashPath))
                {
                    streamWriter.Write(hashData);
                }
            }
        }

        using (UnityWebRequest webRequest = UnityWebRequest.Get(AppUtils.labelsURL))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log(": Error: " + webRequest.error);
            }
            else
            {
                JSONObject labelsResponse = new JSONObject(webRequest.downloadHandler.text);
                string labelsData = AppUtils.DecodeUrlString(labelsResponse[AppUtils.JSON_DATA].ToString());
                using (StreamWriter streamWriter = File.CreateText(dataPath))
                {
                    streamWriter.Write(labelsData);
                }
                LoadedLabelsListHandler(labelsData);
            }
        }
    }

    // постобработка загруженных меток
    private void LoadedLabelsListHandler(string labelsList)
    {
        JSONObject labelsListJSON = new JSONObject(labelsList);
        if (labelsListJSON.list == null)
        {
            Debug.Log("labelsListJSON.list == null");
            return;
        }

        // перебираем все метки
        for (int i = 0; i < labelsListJSON.list.Count; i++)
        {
            JSONObject department = labelsListJSON.list[i];

            if (department != null)
            {
                JSONObject roomsListJSON = department[AppUtils.JSON_ROOMS];
                for (int j = 0; j < roomsListJSON.list.Count; j++)
                {
                    JSONObject room = roomsListJSON.list[j];
                    foreach (string key in department.keys)
                    {
                        if (key != AppUtils.JSON_ID && key != AppUtils.JSON_ROOMS)
                        {
                            room.AddField(key, department[key]);
                        }
                    }

                    string roomNumber = room[AppUtils.JSON_NUMBER].str;

                    LabelsList.self.update(roomNumber, room);
                    GameObject newLabel = GameObject.Instantiate(markerPrefab);
                    newLabel.transform.position = AppUtils.stringToVector3(
                        room[AppUtils.JSON_LOCATION].str) + (Vector3.up * 0.5f);
                    newLabel.transform.SetParent(markersStore.transform);
                    newLabel.GetComponent<Label>().SetName(roomNumber);
                    
                    // добавляем в хранилище меток
                    labelsStorage.Add(roomNumber, newLabel);

                    // создаем кнопку в меню поиска
                    labelsButtonsStorage.AddLabelButton(roomNumber, room[AppUtils.JSON_NAME].str);
                }
            }
            else
            {
                Debug.Log("MainScreen: labelsListJSON.list department == null ");
            }
        }
    }
    
    // фильтрация меток по имени или информации
    public void SearchLabels(string s)
    {
        foreach (Transform obj in content.transform)
        {
            JSONObject label = LabelsList.self.getLabel(obj.name);
            string fullInfo = (obj.name
                + label[AppUtils.JSON_NAME].str
                + label[AppUtils.JSON_FULLNAME].str
                + label[AppUtils.JSON_INFO].str)
                .ToLower();

            obj.gameObject.SetActive(fullInfo.Contains(s));
        }
    }

    public GameObject GetLabel(string labelName)
    {
        GameObject labelObj = null;
        labelsStorage.TryGetValue(labelName, out labelObj);
        return labelObj;
    }

    // выделение метки (изменение цвета)
    public void HighlightLabel(string labelName, Color color)
    {
        GameObject labelObj = GetLabel(labelName);
        if(labelObj != null)
        {
            labelObj.GetComponent<TextMeshPro>().color = color;
        }
    }

    public void ShowOnlyActiveMarkers()
    {
        foreach (Transform marker in markersStore.transform)
        {
            marker.gameObject.SetActive(
                LabelsList.self.getLabel(marker.GetComponent<Label>().GetName())
                .GetField(AppUtils.JSON_FLOOR).str == LevelsController.activeLabelNumber.ToString()
                );
        }
    }   
}
