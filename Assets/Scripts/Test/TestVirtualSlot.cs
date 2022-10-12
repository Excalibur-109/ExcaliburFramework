using Excalibur;
using UnityEngine;
using TMPro;

public class TestSlotData : IItemData
{
    public int id;
    public TestSlotData(int id)
    {
        this.id = id;
    }
}

public class TestVirtualSlot : VirtualSlot
{
    TextMeshProUGUI text;
    TestSlotData data;

    protected override void Awake()
    {
        base.Awake();
        text = GetComponentInChildren<TextMeshProUGUI>();
        int r = Random.Range(0, 256);
        int g = Random.Range(0, 256);
        int b = Random.Range(0, 256);
        background.color = Utility.GetColor(r, g, b);
    }

    protected override void OnRefresh()
    {
        data = GetItemData<TestSlotData>();
        if (text != null)
            text.text = data.id.ToString();
    }
}
