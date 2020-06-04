using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TimelineController : MonoBehaviour
{
    public BagController[] bags;
    public bool isRunning = true;
    public Text victoryText;
    public Text continueText;
    public Text quitText;
    public InteractionManager interactionManager;

    public float victoryWaitTime = 1f;
    private float _victoryTime;
    private bool _isInQuitConfirm;

    public void RefreshBagsState()
    {
        bool areBagsOnShelf = true;

        foreach (BagController bagController in bags)
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

    private void SetVictory()
    {
        isRunning = false;
        _victoryTime = Time.time;
        interactionManager.allowInteractions = false;

        victoryText.gameObject.SetActive(true);
    }

    public void Reset()
    {
        foreach (BagController bagController in bags)
        {
            bagController.PlaceOnCart();
        }

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
                Reset();
            }
        }
    }
}
