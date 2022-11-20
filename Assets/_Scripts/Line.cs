using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    [SerializeField]
    GameObject _a;
    [SerializeField]
    GameObject _b;

    LineRenderer _lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_a && _b)
        {
            _lineRenderer.SetPosition(0, _a.transform.position);
            _lineRenderer.SetPosition(1, _b.transform.position);
        }
    }
}
