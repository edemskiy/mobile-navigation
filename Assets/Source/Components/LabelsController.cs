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
    private float prevActiveLevelPositionY;
    private GameObject markersStore;

    private string dataPath;

    void Start()
    {
        dataPath = Path.Combine(Application.persistentDataPath, AppUtils.labelsLocalFileName);
        LoadLabels();
    }

    void Update()
    {

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
                /*
                GameObject newLabel = GameObject.Instantiate(markerPrefab);
                newLabel.transform.position = AppUtils.stringToVector3(item.GetField(AppUtils.JSON_LOCATION).str);
                newLabel.transform.parent = markersStore.transform;
                newLabel.GetComponent<Label>().SetName(name);
                */

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
