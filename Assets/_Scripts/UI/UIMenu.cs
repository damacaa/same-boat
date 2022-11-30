using UnityEngine;
using UnityEngine.UIElements;

public class UIMenu : MonoBehaviour
{
    [SerializeField] private MenuUIManager _uiManager;

    private VisualElement _root;
    private VisualElement _canvas;
    private Button _optionsButton;
    private Button _playButton;

    private void Awake()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;

        _canvas = _root.Q<VisualElement>("Canvas");
        _canvas.AddToClassList("hide");
        _canvas.ToggleInClassList("hide");

        _optionsButton = _root.Q<Button>("OptionsButton");
        _playButton = _root.Q<Button>("PlayButton");

        _optionsButton.clicked += _uiManager.OpenOptions;
        _optionsButton.clicked += _uiManager.CloseMenu;
        _playButton.clicked += _uiManager.OpenLevelSelect;
        _playButton.clicked += _uiManager.CloseMenu;
    }
}