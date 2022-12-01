using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private UIDocument _gameUI;
    [SerializeField] private UIDocument _optionsUI;

    private VisualElement _gameUICanvas;
    private VisualElement _optionsUICanvas;
    private VisualElement _levelDescriptionCanvas;

    private Label _numberOfCrossings;
    private Label _levelDescription;

    private void Awake()
    {
        _gameUICanvas = _gameUI.rootVisualElement.Q("Canvas");
        _optionsUICanvas = _optionsUI.rootVisualElement.Q("Canvas");

        _levelDescriptionCanvas = _gameUICanvas.Q<VisualElement>("LevelDescriptionContainer");
        _levelDescription = _gameUICanvas.Q<Label>("LevelDescription");

        _gameUICanvas.Q<Button>("UndoButton").clicked += delegate { GameManager.instance.Game.Undo(); };
        _gameUICanvas.Q<Button>("OptionsButton").clicked += OpenOptions;
        _gameUICanvas.Q<Button>("CloseLevelDescriptionButton").clicked +=
            delegate { _levelDescriptionCanvas.ToggleInClassList("hide"); };
        _gameUICanvas.Q<Button>("ShowLevelDescriptionButton").clicked +=
            delegate { _levelDescriptionCanvas.ToggleInClassList("hide"); };

        _numberOfCrossings = _gameUICanvas.Q<Label>("NumberOfCrossingsValue");
        GameManager.instance.OnLevelLoaded += SetCrossingsChangeListener;
        GameManager.instance.OnLevelLoaded += ShowLevelDescription;
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
}