using System;
using System.Collections;
using System.Collections.Generic;
using Helpers;
using UnityEngine;

public class BagController : MonoBehaviour
{
    public bool isOnShelf;

    public Color defaultColor = new Color(1, 1, 1, 1);
    public Color canBePlacedColor = new Color(0.5f, 1, 0.5f, 1);
    public Color wrongPlaceColor = new Color(1, 0.5f, 0.5f, 1);
    
    public Collider2D[] bagAnchors;
    public Collider2D[] gridElements;
    public TimelineController timelineController;
    public InteractionManager interactionManager;
    
    private const string SortingLayerDraggedBag = "dragged_bag";
    private const string SortingLayerBags = "bags";

    private bool _canBePlaced = false;
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
            if (!interactionManager.IsCarryingBag)
            {
                StartDragging();
            }
        } else if (CanBePlaced())
        {
            StopDragging(true);
        }
        else
        {
            _spriteRenderer.color = wrongPlaceColor;
            StartCoroutine(FlashBagOnWrongPlaceDrop());
        }
    }

    private void StartDragging()
    {
        if (isOnShelf)
        {
            SetShelfSpaceFree();
        }
        
        isOnShelf = false;
        
        SetReadinessToBePlaced(CanBePlaced());

        interactionManager.IsCarryingBag = true;
        _isDragging = true;
        _spriteRenderer.sortingLayerID = SortingLayer.NameToID(SortingLayerDraggedBag);

        foreach (GridElementController gridElement in _matchedGridElements)
        {
            gridElement.isOccupied = false;
        }
        _matchedGridElements = new List<GridElementController>();

        SpriteHelper.SetAlpha(_spriteRenderer.color, 0.5f);
    }

    private void SetShelfSpaceFree()
    {
        foreach (GridElementController gridElement in _matchedGridElements)
        {
            gridElement.isOccupied = false;
        }

    }

    private void StopDragging(bool isPlacedOnShelf)
    {
        _isDragging = false;
        _spriteRenderer.sortingLayerID = SortingLayer.NameToID(SortingLayerBags);

        SetReadinessToBePlaced(false);

        interactionManager.IsCarryingBag = false;

        SpriteHelper.SetAlpha(_spriteRenderer.color, 1);

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
        SetShelfSpaceFree();
        
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
        if (!_isDragging)
        {
            return;
        }
        
        UpdateBagReadinessToBePlaced();
        
        if (interactionManager.allowInteractions && Input.GetMouseButtonUp(1))
        {
            StopDragging(false);
        }
        // Move the bag with the cursor
        else
        {
            Vector2 cursorPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector2(cursorPosition.x, cursorPosition.y);
        }
    }

    private void UpdateBagReadinessToBePlaced()
    {
        bool newReadinessToBePlacedFlag = CanBePlaced(); 
        if (_canBePlaced == newReadinessToBePlacedFlag)
        {
            return;
        }

        _canBePlaced = newReadinessToBePlacedFlag;
        SetReadinessToBePlaced(_canBePlaced);
    }

    private void SetReadinessToBePlaced(bool canBePlaced)
    {
        _spriteRenderer.color = canBePlaced ? canBePlacedColor : defaultColor;
    }

    private IEnumerator FlashBagOnWrongPlaceDrop()
    {
        yield return new WaitForSeconds(1);
        
        if (_spriteRenderer.color == wrongPlaceColor)
        {
            _spriteRenderer.color = defaultColor;
        }
    }
}
