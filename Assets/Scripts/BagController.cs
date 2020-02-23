using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagController : MonoBehaviour
{
    private bool _isDragging = false;
    public bool canBeRotated;

    private bool CanBeDropped()
    {
        return true;
    }

    public void OnMouseDown()
    {
        if (!_isDragging)
        {
            _isDragging = true;
            return;
        }

        if (CanBeDropped())
        {
            _isDragging = false;
            PlaceOfShelf();
        }
    }

    void PlaceOfShelf()
    {
        Debug.Log("placed");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_isDragging)
        {
            Vector2 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            transform.position = new Vector2(cursorPosition.x, cursorPosition.y);

        }
    }
}
