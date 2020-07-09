using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameplayModule
{
    public class CommendationsManager : MonoBehaviour
    {
        public GameObject MedalsContainer;
        public GameObject CupPrefab;
        public GameObject GoldMedalPrefab;
        public GameObject SilverMedalPrefab;
        public GameObject BronzeMedalPrefab;
        
        private const int LevelsCountForMedal = 3;
        private const float MedalDistanceOffset = 0.6f;
        
        private int _completedLevelsCount;
        private Transform _medalsContainerTransform;
        private List<GameObject> _commendationsInstances = new List<GameObject>();

        private struct MedalsCount
        {
            public int CupLevel;
            public int GoldCount;
            public int SilverCount;
            public int BronzeCount;

            public override string ToString()
            {
                return $"Cup: {CupLevel}, gold: {GoldCount}, silver: {SilverCount}, bronze: {BronzeCount}";
            }
        }

        public void Start()
        {
            _medalsContainerTransform = MedalsContainer.transform;
        }

        public void OnVictory()
        {
            _completedLevelsCount++;
            UpdateCommendations();
        }

        public void ResetCommendations()
        {
            _completedLevelsCount = 0;
            UpdateCommendations();
        }

        private void UpdateCommendations()
        {
            MedalsCount medalsCount = GetMedalsCountForLevels(_completedLevelsCount);

            foreach (GameObject commendation in _commendationsInstances)
            {
                Destroy(commendation);
            }
            CreateMedals(medalsCount);
        }

        private void CreateMedals(MedalsCount medals)
        {
            _commendationsInstances = new List<GameObject>();

            UpdateCupCommendation(medals.CupLevel);            
            CreateMedalsByType(GoldMedalPrefab, medals.GoldCount);
            CreateMedalsByType(SilverMedalPrefab, medals.SilverCount);
            CreateMedalsByType(BronzeMedalPrefab, medals.BronzeCount);
        }

        private void UpdateCupCommendation(int cupLevel)
        {
            if (cupLevel == 0)
            {
                return;
            }

            GameObject cupInstance = Instantiate(
                CupPrefab,
                _medalsContainerTransform.position,
                _medalsContainerTransform.rotation,
                _medalsContainerTransform
            );
            
            _commendationsInstances.Add(cupInstance);

            cupInstance.gameObject.GetComponentInChildren<Text>().text = cupLevel.ToString();
        }

        private void CreateMedalsByType(GameObject medalPrefab, int medalsCount)
        {
            Quaternion medalRotation = _medalsContainerTransform.rotation;
            
            for (int medalIndex = 0; medalIndex < medalsCount; medalIndex++)
            {
                Vector3 medalPosition = new Vector3(
                    x: _medalsContainerTransform.position.x - MedalDistanceOffset * _commendationsInstances.Count,
                    y: _medalsContainerTransform.position.y
                );
                
                GameObject medalInstance = Instantiate(
                    medalPrefab,
                    medalPosition,
                    medalRotation,
                    _medalsContainerTransform
                );
                
                _commendationsInstances.Add(medalInstance);
            }
        }

        private static MedalsCount GetMedalsCountForLevels(int levelsCount)
        {
            int remainingLevelsCount = levelsCount;
            
            MedalsCount medalsCount = new MedalsCount();

            int cupsDelimiter = (int) Math.Pow(LevelsCountForMedal, 3);
            medalsCount.CupLevel = (int) Math.Floor((decimal) remainingLevelsCount / cupsDelimiter);
            remainingLevelsCount -= medalsCount.CupLevel * cupsDelimiter;

            int goldMedalsDelimiter = (int) Math.Pow(LevelsCountForMedal, 2);
            medalsCount.GoldCount = (int) Math.Floor((decimal) remainingLevelsCount / goldMedalsDelimiter);
            remainingLevelsCount -= medalsCount.GoldCount * goldMedalsDelimiter;

            medalsCount.SilverCount = (int) Math.Floor((decimal) remainingLevelsCount / LevelsCountForMedal);
            remainingLevelsCount -= medalsCount.SilverCount * LevelsCountForMedal;

            medalsCount.BronzeCount = remainingLevelsCount;
            
            return medalsCount;
        }
    }
}
