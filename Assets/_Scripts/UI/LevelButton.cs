using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LevelButton : MonoBehaviour
    {
        [SerializeField]
        private Button _button;
        [SerializeField]
        private TextMeshProUGUI _text;

        private Level _level;

        private bool _initialized;

        public event Action<Level> OnLevelSelected;

        public string Text
        {
            set => _text.text = value;
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }

        public void Init(Level level)
        {
            if (_initialized)
            {
                return;
            }

            _level = level;

            // For each level's island
            for (int i = 0; i < _level.Islands.Length; i++)
            {
                Level.Island island = _level.Islands[i];
            }

            _button.onClick.AddListener(ButtonAction);

            _initialized = true;
        }

        public void Select()
        {
            ButtonAction();
        }

        private void ButtonAction()
        {
            OnLevelSelected.Invoke(_level);
        }
    }
}