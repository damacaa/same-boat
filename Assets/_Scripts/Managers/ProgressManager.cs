using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }

    public Level[] Levels { get { return _levels; } }

    [SerializeField]
    private Level[] _levels;

    private void Awake()
    {
        if (Instance)
            Destroy(this);
        else
            Instance = this;
    }

    void Start()
    {
        DontDestroyOnLoad(this);
    }
}
