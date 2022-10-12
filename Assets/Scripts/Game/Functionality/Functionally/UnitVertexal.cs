using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Excalibur;
using UnityEngine.UI;
using TMPro;

public class UnitVertexal : DirtyNode
{
    LocalizableUnit unit;
    Transform mainCamera;
    CanvasGroup canvasGroup;
    Image hpFill;
    Image mpFill;
    TextMeshProUGUI nameText;

    protected override void Awake()
    {
        mainCamera = Camera.main.transform;
        canvasGroup = GetComponent<CanvasGroup>();
        hpFill = transform.GetChildComponentRecursively<Image>("HP");
        mpFill = transform.GetChildComponentRecursively<Image>("MP");
        nameText = transform.GetChildComponentRecursively<TextMeshProUGUI>("Name");
    }

    protected override void Update()
    {
        transform.LookAt(mainCamera);
    }

    public void OnInit(LocalizableUnit unit)
    {
        this.unit = unit;
        SetDirty();
    }

    protected override void Dirty()
    {

    }
}
