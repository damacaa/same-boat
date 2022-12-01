using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private UIDocument _gameUI;
    [SerializeField] private UIDocument _optionsUI;
    [SerializeField] private UIDocument _gameOverUI;

    private VisualElement _gameUICanvas;
    private VisualElement _gameOverUICanvas;
    private VisualElement _optionsUICanvas;
    private VisualElement _levelDescriptionCanvas;

    private Label _numberOfCrossings;
    private Label _levelDescription;

    private Button _returnToMenuButton;
    private Button _retryButton;

    private void Awake()
    {
        _gameUICanvas = _gameUI.rootVisualElement.Q("Canvas");
        _optionsUICanvas = _optionsUI.rootVisualElement.Q("Canvas");
        _gameOverUICanvas = _gameOverUI.rootVisualElement.Q("Canvas");

        _levelDescriptionCanvas = _gameUICanvas.Q<VisualElement>("LevelDescriptionContainer");
        _levelDescription = _gameUICanvas.Q<Label>("LevelDescription");
        _numberOfCrossings = _gameUICanvas.Q<Label>("NumberOfCrossingsValue");

        _gameUICanvas.Q<Button>("UndoButton").clicked += delegate { GameManager.instance.Game.Undo(); };
        _gameUICanvas.Q<Button>("OptionsButton").clicked += OpenOptions;
        _gameUICanvas.Q<Button>("CloseLevelDescriptionButton").clicked +=
            delegate { _levelDescriptionCanvas.ToggleInClassList("hide"); };
        _gameUICanvas.Q<Button>("ShowLevelDescriptionButton").clicked +=
            delegate { _levelDescriptionCanvas.ToggleInClassList("hide"); };

        _returnToMenuButton = _gameOverUICanvas.Q<Button>("ReturnToMenuButton");
        _retryButton = _gameOverUICanvas.Q<Button>("RetryButton");

        _returnToMenuButton.clicked += () => { SceneManager.LoadScene(0); };
        _retryButton.clicked += () => { GameManager.instance.Reset(); };
        _retryButton.clicked += CloseGameOver;

        GameManager.instance.OnLevelLoaded += SetCrossingsChangeListener;
        GameManager.instance.OnLevelLoaded += ShowLevelDescription;
        GameManager.instance.OnGameFinished += ShowEndGame;
    }

    #region Buttons Actions
    public void OpenOptions()
    {
        _optionsUICanvas.ToggleInClassList("hide");
    }
    public void CloseOptions()
    {
        _optionsUICanvas.ToggleInClassList("hide");
    }
    public void CloseGameOver()
    {
        _gameOverUICanvas.ToggleInClassList("hide");
    }
    public void GoToMenuScene()
    {
        SceneManager.LoadScene(0);
    }
    #endregion

    public void SetCrossingsChangeListener()
    {
        GameManager.instance.Game.Boat.OnCrossingsChange +=
            (int v) => { _numberOfCrossings.text = v.ToString(); };
    }
    public void ShowLevelDescription()
    {
        _levelDescription.text = GameManager.instance.LevelDescription;
        _levelDescriptionCanvas.ToggleInClassList("hide");
    }
    public void ShowEndGame()
    {
        _gameOverUICanvas.ToggleInClassList("hide");
    }
}