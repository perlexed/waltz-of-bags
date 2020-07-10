using Enums;
using UnityEngine;
using UnityEngine.UI;

namespace GameplayModule
{
    public class TimelineController : MonoBehaviour
    {
        public bool isRunning = true;
        public GameObject victoryPanel;
        public Text continueText;
        public InteractionManager interactionManager;
        public DifficultyEnum difficulty;
        public Button ResetBagsButton;
        public GameObject loadingText;

        public float victoryWaitTime = 1f;
        private float _victoryTime;
        private bool _isInQuitConfirm;
        private BagController[] _bags;

        private LevelManager _levelManager;
        private CommendationsManager _commendationsManager;

        private bool _isFirstUpdate = true;
        private bool _isSecondUpdate;

        private void Start()
        {
            _levelManager = gameObject.GetComponent<LevelManager>();
            _commendationsManager = gameObject.GetComponent<CommendationsManager>();
        }

        private void CreateLevel()
        {
            _levelManager.CreateRandomLevel(difficulty);
        }

        public void OnBagPickupStatusChange(bool areAllBagsOnShelf, bool areAllBagsOnCart)
        {
            if (areAllBagsOnShelf)
            {
                SetVictory();
            }
            
            ResetBagsButton.interactable = !areAllBagsOnCart;
        }

        private void SetVictory()
        {
            gameObject.GetComponent<AudioSource>().Play();
        
            isRunning = false;
            _victoryTime = Time.time;
            interactionManager.allowInteractions = false;

            victoryPanel.SetActive(true);
            
            gameObject.GetComponent<CommendationsManager>().OnVictory();
        }

        public void RestartGame()
        {
            _commendationsManager.ResetCommendations();
            
            ClearLevelAndStartNew();
        }

        public void ClearLevelAndStartNew()
        {
            _levelManager.ClearCreatedInstances();
        
            CreateLevel();
        }

        private void ResumeAfterVictory()
        {
            isRunning = true;
            victoryPanel.SetActive(false);
            continueText.gameObject.SetActive(false);

            interactionManager.allowInteractions = true;
        }

        public void TogglePause(bool doPause)
        {
            interactionManager.allowInteractions = !doPause;
        }

        private void Update()
        {
            // Create level after in the first Update frame (after all game objects had started)
            if (_isFirstUpdate)
            {
                CreateLevel();
                _isFirstUpdate = false;
                _isSecondUpdate = true;
            } else if (_isSecondUpdate)
            {
                loadingText.SetActive(false);
            }

            if (!isRunning && Time.time - _victoryTime >= victoryWaitTime)
            {
                if (!continueText.gameObject.activeSelf)
                {
                    continueText.gameObject.SetActive(true);
                }

                if (Input.GetButtonDown("Fire1"))
                {
                    ClearLevelAndStartNew();
                    ResumeAfterVictory();
                }
            }
        }
    }
}
