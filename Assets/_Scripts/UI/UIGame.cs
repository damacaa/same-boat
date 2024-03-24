using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIGame : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI _descriptionText;
    [SerializeField]
    TextMeshProUGUI _crossingsCounterText;

    internal void SetCrossings(int crossings)
    {
        _crossingsCounterText.text = crossings.ToString();
    }

    internal void SetDescription(string levelDescription)
    {
        _descriptionText.text = levelDescription;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
