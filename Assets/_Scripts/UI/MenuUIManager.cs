using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuUIManager : MonoBehaviour
{
    [SerializeField] private UIDocument _menuUI;
    [SerializeField] private UIDocument _levelSelectUI;
    [SerializeField] private UIDocument _optionsUI;

    private VisualElement _menuUICanvas;
    private VisualElement _levelSelectUICanvas;
    private VisualElement _optionsUICanvas;

    [Space(10)]
    public bool GoStraightToGame;
    public Level level;

    private void Awake()
    {
        _menuUICanvas = _menuUI.rootVisualElement.Q("Canvas");
        _levelSelectUICanvas = _levelSelectUI.rootVisualElement.Q("Canvas");
        _optionsUICanvas = _optionsUI.rootVisualElement.Q("Canvas");
    }

    private void Start()
    {
        if (GoStraightToGame)
        {
            SceneManager.LoadScene(1);

            SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) =>
            {
                GameManager game = FindObjectOfType<GameManager>();
                if (game)
                    game.LoadLevel(level);
            };
        }
    }

    #region Buttons Actions
    public void OpenMenu()
    {
        _menuUICanvas.ToggleInClassList("hide");
    }
    public void CloseMenu()
    {
        _menuUICanvas.ToggleInClassList("hide");
    }
    public void OpenLevelSelect()
    {
        _levelSelectUICanvas.ToggleInClassList("hide");
    }
    public void CloseLevelSelect()
    {
        _levelSelectUICanvas.ToggleInClassList("hide");
    }
    public void OpenOptions()
    {
        _optionsUICanvas.ToggleInClassList("hide");
    }
    public void CloseOptions()
    {
        _optionsUICanvas.ToggleInClassList("hide");
    }
    #endregion
}