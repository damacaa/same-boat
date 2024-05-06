using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    [SerializeField]
    private int _targetFramerate = 30;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = _targetFramerate;
    }
}
