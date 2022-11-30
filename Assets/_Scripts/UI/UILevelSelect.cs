using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UILevelSelect : MonoBehaviour
{
    [SerializeField] private MenuUIManager _uiManager;
    [SerializeField] Level[] _levels;

    private List<Button> _levelsButtons;

    private VisualElement _root;
    private VisualElement _canvas;
    private VisualElement.Hierarchy _buttonsList;
    private Button _backButton;

    private void Awake()
    {
        _levelsButtons = new List<Button>();

        _root = GetComponent<UIDocument>().rootVisualElement;

        _canvas = _root.Q<VisualElement>("Canvas");
        _canvas.AddToClassList("hide");

        _buttonsList = _root.Q<ListView>("ButtonsList").hierarchy;

        for (int i = 0; i < _levels.Length; i++)
        {
            _levelsButtons.Add(new Button());
            _levelsButtons[i].text = $"Level {i + 1}";
            _levelsButtons[i].AddToClassList("level-list-button");
            Level level = _levels[i];
            _levelsButtons[i].clicked += delegate { StartLevel(level); };
            _buttonsList.Add(_levelsButtons[i]);
        }

        _backButton = _root.Q<Button>("Back");
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