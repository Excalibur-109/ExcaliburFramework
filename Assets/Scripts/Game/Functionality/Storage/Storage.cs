using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Excalibur;
using UnityEngine.UI;
using TMPro;

public enum ItemBelong
{
    Storage,
    Equip,
    Shotcut,
}

public class Storage
{
    private const int MAX_STORAGE_COUNT = 44;
    private const int MAX_EQUIP_COUNT = 2;
    private const int MAX_SHOTCUT_COUNT = 9;

    private StorageItem[] m_ItemStore = new StorageItem[MAX_STORAGE_COUNT];
    private StorageItem[] m_EquipStore = new StorageItem[MAX_EQUIP_COUNT];
    private StorageItem[] m_ShotcutStore = new StorageItem[MAX_SHOTCUT_COUNT];
}
