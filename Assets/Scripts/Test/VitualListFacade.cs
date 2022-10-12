
using System.Collections.Generic;
using UnityEngine;

public class VirtualListFacade : MonoBehaviour
{
    public VirtualList uiList;
    // Use this for initialization
    void Start()
    {
        uiList.onSelectedEvent = OnSelectedEventHandler;
        List<int> listData = new List<int>();
        for (int i = 0; i < 1000; i++)
        {
            listData.Add(i);
        }
        uiList.Data(listData);
    }

    private void OnSelectedEventHandler(ListItem item)
    {
        Debug.LogError("选择的单元数据为:" + item.GetData().ToString());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            List<int> listData = new List<int>();
            int count = Random.Range(100, 1000);
            for (int i = 0; i < count; i++)
            {
                listData.Add(i);
            }
            uiList.Data(listData);
        }
    }
}