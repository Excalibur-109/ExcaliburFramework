using System.Collections.Generic;
using Excalibur;
using UnityEngine;
using UnityEngine.UIElements;

public class UnitAttribute : DirtyBase
{
    public enum AttriSource
    {
        Level,
        Equip,
        Point,
        Count,
    }

    LocalizableUnit owner;
    Dictionary<AttriType, int>[] attriSourceDic;
    Dictionary<AttriType, int> attriResultDic;

    public UnitAttribute(LocalizableUnit owner) : base()
    {
        this.owner = owner;
        attriSourceDic = new Dictionary<AttriType, int>[(int)AttriSource.Count];
        attriResultDic = new Dictionary<AttriType, int>();
    }

    public int GetAttriResult(AttriType attriType)
    {
        int ret;
        attriResultDic.TryGetValue(attriType, out ret);
        return ret;
    }

    public int ModifyAttriResult(AttriType attriType, int variantValue)
    {
        int primary;
        attriResultDic.TryGetValue(attriType, out primary);
        int ret = primary + variantValue;
        switch (attriType)
        {
            case AttriType.HP:
                primary = Mathf.Clamp(ret, 0, GetAttriResult(AttriType.HPMax));
                break;
            case AttriType.MP:
                primary = Mathf.Clamp(ret, 0, GetAttriResult(AttriType.MPMax));
                break;
        }
        if (ret < 0)
        {
            ret = 0;
        }
        SetAttriResult(attriType, ret);
        return primary;
    }

    public void SetAttriResult(AttriType attriType, int result)
    {
        if (attriResultDic.ContainsKey(attriType))
        {
            attriResultDic[attriType] = result;
        }
        else
        {
            attriResultDic.Add(attriType, result);
        }
    }

    protected override void Dirty()
    {
        CalculateAttri();
    }

    private void CalculateAttri()
    {
        Dictionary<AttriType, int> result = new Dictionary<AttriType, int>();
        for (int i = 0; i < attriSourceDic.Length; ++i)
        {
            Dictionary<AttriType, int> temp = attriSourceDic[i];
            foreach (var item in temp)
            {
                if (!result.ContainsKey(item.Key))
                {
                    result.Add(item.Key, item.Value);
                }
                else
                {
                    result[item.Key] += item.Value;
                }
            }
        }

        foreach (var item in result)
        {
            SetAttriResult(item.Key, item.Value);
        }
    }
}
