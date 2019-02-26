using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

public class AppUtils
{
    public const string labelsURL = "http://127.0.0.1:8000/api/codes/";
    public const string labelsLocalFileName = "labels.txt";


    // JSON поля
    public const string JSON_ID = "id";
    public const string JSON_NAME = "name";
    public const string JSON_KEY = "key";
    public const string JSON_SLIDE = "slide";
    public const string JSON_URL = "url";
    public const string JSON_DATA = "data";
    public const string JSON_MONITOR = "monitor";
    public const string JSON_MEDIAS = "medias";
    public const string JSON_LOCATION = "location";
    public const string JSON_FLOOR = "floor";
    public const string JSON_MESSAGE = "message";
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
}
