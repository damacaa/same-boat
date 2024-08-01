using UnityEngine;

public class SetCanvasCamera : MonoBehaviour
{
    public Canvas canvas;

    // Start is called before the first frame update
    void Start()
    {
        Camera mainCamera = FindObjectOfType<Camera>();

        if (mainCamera != null)
        {
            canvas.worldCamera = mainCamera;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
