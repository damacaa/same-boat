using UnityEngine;

public class Singleton : MonoBehaviour
{
    public static Singleton Instance;

    private void Start()
    {
        DontDestroyOnLoad(this);

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
}