using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagController : MonoBehaviour
{
    public bool canBeRotated = false;
    public bool isOnShelf = false;

    public Collider2D[] bagAnchors;
    public Collider2D[] gridElements;
    public TimelineController timelineController;
    public InteractionManager interactionManager;
    
    private const string SortingLayerDraggedBag = "dragged_bag";
    private const string SortingLayerBags = "bags";

    private bool _isDragging = false;
    private Vector2 _startingPosition;

    private SpriteRenderer _spriteRenderer;
    private Camera _camera;

    private List<GridElementController> _matchedGridElements = new List<GridElementController>();

    void Start()
    {
        _camera = Camera.main;
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _startingPosition = transform.position;
    }

    private bool CanBePlaced()
    {
        bool canBagBePlaced = true;
        _matchedGridElements = new List<GridElementController>();

        foreach (Collider2D bagAnchor in bagAnchors)
        {
            bool canAnchorBePlaced = false;

            foreach (Collider2D gridElementCollider in gridElements)
            {
                GridElementController gridElement = gridElementCollider.GetComponent<GridElementController>();
                
                if (!gridElement.isOccupied && bagAnchor.IsTouching(gridElementCollider))
                {
                    canAnchorBePlaced = true;
                    _matchedGridElements.Add(gridElement);
                }
            }
            canBagBePlaced = canBagBePlaced && canAnchorBePlaced;
        }
        
        return canBagBePlaced;
    }

    public void OnMouseDown()
    {
        if (!interactionManager.allowInteractions)
        {
            return;
        }
        
        if (!_isDragging)
        {
            if (!interactionManager.isCarryingBag)
            {
                StartDragging();
            }
        } else if (CanBePlaced())
        {
            StopDragging(true);
        }
        else
        {
            // @todo flash bag with red
            Debug.Log("bag can't be placed");
        }
    }

    private void StartDragging()
    {
        isOnShelf = false;
        
        interactionManager.isCarryingBag = true;
        _isDragging = true;
        _spriteRenderer.sortingLayerID = SortingLayer.NameToID(SortingLayerDraggedBag);

        foreach (GridElementController gridElement in _matchedGridElements)
        {
            gridElement.isOccupied = false;
        }
        _matchedGridElements = new List<GridElementController>();

        SetTransparency(0.5f);
    }

    private void StopDragging(bool isPlacedOnShelf)
    {
        _isDragging = false;
        _spriteRenderer.sortingLayerID = SortingLayer.NameToID(SortingLayerBags);

        interactionManager.isCarryingBag = false;

        SetTransparency(1f);

        if (isPlacedOnShelf)
        {
            PlaceOfShelf();
        }
        else
        {
            PlaceOnCart();
        }
    }

    public void PlaceOnCart()
    {
        transform.position = _startingPosition;
        isOnShelf = false;
    }

    void PlaceOfShelf()
    {
        isOnShelf = true;
        timelineController.RefreshBagsState();

        foreach (GridElementController gridElement in _matchedGridElements)
        {
            gridElement.isOccupied = true;
        }
    }

    void Update()
    {
        if (_isDragging)
        {
            if (interactionManager.allowInteractions && Input.GetMouseButtonUp(1))
            {
                StopDragging(false);
            }
            else
            {
                Vector2 cursorPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
                transform.position = new Vector2(cursorPosition.x, cursorPosition.y);
            }
        }
    }

    private void SetTransparency(float transparency)
    {
        Color semiTransparentColor = _spriteRenderer.color;
        semiTransparentColor.a = transparency;
        _spriteRenderer.color = semiTransparentColor;
    }
}
