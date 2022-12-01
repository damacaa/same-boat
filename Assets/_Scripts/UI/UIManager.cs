using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [SerializeField] private UIDocument _menu;
    [SerializeField] private UIDocument _levelSelect;
    [SerializeField] private UIDocument _options;

    private VisualElement _menuCanvas;
    private VisualElement _levelSelectCanvas;
    private VisualElement _optionsCanvas;

    [Space(10)]
    public bool GoStraightToGame;
    public Level level;

    private void Awake()
    {
        _menuCanvas = _menu.rootVisualElement.Q("Canvas");
        _levelSelectCanvas = _levelSelect.rootVisualElement.Q("Canvas");
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

    public void GoToLevelSelect()
    {
        _menuCanvas.ToggleInClassList("hide");
        _levelSelectCanvas.ToggleInClassList("hide");
    }
    public void GoToMenu()
    {
        _levelSelectCanvas.ToggleInClassList("hide");
        _menuCanvas.ToggleInClassList("hide");
    }
    public void OpenOptions()
    {
        _optionsCanvas.ToggleInClassList("hide");
    }
    public void CloseOptions()
    {
        _optionsCanvas.ToggleInClassList("hide");
    }
}