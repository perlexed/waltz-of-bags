﻿using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using UnityEngine;
using UnityEngine.UI;

namespace GameplayModule
{
    public class DifficultyManager : MonoBehaviour
    {
        public GameObject normalDifficultyButton;
        public GameObject hardDifficultyButton;

        private TimelineController _timelineController;
        private CommendationsManager _commendationsManager;

        private void Start()
        {
            _timelineController = gameObject.GetComponent<TimelineController>();
            _commendationsManager = gameObject.GetComponent<CommendationsManager>();
            
            normalDifficultyButton.SetActive(_timelineController.difficulty == DifficultyEnum.Hard);
            hardDifficultyButton.SetActive(_timelineController.difficulty == DifficultyEnum.Normal);
        }

        public void OnHardDifficultyClick()
        {
            ApplyDifficultyChange(DifficultyEnum.Hard);
        }

        public void OnNormalDifficultyClick()
        {
            ApplyDifficultyChange(DifficultyEnum.Normal);
        }

        private void ApplyDifficultyChange(DifficultyEnum difficulty)
        {
            if (_timelineController.difficulty == difficulty)
            {
                return;
            }
            
            normalDifficultyButton.SetActive(difficulty == DifficultyEnum.Hard);
            hardDifficultyButton.SetActive(difficulty == DifficultyEnum.Normal);
            _timelineController.difficulty = difficulty;
            _timelineController.ClearLevelAndStartNew();
            _commendationsManager.ResetCommendations();
        }
    }
}
