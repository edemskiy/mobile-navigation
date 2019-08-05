using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine.EventSystems;

public class AppUtils
{
    // public const string labelsURL = "http://127.0.0.1:8000/api/codes/";
    public const string labelsURL = "http://lk.spiiras.nw.ru/command.php?a=externalrequest&request_type=departments";
    public const string hashURL = "http://lk.spiiras.nw.ru/command.php?a=externalrequest&request_type=departments&format=crc";
    public const string labelsFileName = "labels.txt";
    public const string hashFileName = "hash.txt";

    // Names
    public const string ButtonFrom_DefaultName = "ОТКУДА";
    public const string ButtonTo_DefaultName = "КУДА";
    public const string NavigationSceneName = "Navigation";
    public const string ScanerSceneName = "Scaner";

    // Colors
    public static Color LightYellowColor = new Color(1f, 0.92f, 0.16f, 1.0f);
    public static Color LightRedColor = new Color(1f, 0.41f, 0.36f, 1.0f);
    public static Color LightBlueColor = new Color(0.28f, 0.65f, 1f, 1.0f);
    public static Color DefaultLabelColor = Color.white;

    // Events
    public const string floorChanged = "floorChanged";
    public const string qrDetected = "qrDetected";
    public const string labelsLoaded = "labelsLoaded";

    // JSON fields
    public const string JSON_ID = "id";
    public const string JSON_NAME = "name";
    public const string JSON_FULLNAME = "fullname";
    public const string JSON_KEY = "key";
    public const string JSON_URL = "url";
    public const string JSON_DATA = "data";
    public const string JSON_LOCATION = "location";
    public const string JSON_ROOMS = "rooms";
    public const string JSON_NUMBER = "number";
    public const string JSON_FLOOR = "floor";
    public const string JSON_INFO = "description";
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
        if (sVector == null)
            return Vector3.zero;
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }
        
        string[] array = sVector.Split(',');

#if UNITY_EDITOR // в редакторе не парсится str->float, если число с точкой  ¯\_(ツ)_/¯ 
        array[0] = array[0].Replace(".", ",");
        array[1] = array[1].Replace(".", ",");
        array[2] = array[2].Replace(".", ",");
#endif

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

    public static string DecodeUrlString(string url)
    {
        return Regex.Replace(url, @"\\u([0-9A-Fa-f]{4})", m => "" + (char)System.Convert.ToInt32(m.Groups[1].Value, 16));
    }
}
