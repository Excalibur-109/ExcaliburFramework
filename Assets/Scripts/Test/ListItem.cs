using UnityEngine;
using UnityEngine.UI;

public class ListItem : MonoBehaviour
{

    [System.NonSerialized]
    public int itemIndex;
    [System.NonSerialized]
    public GameObject itemObject;
    private object data;
    public void UpdateItem(int index, GameObject item)
    {
        itemIndex = index;
        itemObject = item;
    }
    public virtual void Data(object data)
    {
        this.data = data;
        transform.Find("Text").GetComponent<Text>().text = "数据" + data.ToString();
    }
    public virtual object GetData()
    {
        return data;
    }
    public virtual void SetSelected(bool selected)
    {

    }
}