using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintIconController : MonoBehaviour
{
    public GameObject hintsPanel;
    
    public void OnMouseEnter()
    {
        hintsPanel.SetActive(true);
    }

    public void OnMouseExit()
    {
        hintsPanel.SetActive(false);
    }
}
