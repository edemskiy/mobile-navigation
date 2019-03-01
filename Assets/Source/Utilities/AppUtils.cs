using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine.EventSystems;

public class AppUtils
{
    // public const string labelsURL = "http://127.0.0.1:8000/api/codes/";
    public const string labelsURL = "http://88.201.132.192:8000/api/codes/";
    public const string labelsLocalFileName = "labels.txt";

    // Names
    public const string ButtonFrom_DefaultName = "ОТКУДА";
    public const string ButtonTo_DefaultName = "КУДА";

    // Colors
    public static Color LightYellowColor = new Color(1f, 0.92f, 0.16f, 1.0f);
    public static Color LightRedColor = new Color(1f, 0.41f, 0.36f, 1.0f);
    public static Color LightBlueColor = new Color(0.28f, 0.65f, 1f, 1.0f);
    public static Color DefaultLabelColor = Color.white;

    // Events
    public const string floorChanged = "floorChanged";
    public const string qrDetected = "qrDetected";

    // JSON fields
    public const string JSON_ID = "id";
    public const string JSON_NAME = "name";
    public const string JSON_KEY = "key";
    public const string JSON_URL = "url";
    public const string JSON_DATA = "data";
    public const string JSON_LOCATION = "location";
    public const string JSON_FLOOR = "floor";
    public const string JSON_INFO = "info";
    public const string JSON_QR = "qrcode";
    public const string JSON_TYPE = "type";

    public static bool isOnline()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }

    public static string fixInputString(string inputString)
    {
        return Regex.Replace(inputString, "[;\\/:*?\"\"<>{}|&']", "");
    }

    public static Vector3 stringToVector3(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // Split the items
        string[] array = sVector.Split(',');

        float x = 0.0f;
        float y = 0.0f;
        float z = 0.0f;

        Vector3 result = new Vector3();

        if (array.Count() > 2)
        {
            if (float.TryParse(array[0], out x) && float.TryParse(array[1], out y) && float.TryParse(array[2], out z))
            {
                result = new Vector3(x, y, z);
            }
        }

        return result;
    }

    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
