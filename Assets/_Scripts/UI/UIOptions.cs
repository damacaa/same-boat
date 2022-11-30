using UnityEngine;
using UnityEngine.UIElements;

public class UIOptions : MonoBehaviour
{
    [SerializeField] private MenuUIManager _menuUIManager;
    [SerializeField] private GameUIManager _gameUIManager;
    [SerializeField] private bool _isGameOptions;

    private VisualElement _canvas;

    private Button _closeOptionsButton;
    private Button _backToMenuButton;
    private Button _musicButton;
    private Slider _musicSlider;
    private Button _vfxButton;
    private Slider _vfxSlider;

    private void Awake()
    {
        _canvas = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("Canvas");

        _canvas.AddToClassList("hide");

        _closeOptionsButton = _canvas.Q<Button>("CloseOptionsButton");
        _musicButton = _canvas.Q<Button>("MusicToggle");
        _musicSlider = _canvas.Q<Slider>("MusicSlider");
        _vfxButton = _canvas.Q<Button>("VFXToggle");
        _vfxSlider = _canvas.Q<Slider>("VFXSlider");

        Debug.Log(_canvas.Q<Button>("CloseOptionsButton").text);

        if (_isGameOptions)
        {
            _backToMenuButton = _canvas.Q<Button>("BackToMenuButton");
            _closeOptionsButton.clicked += _gameUIManager.CloseOptions;
            _backToMenuButton.clicked += _gameUIManager.GoToMenuScene;
        }
        else
        {
            _closeOptionsButton.clicked += _menuUIManager.CloseOptions;
            _closeOptionsButton.clicked += _menuUIManager.OpenMenu;
        }

        _musicButton.clicked += () => { };
        _vfxButton.clicked += () => { };

        _musicSlider.RegisterValueChangedCallback(evt =>
        {
            Debug.Log(evt.newValue);
        });
        _vfxSlider.RegisterValueChangedCallback(evt =>
        {
            Debug.Log(evt.newValue);
        });
    }
}
