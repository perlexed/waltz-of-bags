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
    public InteractionManager interactionManager;

    public float victoryWaitTime = 1f;
    private float _victoryTime;

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

    private void Reset()
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

    private void Update()
    {
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
