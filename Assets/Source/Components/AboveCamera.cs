using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AboveCamera : MonoBehaviour
{
    bool isDragging;
    public bool active;
    Camera cam;
    public float zoomSpeed = 0.05f;

    private void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographicSize = 25;
        cam.gameObject.SetActive(true);
        cam.orthographic = true;
        active = true;
    }

    private void Update()
    {
        /* Блокировка нажатий сквозь интерфейс */
        /*if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        */

        if (AppUtils.IsPointerOverUIObject())
        {
            return;
        }
        /**************************************/

        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        Vector3 newPosition = transform.position;

        if (isDragging || Input.touchCount > 0)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                Touch touch = Input.GetTouch(0);
                newPosition.x -= touch.deltaPosition.x / 50f;
                newPosition.z -= touch.deltaPosition.y / 50f;

                if (Input.touchCount == 2)
                {
                    // Store both touches.
                    Touch touchZero = Input.GetTouch(0);
                    Touch touchOne = Input.GetTouch(1);

                    // Find the position in the previous frame of each touch.
                    Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                    Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                    // Find the magnitude of the vector (the distance) between the touches in each frame.
                    float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                    float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                    // Find the difference in the distances between each frame.
                    float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                    // ... change the orthographic size based on the change in distance between the touches.
                    cam.orthographicSize += deltaMagnitudeDiff * zoomSpeed;

                    //Make sure the orthographic size never drops below zero.
                    cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, 10f, 70f);
                }
            }
            else
            {
                newPosition.x -= Input.GetAxis("Mouse X");
                newPosition.z -= Input.GetAxis("Mouse Y");
            }
        }

        // zoom       
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            if (Application.platform == RuntimePlatform.Android)
                return;

            float newOthographicSize = cam.orthographicSize - Input.GetAxis("Mouse ScrollWheel") * 10;
            if (newOthographicSize > 0)
            {
                cam.orthographicSize = newOthographicSize;
            }
        }
        
        transform.position = newPosition;
    }

    public void ZoomIn(float zoom_coeff)
    {
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - zoom_coeff, 10f, 40f);
    }

    public void ZoomOut(float zoom_coeff)
    {
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + zoom_coeff, 10f, 40f);
    }
}