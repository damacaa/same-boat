using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderCameraSetter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (Singleton.Instance)
        {
            var canvas = GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;

            canvas.worldCamera.gameObject.SetActive(false);
            canvas.worldCamera = Camera.main;

            canvas.planeDistance = 1;
            canvas.sortingLayerID = SortingLayer.NameToID("UI");
            canvas.sortingOrder = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
