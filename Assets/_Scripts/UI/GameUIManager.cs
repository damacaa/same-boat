using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private UIDocument _gameUI;
    [SerializeField] private UIDocument _optionsUI;

    private VisualElement _gameUICanvas;
    private VisualElement _optionsUICanvas;

    private Label _numberOfMovementsLabel;
    private Label _animalsDelivered;
    private Label _currentLevel;

    private Button _undoButton;
    private Button _optionsButton;

    private void Awake()
    {
        _gameUICanvas = _gameUI.rootVisualElement.Q("Canvas");
        _optionsUICanvas = _optionsUI.rootVisualElement.Q("Canvas");

        _numberOfMovementsLabel = _gameUICanvas.Q<Label>("NumberOfMovements");
        _animalsDelivered = _gameUICanvas.Q<Label>("AnimalsDelivered");
        _currentLevel = _gameUICanvas.Q<Label>("CurrentLevel");

        _undoButton = _gameUICanvas.Q<Button>("UndoButton");
        _optionsButton = _gameUICanvas.Q<Button>("OptionsButton");

        _undoButton.clicked += () => { };
        _optionsButton.clicked += OpenOptions;
    }

    private void Start()
    {

    }

    #region Buttons Actions
    public void SetNumberOfMovements(int num)
    {
        _numberOfMovementsLabel.text = num.ToString();
    }
    public void SetNumberOfAnimalsDelivered(int num, int max)
    {
        _animalsDelivered.text = $"{num}/{max}";
    }
    public void SetCurrentLevelNumber(int num)
    {
        _currentLevel.text = num.ToString();
    }
    public void OpenOptions()
    {
        _optionsUICanvas.ToggleInClassList("hide");
    }
    public void CloseOptions()
    {
        _optionsUICanvas.ToggleInClassList("hide");
    }
    public void GoToMenuScene()
    {
        SceneManager.LoadScene(0);
    }
    #endregion
}