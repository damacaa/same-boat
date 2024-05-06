using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortBehaviour : MonoBehaviour
{
    Rigidbody _rb;
    float _t = 0;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _t += Time.fixedDeltaTime;
        if (_t > 0.1f && _rb.velocity.sqrMagnitude < 0.5f)
        {
            gameObject.SetActive(false);
        }
    }
}
