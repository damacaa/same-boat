using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnableDeveloperMode : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private const float TIME_TO_ACTIVATE = 5.0f;
    private float _pointerDownTime;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Down");
        _pointerDownTime = Time.time;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Up");
        if (Time.time - _pointerDownTime >= TIME_TO_ACTIVATE)
        {
            Debug.Log("All levels unlocked");
            ProgressManager.Instance.UnlockAll();
        }
    }



}
