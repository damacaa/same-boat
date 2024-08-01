using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class WaveAspectRatioUpdater : MonoBehaviour
{
    [SerializeField]
    private List<Image> _waveList;

    private float _aspectRatio;

    private void Awake()
    {
        _aspectRatio = (float)Screen.width / Screen.height;
    }

    private void Update()
    {
        float aspectRatio = (float)Screen.width / Screen.height;

        if (aspectRatio == _aspectRatio)
            return;

        _aspectRatio = aspectRatio;
        foreach (var wave in _waveList)
        {
            wave.material.SetFloat("_AspectRatio", _aspectRatio);
        }
    }
}
