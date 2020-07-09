using System.Linq;
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

        public float victoryWaitTime = 1f;
        private float _victoryTime;
        private bool _isInQuitConfirm;
        private BagController[] _bags;

        private LevelManager _levelManager;

        private bool _isFirstUpdate = true;
        private bool _shouldRefreshBagsOnUpdate;

        private bool AreAllBagsOnCart => _bags
            .ToList()
            .Aggregate(true, (areAllBagsOnCart, bag) => areAllBagsOnCart && bag.isOnCart);
        
        private bool AreAllBagsOnShelf => _bags
            .ToList()
            .Aggregate(true, (areAllBagsOnCart, bag) => areAllBagsOnCart && bag.isOnShelf);

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
            
            foreach (var bag in _bags)
            {
                bag.OnBagPickupStatusChangeEvent += OnBagPickupStatusChange;
            }
        }

        private void OnBagPickupStatusChange()
        {
            if (AreAllBagsOnShelf)
            {
                SetVictory();
            }
            
            ResetBagsButton.interactable = !AreAllBagsOnCart;
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

            victoryPanel.SetActive(true);
            
            gameObject.GetComponent<CommendationsManager>().OnVictory();
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
            }

            if (_shouldRefreshBagsOnUpdate)
            {
                foreach (BagController bag in _bags)
                {
                    bag.RefreshGridElements();
                }
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
