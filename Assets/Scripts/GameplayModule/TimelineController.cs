using Enums;
using UnityEngine;
using UnityEngine.UI;

namespace GameplayModule
{
    public class TimelineController : MonoBehaviour
    {
    
        public bool isRunning = true;
        public Text victoryText;
        public Text continueText;
        public Text quitText;
        public InteractionManager interactionManager;
        public DifficultyEnum difficulty;

        public float victoryWaitTime = 1f;
        private float _victoryTime;
        private bool _isInQuitConfirm;
        private BagController[] _bags;

        private LevelManager _levelManager;

        private bool _isFirstUpdate = true;
        private bool _shouldRefreshBagsOnUpdate = false;

        private void Start()
        {
            _levelManager = gameObject.GetComponent<LevelManager>();
        }

        private void CreateLevel()
        {
            _levelManager.CreateRandomLevel(difficulty);

            _shouldRefreshBagsOnUpdate = true;
        }

        public void InitBags(BagController[] bags)
        {
            _bags = bags;
        }

        public void RefreshBagsState()
        {
            bool areBagsOnShelf = true;

            foreach (BagController bagController in _bags)
            {
                if (!bagController.isOnShelf)
                {
                    areBagsOnShelf = false;
                }
            }

            if (areBagsOnShelf)
            {
                SetVictory();
            }
        }

        private void ClearCreatedInstances()
        {
            foreach (BagController bag in _bags)
            {
                Destroy(bag.gameObject);
            }

            foreach (GameObject generatedGridElement in _levelManager.generatedGridElements)
            {
                Destroy(generatedGridElement);
            }
        }

        private void SetVictory()
        {
            gameObject.GetComponent<AudioSource>().Play();
        
            isRunning = false;
            _victoryTime = Time.time;
            interactionManager.allowInteractions = false;

            victoryText.gameObject.SetActive(true);
            
            gameObject.GetComponent<VictoriesCountManager>().OnVictory();
        }

        public void ReturnBagsOnCart()
        {
            foreach (BagController bagController in _bags)
            {
                bagController.PlaceOnCart();
            }
        }

        public void ClearLevelAndStartNew()
        {
            ClearCreatedInstances();
        
            CreateLevel();
        }

        private void ResumeAfterVictory()
        {
            isRunning = true;
            victoryText.gameObject.SetActive(false);
            continueText.gameObject.SetActive(false);

            interactionManager.allowInteractions = true;
        }

        void OnGUI()
        {
            Event e = Event.current;
            if (e.isKey && e.keyCode != KeyCode.Escape)
            {
                CancelQuitCheck();
            }
        }

        private void CancelQuitCheck()
        {
            quitText.gameObject.SetActive(false);
            _isInQuitConfirm = false;
            interactionManager.allowInteractions = true;
        }
    
        private void QuitCheck()
        {
            if (_isInQuitConfirm)
            {
                Application.Quit();
            }
            else
            {
                _isInQuitConfirm = true;
                quitText.gameObject.SetActive(true);
                interactionManager.allowInteractions = false;
            }
        }

        private void Update()
        {
            // Create level after in the first Update frame (after all game objects had started)
            if (_isFirstUpdate)
            {
                CreateLevel();
                _isFirstUpdate = false;
            }

            if (_shouldRefreshBagsOnUpdate)
            {
                foreach (BagController bag in _bags)
                {
                    bag.RefreshGridElements();
                }
            }
        
            if (Input.GetButtonUp("Cancel"))
            {
                QuitCheck();
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
