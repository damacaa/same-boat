using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuFallingSprite : MonoBehaviour
{
    public Vector3 Velocity;
    float w = 0;

    private void Start()
    {
        w = Random.Range(-90f, 90f);
        transform.Rotate(new Vector3(0, 0, w));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Velocity == null)
            return;

        transform.position += Velocity * Time.fixedDeltaTime;

        transform.Rotate(new Vector3(0, 0, w * Time.fixedDeltaTime));

        if (transform.position.z > 0)
            Destroy(gameObject);
    }
}
