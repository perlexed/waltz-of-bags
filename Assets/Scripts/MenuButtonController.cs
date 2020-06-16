using System;
using System.Collections;
using System.Collections.Generic;
using GameplayModule;
using UnityEngine;
using UnityEngine.UI;

public class MenuButtonController : MonoBehaviour
{
    public enum ButtonTypeEnum
    {
        Exit,
        ResetBags
    }

    public ButtonTypeEnum buttonType;
    public TimelineController timelineController;
    
    private void OnMouseUp()
    {
        switch (buttonType)
        {
            case ButtonTypeEnum.Exit:
                Application.Quit();
                break;
            case ButtonTypeEnum.ResetBags:
                timelineController.ReturnBagsOnCart();
                break;
        }
    }
}
