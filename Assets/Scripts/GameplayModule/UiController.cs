using UnityEngine;

namespace GameplayModule
{
    public class UiController : MonoBehaviour
    {
        public void OnSoundToggle(bool isSoundOn)
        {
            AudioListener.volume = isSoundOn ? 1f : 0;
        }

        public void OnExitButton()
        {
            Application.Quit();
        }
    }
}
