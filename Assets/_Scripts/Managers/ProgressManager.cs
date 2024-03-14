using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }

    [SerializeField]
    private Level[] _levels;

    private Level _levelToLoad;

    public Level[] Levels { get { return _levels; } }
    public Level LevelToLoad
    {
        set => _levelToLoad = value;
        get { return _levelToLoad; }
    }

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