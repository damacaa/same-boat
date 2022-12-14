using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    [SerializeField]
    float _cursorHeight = .05f;

    [Header("Cursor smoothing")]
    [SerializeField]
    float _smoothTime = 0.1f;
    [SerializeField]
    float _maxSmoothSpeed = 1f;
    Vector3 _cursorVelocity;

    [Header("Line width")]
    [SerializeField]
    float _lineWidthSmoothTime = 0.1f;
    [SerializeField]
    float _maxLineWidth = 1f;
    float _lineWidth = 0f;

    LineRenderer _lineRenderer;
    Transform _a;
    Transform _b;

    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (_a && _b)
        {
            _lineRenderer.SetPosition(0, new Vector3(_a.position.x, _a.position.y, -_cursorHeight));
            _lineRenderer.SetPosition(1, Vector3.SmoothDamp(_lineRenderer.GetPosition(1), new Vector3(_b.position.x, _b.position.y, -_cursorHeight), ref _cursorVelocity, _smoothTime, _maxSmoothSpeed));

            _lineWidth = Mathf.Min(_lineWidth + Time.deltaTime / _lineWidthSmoothTime, _maxLineWidth);

            if (Vector3.Distance(_lineRenderer.GetPosition(0), _lineRenderer.GetPosition(1)) < 0.1f)
            {
                _b = null;
            }
        }
        else
        {
            Vector3 v = (_lineRenderer.GetPosition(1) - _lineRenderer.GetPosition(0));
            _lineRenderer.SetPosition(1, _lineRenderer.GetPosition(1) - v * Time.deltaTime * 10f);

            if (v.magnitude < 0.01f)
                _lineWidth = Mathf.Max(_lineWidth - Time.deltaTime / _lineWidthSmoothTime, 0);
        }

        _lineRenderer.startWidth = _lineWidth;
    }

    public void Begin(Transform t)
    {
        _a = t;
        _b = null;

        _lineRenderer.SetPosition(0, new Vector3(_a.position.x, _a.position.y, -_cursorHeight));
        _lineRenderer.SetPosition(1, new Vector3(_a.position.x, _a.position.y, -_cursorHeight));
        _cursorVelocity = Vector3.zero;
        _lineWidth = 0f;
    }

    public void End(Transform t)
    {
        _b = t;
    }

    public void Reset()
    {
        _a = null;
        _b = null;
    }
}
