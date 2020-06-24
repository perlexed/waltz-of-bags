using System;
using System.Collections;
using System.Collections.Generic;
using GameplayModule;
using Helpers;
using UnityEngine;

public class BagController : MonoBehaviour
{
    public bool isOnShelf;
    public bool isOnCart;
    public bool canBeRotated;

    public Color defaultColor = new Color(1, 1, 1, 1);
    public Color canBePlacedColor = new Color(0.5f, 1, 0.5f, 1);
    public Color wrongPlaceColor = new Color(1, 0.5f, 0.5f, 1);
    
    public Collider2D[] bagAnchors;
    public GameObject shelfGrid;
    public TimelineController timelineController;
    public InteractionManager interactionManager;

    public Material outlineMaterial;
    public Material normalMaterial;

    public AudioClip pickupSound;
    public AudioClip dropOnCartSound;
    public AudioClip dropOnShelfSound;
    
    private const string SortingLayerDraggedBag = "dragged_bag";
    private const string SortingLayerBags = "bags";

    private bool _isRotated;
    private bool _canBePlaced;
    private bool _isDragging;
    private Vector2 _startingPosition;

    private Vector3 _nearestGridOffset;

    private SpriteRenderer _spriteRenderer;
    private Camera _camera;
    private AudioSource _audioSource;
    private Collider2D[] _gridElements;

    private List<GridElementController> _matchedGridElements = new List<GridElementController>();

    public delegate void OnBagPickupStatusChange();
    public event OnBagPickupStatusChange OnBagPickupStatusChangeEvent;

    void Start()
    {
        _camera = Camera.main;
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _startingPosition = transform.position;
        _audioSource = gameObject.GetComponent<AudioSource>();
    }

    public void RefreshGridElements()
    {
        _gridElements = shelfGrid.GetComponentsInChildren<Collider2D>();
    }

    private bool CanBePlaced()
    {
        bool canBagBePlaced = true;
        _matchedGridElements = new List<GridElementController>();

        bool isOffsetFound = false;
        Vector2 nearestGridOffset = new Vector2();

        foreach (Collider2D bagAnchor in bagAnchors)
        {
            bool canAnchorBePlaced = false;

            foreach (Collider2D gridElementCollider in _gridElements)
            {
                GridElementController gridElement = gridElementCollider.GetComponent<GridElementController>();
                
                if (!gridElement.isOccupied && bagAnchor.IsTouching(gridElementCollider))
                {
                    if (!isOffsetFound)
                    {
                        isOffsetFound = true;
                        nearestGridOffset = gridElementCollider.transform.position - bagAnchor.transform.position;
                    }
                    
                    canAnchorBePlaced = true;
                    _matchedGridElements.Add(gridElement);
                }
            }
            
            canBagBePlaced = canBagBePlaced && canAnchorBePlaced;
        }

        if (canBagBePlaced)
        {
            _nearestGridOffset = nearestGridOffset;
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

    public void OnMouseOver()
    {
        _spriteRenderer.material = outlineMaterial;
    }

    public void OnMouseExit()
    {
        _spriteRenderer.material = normalMaterial;
    }

    private void StartDragging()
    {
        _audioSource.clip = pickupSound;
        _audioSource.Play();

        if (isOnShelf)
        {
            SetShelfSpaceFree();
        }
        
        isOnShelf = false;
        isOnCart = false;
        
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

        OnBagPickupStatusChangeEvent?.Invoke();
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
            _audioSource.clip = dropOnShelfSound;
            _audioSource.Play();
            PlaceOfShelf();
        }
        else
        {
            _audioSource.clip = dropOnCartSound;
            _audioSource.Play();
            PlaceOnCart();
        }
    }

    public void PlaceOnCart()
    {
        SetShelfSpaceFree();
        
        transform.position = _startingPosition;
        isOnShelf = false;
        isOnCart = true;
        
        OnBagPickupStatusChangeEvent?.Invoke();
    }

    void PlaceOfShelf()
    {
        isOnShelf = true;
        isOnCart = false;

        foreach (GridElementController gridElement in _matchedGridElements)
        {
            gridElement.isOccupied = true;
        }

        // Position bag on grid
        transform.position += _nearestGridOffset;
        
        OnBagPickupStatusChangeEvent?.Invoke();
    }

    void Update()
    {
        if (!_isDragging)
        {
            return;
        }
        
        UpdateBagReadinessToBePlaced();
        
        if (interactionManager.allowInteractions)
        {
            if (Input.GetButtonUp("Jump"))
            {
                Rotate();
            }

            if (Input.GetMouseButtonUp(1))
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

    private void Rotate()
    {
        if (!canBeRotated)
        {
            return;
        }
        
        transform.Rotate(0, 0, _isRotated ? -90 : 90);
        _isRotated = !_isRotated;
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
