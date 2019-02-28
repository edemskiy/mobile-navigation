using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using UnityEngine.Events;
public class LabelsController : MonoBehaviour
{
    //public PointsInputsController pointsInputsController;
    public GameObject buttonPrefab;
    public GameObject markerPrefab;
    public GameObject content;
    public LabelsButtonsStorage labelsButtonsStorage;
    private GameObject markersStore;

    private UnityAction floorChangeListener;

    private string dataPath;
    private Dictionary<string, GameObject> labelsStorage;

    // для хранения точки касания экрана
    private Vector3 touchPoint;

    void Start()
    {
        markersStore = new GameObject("Markers");
        labelsStorage = new Dictionary<string, GameObject>();
        dataPath = Path.Combine(Application.persistentDataPath, AppUtils.labelsLocalFileName);
        LoadLabels();
        ShowOnlyActiveMarkers();
    }

    private void Awake()
    {
        floorChangeListener = new UnityAction(OnFloorChange);
    }

    private void OnEnable()
    {
        EventManager.StartListening(AppUtils.floorChanged, floorChangeListener);
    }

    private void OnDisable()
    {
        EventManager.StopListening(AppUtils.floorChanged, floorChangeListener);
    }

    private void OnFloorChange()
    {
        ShowOnlyActiveMarkers();
    }

    void Update()
    {
        if (AppUtils.IsPointerOverUIObject())
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                touchPoint = hit.point;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Label label = hit.transform.gameObject.GetComponent<Label>();
                if (label != null && touchPoint == hit.point)
                {
                    labelsButtonsStorage.OpenLabelInfoWindow(LabelsList.self.getLabel(label.labelName));
                }
            }
        }
    }

    public void LoadLabels()
    {
        if (File.Exists(dataPath))
        {
            using (StreamReader streamReader = File.OpenText(dataPath))
            {
                LoadedLabelsListHandler(streamReader.ReadToEnd());                
            }
        }
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

    public GameObject GetLabelObjectByName(string name)
    {
        GameObject labelObj = null;
        labelsStorage.TryGetValue(name, out labelObj);
        return labelObj;
    }
    private void LoadedLabelsListHandler(string labelsList)
    {
        JSONObject labelsListJSON = new JSONObject(labelsList);
        if(labelsListJSON.list == null)
        {
            Debug.Log("labelsListJSON.list == null");
            return;
        }

        for (int i = 0; i < labelsListJSON.list.Count; i++)
        {
            JSONObject item = labelsListJSON.list[i];

            if (item != null)
            {
                string name = Regex.Unescape(item.GetField(AppUtils.JSON_NAME).str);
                LabelsList.self.update(name, item);
                
                GameObject newLabel = GameObject.Instantiate(markerPrefab);
                newLabel.transform.position = AppUtils.stringToVector3(
                    item.GetField(AppUtils.JSON_LOCATION).str) + (Vector3.up * 0.5f);
                newLabel.transform.SetParent(markersStore.transform);
                labelsStorage.Add(name, newLabel);
                newLabel.GetComponent<Label>().SetName(name);

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
