using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Excalibur
{
    public sealed class FormManager : MonoSingleton<FormManager>
    {
        Transform _visibleUINode;
        Transform _residentUINode;
        Transform _visibleTipsUINode;
        Transform _residentTipsUINode;

        Dictionary<FormType, string> _formsDic;

        Dictionary<FormType, IForm> _residentForms;
        Dictionary<FormType, IForm> _dynamicForms;

        protected override void Awake()
        {
            base.Awake();

            Utility.SetParent(Utility.LoadAsset<GameObject>(AssetType.EventSystem, "EventSystem").transform, this.transform);

            Canvas canvas;
            CanvasScaler canvasScaler;
            GraphicRaycaster graphicRaycaster;

            canvas = transform.GetComponent<Canvas>();
            if (canvas == null)
                canvas = gameObject.AddComponent<Canvas>();

            canvasScaler = transform.GetComponent<CanvasScaler>();
            if (canvasScaler == null)
                canvasScaler = gameObject.AddComponent<CanvasScaler>();

            graphicRaycaster = transform.GetComponent<GraphicRaycaster>();
            if (graphicRaycaster == null)
                graphicRaycaster = gameObject.AddComponent<GraphicRaycaster>();

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.enabled = true;
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = Utility.ScreenVector();
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.matchWidthOrHeight = 0.5f;
            canvasScaler.enabled = true;
            graphicRaycaster.ignoreReversedGraphics = true;
            graphicRaycaster.enabled = true;

            _residentUINode = CreateUINode("UINode_Resident", 1);
            _residentTipsUINode = CreateUINode("UINode_ResidentTip", 2);
            _visibleUINode = CreateUINode("UINode_Dynamic", 3);
            _visibleTipsUINode = CreateUINode("UINode_DynamicTip", 4);

            _residentForms = new Dictionary<FormType, IForm>();
            _dynamicForms = new Dictionary<FormType, IForm>();

            RegistForm(FormType.FT_NONE);
        }

        protected override void Start()
        {
            base.Start();
        }

        private Transform CreateUINode(string name, int layer)
        {
            GameObject go = Utility.CreateGameObject(name, false, this.transform);
            go.layer = layer;
            Transform trans = go.transform;
            RectTransform rect = trans as RectTransform;
            rect.anchorMin = Vector3.zero;
            rect.anchorMax = Vector3.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            return trans;
        }

        private void RegistForm(FormType formType)
        {
            if (_formsDic == null)
                _formsDic = new Dictionary<FormType, string>();

            string formName = string.Empty;
            switch (formType)
            {
                case FormType.FT_NONE:
                    Array formTypes = Enum.GetValues(typeof(FormType));
                    foreach (var item in formTypes)
                    {
                        FormType type = (FormType)item;
                        if (type == FormType.FT_NONE)
                            continue;
                        RegistForm(type);
                    }
                    break;
                case FormType.FT_EXCAMPLE:
                    formName = "ExcampleForm";
                    break;
                case FormType.FT_TIP:
                    formName = "TipForm";
                    break;
                case FormType.FT_LOADING:
                    formName = "LoadingForm";
                    break;
            }

            if (Utility.StrAvalible(formName) && !_formsDic.ContainsKey(formType))
            {
                _formsDic.Add(formType, formName);
            }
        }

        public IForm CreateForm(FormType formType)
        {
            Form form = ExitsForm(formType) as Form;
            if (form != null)
                return form;

            form = Utility.LoadAsset<Form>(AssetType.AT_FORM, _formsDic[formType]);
            if (form != null)
            {
                switch (form.DisplayType)
                {
                    case DisplayeType.DT_RESIDENT:
                        if (formType == FormType.FT_TIP)
                        {
                            form.transform.SetParent(_residentTipsUINode);
                        }
                        else
                        {
                            form.transform.SetParent(_residentUINode);
                        }
                        _residentForms.Add(formType, form);
                        break;
                    case DisplayeType.DT_DYNAMIC:
                        if (formType == FormType.FT_LOADING)
                        {
                            form.transform.SetParent(_visibleTipsUINode);
                        }
                        else
                        {
                            form.transform.SetParent(_visibleUINode);
                        }
                        _dynamicForms.Add(formType, form);
                        break;
                }

                form.OnInitialzed(formType);
            }

            return form;
        }

        public IForm FindForm(FormType formType)
        {
            IForm form = FindResidentForm(formType);
            if (form == null)
                form = FindDynamicForm(formType);

            if (form == null)
                form = CreateForm(formType);

            return form;
        }

        public IForm ExitsForm(FormType formType)
        {
            IForm form = FindResidentForm(formType);
            if (form == null)
                form = FindDynamicForm(formType);
            
            return form;
        }

        public IForm FindResidentForm(FormType formType)
        {
            _residentForms.TryGetValue(formType, out IForm form);
            return form;
        }

        public IForm FindDynamicForm(FormType formType)
        {
            _dynamicForms.TryGetValue(formType, out IForm form);
            return form;
        }

        public void ActivateFormInOut(FormType formType)
        {
            IForm form = FindForm(formType);
            if (form != null)
            {
                form.ActivteInOut();
            }
        }

        public void ForceActivateFormInOut(FormType formType)
        {
            IForm form = FindForm(formType);
            if (form != null)
            {
                form.ForceActivateInOut();
            }
        }

        public void ForceActivateFormsInOut(bool active)
        {
            if (active)
            {
                foreach (var form in _residentForms.Values)
                {
                    if (!form.IsActive)
                    {
                        form.ForceActivateInOut();
                    }
                }
            }
            else
            {
                foreach (var form in _residentForms.Values)
                {
                    if (form.IsActive)
                    {
                        form.ForceActivateInOut();
                    }
                }
                foreach (var form in _residentForms.Values)
                {
                    if (form.IsActive)
                    {
                        form.ForceActivateInOut();
                    }
                }
            }
        }

        public void OpenForm(FormType formType)
        {
            IForm form = FindForm(formType);

            if (form != null)
            {
                form.Open();
            }
        }

        public void CloseForm(FormType formType)
        {
            IForm form = FindForm(formType);

            if (form != null)
            {
                form.Close();
            }
        }
    }
}
