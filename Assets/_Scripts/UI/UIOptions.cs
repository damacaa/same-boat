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
    private Slider _musicSlider;
    private Slider _vfxSlider;

    private void Awake()
    {
        _canvas = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("Canvas");

        _canvas.AddToClassList("hide");

        _closeOptionsButton = _canvas.Q<Button>("CloseOptionsButton");
        _musicSlider = _canvas.Q<Slider>("MusicSlider");
        _vfxSlider = _canvas.Q<Slider>("VFXSlider");

        Debug.Log(_canvas.Q<Button>("CloseOptionsButton").text);

        if (_isGameOptions)
        {
            _backToMenuButton = _canvas.Q<Button>("BackToMenuButton");
            _backToMenuButton.clicked += delegate { SoundController.Instace.PlaySound(SoundController.Sound.UI); };
            _backToMenuButton.clicked += _gameUIManager.GoToMenuScene;

            _closeOptionsButton.clicked += delegate { SoundController.Instace.PlaySound(SoundController.Sound.UI); };
            _closeOptionsButton.clicked += _gameUIManager.CloseOptions;
        }
        else
        {
            _closeOptionsButton.clicked += delegate { SoundController.Instace.PlaySound(SoundController.Sound.UI); };
            _closeOptionsButton.clicked += _menuUIManager.CloseOptions;
            _closeOptionsButton.clicked += _menuUIManager.OpenMenu;
        }

        _musicSlider.RegisterValueChangedCallback(evt =>
        {
            SoundController.Instace.SetMusicVolume(evt.newValue);
        });
        _vfxSlider.RegisterValueChangedCallback(evt =>
        {
            SoundController.Instace.SetSFXVolume(evt.newValue);
        });
    }
}
