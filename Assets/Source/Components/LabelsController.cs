using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using UnityEngine.UI;
public class LabelsController : MonoBehaviour
{
    //public PointsInputsController pointsInputsController;
    public GameObject buttonPrefab;
    public GameObject markerPrefab;
    public GameObject content;
    public LevelsController levelsController;
    public LabelInfoWindow labelInfoWindow;
    private GameObject markersStore;

    private string dataPath;

    void Start()
    {
        markersStore = new GameObject("Markers");
        dataPath = Path.Combine(Application.persistentDataPath, AppUtils.labelsLocalFileName);
        LoadLabels();
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
                Label label = hit.transform.gameObject.GetComponent<Label>();
                if(label != null)
                {
                    labelInfoWindow.gameObject.SetActive(true);
                    labelInfoWindow.Init(LabelsList.self.getLabel(label.labelName));
                }
            }
        }
    }

    private void ClearLabelsButtonsList()
    {
        foreach (Transform obj in content.transform)
        {
            Button button = GetComponent<Button>();
            if (button != null)
            {
                // удалить объект
                // Destroy(button.gameObject);

                // сделать неактивным
                button.gameObject.SetActive(false);
            }
        }
    }

    public void ClearSearchInput(InputField inputField)
    {
        inputField.text = "";
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
                newLabel.transform.position = AppUtils.stringToVector3(item.GetField(AppUtils.JSON_LOCATION).str);
                newLabel.transform.SetParent(markersStore.transform);
                newLabel.GetComponent<Label>().SetName(name);

                GameObject newButton = GameObject.Instantiate(buttonPrefab);
                newButton.name = name;
                newButton.GetComponent<LabelButton>().SetText(name);
                newButton.GetComponent<LabelButton>().SetLabelsController(this);
                newButton.transform.SetParent(content.transform);
                newButton.transform.localScale = Vector3.one;
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
            marker.gameObject.SetActive((marker.gameObject.transform.position.y < levelsController.getActiveLevelPosition().y + 1f) &&
                (marker.gameObject.transform.position.y > levelsController.getActiveLevelPosition().y - 5f));
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
