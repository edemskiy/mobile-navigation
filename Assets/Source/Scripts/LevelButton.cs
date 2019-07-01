using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour {

    public GameObject level;
    public static float activeLevelPositionY;

    public void SetActive()
    {     
        GetComponentInParent<BuildingPopup>().SetActiveLevel(level);
        GetComponentInChildren<Text>().color = AppUtils.LightYellowColor;
        activeLevelPositionY = level.transform.position.y;
        EventManager.TriggerEvent(AppUtils.floorChanged);
    }

}
