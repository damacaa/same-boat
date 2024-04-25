using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }

    public static readonly string COMPLETED_LEVELS = "progress";

    [SerializeField]
    int _completedLevels;
    public int CurrentLevel { get { return _completedLevels; } }

    [SerializeField]
    private LevelCollection _levels;

    private Level _levelToLoad;

    public LevelCollection Levels { get { return _levels; } }
    public Level LevelToLoad
    {
        set => _levelToLoad = value;
        get { return _levelToLoad; }
    }

    [SerializeField]
    private bool _everythingUnlocked;


    private void Awake()
    {
        if (Instance)
            Destroy(this);
        else
            Instance = this;

        if (_everythingUnlocked)
            _completedLevels = int.MaxValue;
        else
            _completedLevels = PlayerPrefs.GetInt(COMPLETED_LEVELS);
    }

    void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void CompleteLevel()
    {
        try
        {
            if (Levels[_completedLevels] == _levelToLoad)
            {
                _completedLevels++;
                SaveProgress();
            }
        }
        catch (System.Exception)
        {
        }

    }

    private void OnDestroy()
    {
        SaveProgress();
    }

    private void SaveProgress()
    {
        PlayerPrefs.SetInt(COMPLETED_LEVELS, _completedLevels);
        PlayerPrefs.Save();
    }
}