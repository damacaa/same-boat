using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance)
            Destroy(this);
        else
            Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
