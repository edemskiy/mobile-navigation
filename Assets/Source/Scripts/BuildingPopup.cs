using UnityEngine;
using UnityEngine.UI;

public class BuildingPopup : MonoBehaviour {
    public LevelsController levelsController;
    
    public void SetActiveLevel(GameObject level)
    {
        foreach (Transform obj in transform)
        {
            LevelButton lvlButton = obj.GetComponent<LevelButton>();
            if (lvlButton != null)
            {
                lvlButton.GetComponentInChildren<Text>().color = Color.white;
            }
        }

        levelsController.SetActive(level);
    }
    public void Open()
    {
        gameObject.SetActive(true);
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
}
