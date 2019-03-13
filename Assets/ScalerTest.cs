using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScalerTest : MonoBehaviour
{
    public Text scaleX, scaleY;
    public RectTransform cameraRect;

    // Start is called before the first frame update
    public void UpdScale()
    {
        scaleX.text = cameraRect.localScale.x.ToString();
        scaleY.text = cameraRect.localScale.y.ToString();
    }

    public void OnSliderXValueChange(float value)
    {
        Vector3 tmp = cameraRect.localScale;
        cameraRect.localScale = new Vector3(value * 3, tmp.y);
        scaleX.text = cameraRect.localScale.x.ToString();
    }

    public void OnSliderYValueChange(float value)
    {
        Vector3 tmp = cameraRect.localScale;
        cameraRect.localScale = new Vector3(tmp.x, value * 3);
        scaleY.text = cameraRect.localScale.y.ToString();
    }
}
