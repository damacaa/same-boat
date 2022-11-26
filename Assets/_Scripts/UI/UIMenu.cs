using UnityEngine;
using UnityEngine.UIElements;

public class UIMenu : MonoBehaviour
{
    [SerializeField] private UIManager _uiManager;

    private VisualElement _root;
    private VisualElement _canvas;
    private Button _playButton;

    private void Awake()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;

        _canvas = _root.Q<VisualElement>("Canvas");
        _canvas.AddToClassList("hide");
        _canvas.ToggleInClassList("hide");

        _playButton = _root.Q<Button>("PlayButton");
        _playButton.clicked += _uiManager.GoToLevelSelect;
    }
}