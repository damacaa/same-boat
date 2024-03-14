using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class LevelsView : MonoBehaviour
    {
        [SerializeField]
        private Button _playButton;
        [Header("Preview")]
        [SerializeField]
        private Image _levelPreview;
        [SerializeField]
        private GameObject _transportablesContainer;
        [SerializeField]
        private GameObject _imagePrefab;
        [Header("Levels")]
        [SerializeField]
        private GameObject _levelsContainer;
        [SerializeField]
        private GameObject _levelButtonPrefab;
        [SerializeField]
        private List<Level> _levels;

        private List<LevelButton> _levelsButtons = new();

        private void Start()
        {
            #region NULL_CHECKS
            if (_levels == null || _levels.Count == 0)
            {
#if UNITY_EDITOR
                Debug.LogError("No available levels to select. Fill with levels scriptables.");
#endif
                return;
            }

            if (_levelButtonPrefab == null)
            {
#if UNITY_EDITOR
                Debug.LogError("Required button prefab not available.");
#endif
                return;
            }

            if (_levelsContainer == null)
            {
#if UNITY_EDITOR
                Debug.LogError("Required container not available.");
#endif
                return;
            }

            if (_playButton == null)
            {
#if UNITY_EDITOR
                Debug.LogError("Play button reference not available.");
#endif
                return;
            }
            #endregion

            _playButton.onClick.AddListener(LoadLevel);

            for (int i = 0; i < _levels.Count; i++)
            {
                GameObject go = Instantiate(_levelButtonPrefab, _levelsContainer.transform);

                if (go.TryGetComponent(out LevelButton levelButton))
                {
                    levelButton.Text = _levels[i].name;
                    levelButton.Init(_levels[i], _levelPreview, _transportablesContainer);
                    _levelsButtons.Add(levelButton);
                }
                else
                {
#if UNITY_EDITOR
                    Debug.LogError("Required level button script not found.");
                    return;
#endif
                    continue;
                }
            }

            if (_levels.Count > 0)
            {
                _levelsButtons[0].Select();
            }
        }

        private void LoadLevel()
        {
            SceneManager.LoadScene(1);
        }
    }
}