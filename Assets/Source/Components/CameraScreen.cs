using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ArucoModule;
using ZXing;

public class CameraScreen : MonoBehaviour {

    public bool searchForAruco = false; // искать aruco маркер в кадре?
    public bool searchForQr = true; // искать qr-метку в кадре?
    public bool allowFrontCam = false;
    public RawImage background;
    public AspectRatioFitter fit;
    public Text info; // поле для вывода текста ошибок

    bool camAvailable;
    private WebCamTexture backCam;
    private Texture2D texture;
    private Texture defaultBackground;
    private WebCamDevice[] devices;
    private int cameraId = 0;
    private int frames; // счетчик фреймов
    
    /* для aruco сканера */
    private Dictionary dictionary;
    private Mat rgbaMat;
    private Color32[] colors;
    private List<Mat> corners;
    private Mat ids;
    private DetectorParameters detectorParams;
    private List<Mat> rejectedCorners;
    private Mat camMatrix;
    private MatOfDouble distCoeffs;
    private Mat rvecs, tvecs;
    /* ----------------- */

    /* для qr сканера */
    BarcodeReader qrReader;
    private string qrInfo;
    private Color[] originalc;
    private Color32[] targetColorARR;
    private int W, H;
#if UNITY_IOS
	int blockWidth = 450;
#elif UNITY_ANDROID
    int blockWidth = 350;
#else
	int blockWidth = 350;
#endif
    /* ----------------- */    
    
    IEnumerator Start () {
        // ждем подтверждения разрешения на исползование камеры
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);

        Screen.orientation = ScreenOrientation.Portrait;
        frames = 0;
        defaultBackground = background.texture;
        devices = WebCamTexture.devices;
        
        // если камеры не найдены: ошибка.
        if (devices.Length == 0)
        {
            info.text = "Ошибка: Камера не найдена";
            background.gameObject.SetActive(false);
            camAvailable = false;
            yield break;
        }

        // перебор всех доступных камер
        for (int i = 0; i < devices.Length; i++)
        {
            // если камера не фронтальная, то используем её
            if (!devices[i].isFrontFacing || allowFrontCam)
            {
                backCam = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
                cameraId = i;
            }
        }

        // если задних камер не найдено: ошибка
        if (backCam == null)
        {
            info.text = "Ошибка: Задняя камера не найдена";
            background.gameObject.SetActive(false);
            yield break;
        }

        // включаем камеру
        backCam.Play();
        camAvailable = true;

        texture = new Texture2D(backCam.width, backCam.height, TextureFormat.RGB24, false);
        //background.texture = texture;
        background.texture = backCam;

        /* Инициализация переменных для работы aruco сканера */
        dictionary = Aruco.getPredefinedDictionary(Aruco.DICT_5X5_100);
        rgbaMat = new Mat(backCam.height, backCam.width, CvType.CV_8UC3);
        colors = new Color32[backCam.width * backCam.height];
        corners = new List<Mat>();
        ids = new Mat();
        detectorParams = DetectorParameters.create();
        rejectedCorners = new List<Mat>();

        camMatrix = CreateCameraMatrix(backCam.width, backCam.height);
        distCoeffs = new MatOfDouble(0, 0, 0, 0, 0);
        rvecs = new Mat();
        tvecs = new Mat();
        /*---------------------------------------------------*/

        qrReader = new BarcodeReader();
        qrReader.AutoRotate = true;
        qrReader.TryInverted = true;

        OnCameraItit();
    }
	
	void Update () {
        if (!camAvailable)
            return;
        frames++;


        /*
        Utils.webCamTextureToMat(backCam, rgbaMat, colors);

        if (((frames+10) % 20 == 0) && searchForAruco)
        {
            DetectAruco();
        }

        Utils.fastMatToTexture2D(rgbaMat, texture);
        */

        if ((frames % 15 == 0) && searchForQr)
        {
            DetectQR();
        }
    }

    private Mat CreateCameraMatrix(float width, float height)
    {
        int max_d = (int)Mathf.Max(width, height);
        double fx = max_d;
        double fy = max_d;
        double cx = width / 2.0f;
        double cy = height / 2.0f;

        Mat camMatrix = new Mat(3, 3, CvType.CV_64FC1);
        camMatrix.put(0, 0, fx);
        camMatrix.put(0, 1, 0);
        camMatrix.put(0, 2, cx);
        camMatrix.put(1, 0, 0);
        camMatrix.put(1, 1, fy);
        camMatrix.put(1, 2, cy);
        camMatrix.put(2, 0, 0);
        camMatrix.put(2, 1, 0);
        camMatrix.put(2, 2, 1.0f);

        return camMatrix;
    }

    // настройка правильных пропорций изображения камеры
    private void OnCameraItit()
    {
        int orient = -backCam.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);

        int ScreenWidth = Mathf.Max(Screen.width, Screen.height);
        int ScreenHeight = Mathf.Min(Screen.width, Screen.height);

        int CamWidth = Mathf.Max(backCam.width, backCam.height);
        int CamHeight = Mathf.Min(backCam.width, backCam.height);
        
        float camRatio = CamWidth * 1.0f/ CamHeight;
        float screenRatio = ScreenWidth * 1.0f / ScreenHeight;

        if(Mathf.Abs(orient) == 90)
        {
            background.rectTransform.localScale = new Vector3(screenRatio, 1 / camRatio, 1);
        }
        else
        {
            background.rectTransform.localScale = new Vector3(screenRatio * camRatio, 1, 1);
        }
    }

    // распознавание qr-метки
    public void DetectQR()
    {
        W = backCam.width;
        H = backCam.height;

        blockWidth = System.Math.Min(W, H);

        if (targetColorARR == null)
        {
            targetColorARR = new Color32[blockWidth * blockWidth];
        }

        int posx = ((W - blockWidth) >> 1);
        int posy = ((H - blockWidth) >> 1);
        originalc = backCam.GetPixels(posx, posy, blockWidth, blockWidth);

        for (int i = 0; i != blockWidth; i++)
        {
            for (int j = 0; j != blockWidth; j++)
            {
                targetColorARR[i + j * blockWidth].r = (byte)(originalc[i + j * blockWidth].r * 255);
                targetColorARR[i + j * blockWidth].g = (byte)(originalc[i + j * blockWidth].g * 255);
                targetColorARR[i + j * blockWidth].b = (byte)(originalc[i + j * blockWidth].b * 255);
                targetColorARR[i + j * blockWidth].a = 255;
            }
        }

        Loom.RunAsync(() =>
        {
            try
            {
                Result data = qrReader.Decode(targetColorARR, blockWidth, blockWidth);
                if (data != null)
                {
                    #if UNITY_ANDROID || UNITY_IOS
                    Handheld.Vibrate();
                    #endif

                    EventManager.TriggerEvent(AppUtils.qrDetected, data.Text);
                    Debug.Log("qr found");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Decode Error: " + e.Data.ToString());
            }
        });
    }

    // распознавание aruco маркера
    public void DetectAruco()
    {        
        Aruco.detectMarkers(rgbaMat, dictionary, corners, ids, detectorParams, rejectedCorners);
        
        // если в кадре обнаружен только один aruko маркер
        if (ids.total() == 1)
        {
            Aruco.drawDetectedMarkers(rgbaMat, corners, ids);
            float markerLength = 0.05f;
            Aruco.estimatePoseSingleMarkers(corners, markerLength, camMatrix, distCoeffs, rvecs, tvecs);

            double[] rVectorArr = rvecs.get(0, 0);
            Vector3 rVector = new Vector3((float)rVectorArr[0], (float)rVectorArr[1], (float)rVectorArr[2]);
            double[] tVectorArr = tvecs.get(0, 0);
            Vector3 tVector = new Vector3((float)tVectorArr[0], (float)tVectorArr[1], (float)tVectorArr[2]);

            Debug.Log($"id маркера: {ids.get(0, 0)[0]}. Distance*:  {tVector.magnitude}");

            /* Нарисовать оси*/
            /*
            for (int i = 0; i < ids.total(); i++)
            {
                using (Mat rvec = new Mat(rvecs, new OpenCVForUnity.Rect(0, i, 1, 1)))
                using (Mat tvec = new Mat(tvecs, new OpenCVForUnity.Rect(0, i, 1, 1)))
                {
                    // In this example we are processing with RGB color image, so Axis-color correspondences are X: blue, Y: green, Z: red. (Usually X: red, Y: green, Z: blue)
                    Aruco.drawAxis(rgbaMat, camMatrix, distCoeffs, rvec, tvec, markerLength * 0.5f);
                }
            }
            */
            /*----------------------------------------------------------------*/
        }
    }

    // переключение камеры (не используется)
    public void SwitchCam()
    {
        if (backCam)
        {
            cameraId++;
            cameraId = cameraId >= devices.Length ? 0 : cameraId;

            backCam = new WebCamTexture(devices[cameraId].name, Screen.width, Screen.height);

            backCam.Play();
            background.texture = backCam;

            camAvailable = true;
            OnCameraItit();

        }
    }

    // выключение активной камеры
    public void TurnOffCam()
    {
        if (camAvailable)
        { 
            backCam.Stop();
            camAvailable = false;
        }
    }
}
