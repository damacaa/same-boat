using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UILevelSelect : MonoBehaviour
{
    [SerializeField] private MenuUIManager _uiManager;
    [SerializeField] Level[] _levels;

    private VisualElement _root;
    private VisualElement _canvas;
    private VisualElement.Hierarchy _buttonsList;
    private Button _backButton;

    private void Awake()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;

        _canvas = _root.Q<VisualElement>("Canvas");
        _canvas.AddToClassList("hide");

        _buttonsList = _root.Q<ListView>("ButtonsList").hierarchy;

        for (int i = 0; i < _levels.Length; i++)
        {
            Button button = new Button();
            button.text = $"Level {i + 1}";
            button.AddToClassList("level-list-button");
            Level level = _levels[i];
            button.clicked += delegate { SoundController.Instace.PlaySound(SoundController.Sound.UI); };
            button.clicked += _uiManager.CloseLevelSelect;
            button.clicked += _uiManager.CloseMenu;
            button.clicked += () =>
            {
                SceneManager.LoadScene(1);

                SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) =>
                {
                    GameManager game = FindObjectOfType<GameManager>();
                    if (game)
                        game.LoadLevel(level);
                };
            };
            _buttonsList.Add(button);
        }

        _backButton = _root.Q<Button>("Back");
        _backButton.clicked += delegate { SoundController.Instace.PlaySound(SoundController.Sound.UI); };
        _backButton.clicked += _uiManager.CloseLevelSelect;
        _backButton.clicked += _uiManager.OpenMenu;
    }

    public void StartLevel(Level level)
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