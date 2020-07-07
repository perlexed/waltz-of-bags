using System;
using Lean.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace GameplayModule
{
    public class UiController : MonoBehaviour
    {
        public GameObject menu;
        public Dropdown languageSelector;
        public LeanLocalization localization;

        private bool _isMenuShown;
        private TimelineController _timelineController;

        public void Start()
        {
            _timelineController = gameObject.GetComponent<TimelineController>();
        }

        public void OnSoundToggle(bool isSoundOn)
        {
            AudioListener.volume = isSoundOn ? 1f : 0;
        }

        public void OnExitButton()
        {
            Application.Quit();
        }

        private void ToggleMenu()
        {
            _isMenuShown = !_isMenuShown;
            _timelineController.TogglePause(_isMenuShown);
            menu.SetActive(_isMenuShown);
        }

        public void OnLanguageSelectorChange()
        {
            localization.SetCurrentLanguage(languageSelector.value);
        }

        public void OnMenuClick()
        {
            ToggleMenu();
        }

        public void Update()
        {
            if (Input.GetButtonUp("Cancel"))
            {
                ToggleMenu();
            }
        }
    }
}
