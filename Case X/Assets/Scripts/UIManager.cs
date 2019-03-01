﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public bool _SettingsUIDisplay = false;
    public bool _DiaryUIDisplay = false;
    public bool _InventoryDisplay = false;

    public GameObject SettingsDisplay;
    public GameObject DiaryDisplay;
    public GameObject InventoryDisplay;

    public void ToggleSettingsUI()
    {
        if (_DiaryUIDisplay)
        {
            DiaryDisplay.SetActive(false);
            _DiaryUIDisplay = !_DiaryUIDisplay;
        }
        _SettingsUIDisplay = !_SettingsUIDisplay;
        if (_SettingsUIDisplay)
        {
            SettingsDisplay.SetActive(true);
        } else if(!_SettingsUIDisplay)
        {
            SettingsDisplay.SetActive(false);
        }
    }
    public void ToggleDiaryUI()
    {
        if (_SettingsUIDisplay)
        {
            SettingsDisplay.SetActive(false);
            _SettingsUIDisplay = !_SettingsUIDisplay;
        }
        _DiaryUIDisplay = !_DiaryUIDisplay;
        if (_DiaryUIDisplay)
        {
            DiaryDisplay.SetActive(true);
        }
        else if (!_DiaryUIDisplay)
        {
            DiaryDisplay.SetActive(false);
        }
    }
    public void ToggleInventoryUI()
    {
        _InventoryDisplay = !_InventoryDisplay;
        if (_InventoryDisplay)
        {
            InventoryDisplay.SetActive(true);
        }
        else if (!_InventoryDisplay)
        {
            InventoryDisplay.SetActive(false);
        }
    }
}