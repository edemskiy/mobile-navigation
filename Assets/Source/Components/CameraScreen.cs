using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ArucoModule;
using OpenCVForUnity.UnityUtils;
using UnityEngine.SceneManagement;

using ZXing;

public class CameraScreen : MonoBehaviour {
    // public Button showMapButton;

    bool camAvailable, markerFound;
    private string infoText = null;
    private WebCamTexture backCam;
    private Texture2D texture;
    private Texture defaultBackground;
    private WebCamDevice[] devices;
    private int cameraId = 0;
    private Dictionary dictionary;
    private Mat rgbaMat;
    private Color32[] colors;
    private List<Mat> corners;
    private Mat ids;
    private DetectorParameters detectorParams;
    private List<Mat> rejectedCorners;

    private string qrInfo;

    int frames;
    int W, H;
    private Color[] originalc;
    private Color32[] targetColorARR;
#if UNITY_IOS
	int blockWidth = 450;
#elif UNITY_ANDROID
    int blockWidth = 350;
#else
	int blockWidth = 350;
#endif

    Mat camMatrix;
    MatOfDouble distCoeffs;
    Mat rvecs, tvecs;

    BarcodeReader qrReader;

    public RawImage background;
    public AspectRatioFitter fit;
    public Text info;
    
    void Start () {
        frames = 0;
        defaultBackground = background.texture;
        devices = WebCamTexture.devices;
                
        if (devices.Length == 0)
        {
            info.text = "Камера не найдена";
            camAvailable = false;
            return;
        }

        for (int i = 0; i < devices.Length; i++)
        {
            if (!devices[i].isFrontFacing)
            {
                backCam = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
                cameraId = i;
            }
        }

        if (backCam == null)
        {
            info.text = "Задняя камера не найдена";
            return;
        }

        backCam.Play();
        texture = new Texture2D(backCam.width, backCam.height, TextureFormat.RGB24, false);
        background.texture = texture;

        camAvailable = true;

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

        qrReader = new BarcodeReader();
        qrReader.AutoRotate = true;
        qrReader.TryInverted = true;

        OnCameraItit();
    }
	
	void Update () {
        if (!camAvailable)
            return;
        frames++;

  
        Utils.webCamTextureToMat(backCam, rgbaMat, colors);
        
        if ((frames+10) % 20 == 0)
        {
            //DetectAruco();
        }

        Utils.fastMatToTexture2D(rgbaMat, texture);

        
        if (frames % 20 == 0)
        {
            DetectQR();
        }

        /*
        if (!arucoFound && !qrFound)
        {
            info.text = "No markers detected";
        }
        */

        if (frames > 999999)
            frames = 0;
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

    private void OnCameraItit()
    {
        Debug.Log("screenW: " + Screen.width + ", screenH" + Screen.height);
        Debug.Log("width: " + backCam.width + ", height" + backCam.height);
        float ratio = (float)backCam.width / (float)backCam.height;
        fit.aspectRatio = ratio;

        float scaleY = backCam.videoVerticallyMirrored ? -1f : 1f;
        background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

        int orient = -backCam.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
    }

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
                    // qrInfo = new JSONObject(data.Text).str;
#if UNITY_ANDROID || UNITY_IOS

                    Handheld.Vibrate();

#endif
                    EventManager.TriggerEvent(AppUtils.qrDetected, data.Text);
                    Debug.Log("qr found");
                }
            }
            catch (System.Exception e)
            {
                //Debug.LogError("Decode Error: " + e.Data.ToString());
            }
        });
    }

    public void DetectAruco()
    {
        
        Aruco.detectMarkers(rgbaMat, dictionary, corners, ids, detectorParams, rejectedCorners);
        if (ids.total() == 1)
        {

            Aruco.drawDetectedMarkers(rgbaMat, corners, ids);
            float markerLength = 0.05f;
            Aruco.estimatePoseSingleMarkers(corners, markerLength, camMatrix, distCoeffs, rvecs, tvecs);

            double[] rVectorArr = rvecs.get(0, 0);
            Vector3 rVector = new Vector3((float)rVectorArr[0], (float)rVectorArr[1], (float)rVectorArr[2]);
            double[] tVectorArr = tvecs.get(0, 0);
            Vector3 tVector = new Vector3((float)tVectorArr[0], (float)tVectorArr[1], (float)tVectorArr[2]);

            infoText = "id маркера: " + ids.get(0, 0)[0] + ". Distance*: " + tVector.magnitude;

            /* Draw axes*/
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
        }
        else
        {

            //infoText = "no markers found";
        }        
    }

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

    public void TurnOffCam()
    {
        backCam.Stop();
        camAvailable = false;
    }
}
