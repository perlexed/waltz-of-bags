using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using Services;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TimelineController : MonoBehaviour
{
    
    public bool isRunning = true;
    public Text victoryText;
    public Text continueText;
    public Text quitText;
    public InteractionManager interactionManager;
    public DifficultyEnum defaultDifficulty;

    public float victoryWaitTime = 1f;
    private float _victoryTime;
    private bool _isInQuitConfirm;
    private BagController[] _bags;

    private bool _isFirstUpdate = true;

    private void OnFirstUpdate()
    {
        GetComponent<LevelManager>().CreateRandomLevel(defaultDifficulty);
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

    private void SetVictory()
    {
        gameObject.GetComponent<AudioSource>().Play();
        
        isRunning = false;
        _victoryTime = Time.time;
        interactionManager.allowInteractions = false;

        victoryText.gameObject.SetActive(true);
    }

    public void Reset()
    {
        foreach (BagController bagController in _bags)
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
        if (_isFirstUpdate)
        {
            OnFirstUpdate();
            _isFirstUpdate = false;
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
                Reset();
            }
        }
    }
}
