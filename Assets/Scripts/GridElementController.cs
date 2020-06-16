using System;
using System.Collections;
using System.Collections.Generic;
using GameplayModule;
using Helpers;
using UnityEngine;
using UnityEngine.Serialization;

public class GridElementController : MonoBehaviour
{
    public bool isOccupied = false;

    public InteractionManager interactionManager;

    private void OnDestroy()
    {
        interactionManager.OnCarryingStatusChangeEvent -= OnIsDraggingStatusChange;
    }

    public void Start()
    {
        interactionManager.OnCarryingStatusChangeEvent += OnIsDraggingStatusChange;
    }

    private void OnIsDraggingStatusChange(bool newValue)
    {
        SetHighlightedMode(newValue);
    }

    private void SetHighlightedMode(bool isHighlighted)
    {
        SpriteRenderer spriteRenderer = transform.gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.color = SpriteHelper.SetAlpha(spriteRenderer.color, isHighlighted ? 1f : 0.4f);
    }
}
