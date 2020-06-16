using UnityEngine;
using UnityEngine.UI;

namespace GameplayModule
{
    public class VictoriesCountManager : MonoBehaviour
    {
        private int _completedLevelsCount = 0;

        public Text completedLevelsText;

        public void OnVictory()
        {
            _completedLevelsCount++;

            completedLevelsText.text = $"Completed levels: {_completedLevelsCount}";
        }
    }
}
