using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QRValidator : MonoBehaviour
{
    private UnityAction qrDetectedListener;

    void Start()
    {
        
    }

    private void Awake()
    {
        qrDetectedListener = new UnityAction(OnQrDetected);
    }

    private void OnEnable()
    {
        EventManager.StartListening(AppUtils.qrDetected, qrDetectedListener);
    }

    private void OnDisable()
    {
        EventManager.StopListening(AppUtils.qrDetected, qrDetectedListener);
    }

    private void OnQrDetected()
    {
        // проверка валидности маркера
    }

    void Update()
    {
        
    }
}
