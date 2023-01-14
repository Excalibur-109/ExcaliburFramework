using UnityEngine;
using DG.Tweening;
using System;

namespace Excalibur
{
    /// <summary>
    /// 所有的窗口都要来继承Form
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public abstract class Form : DirtyNode, IForm
    {
        [SerializeField] private DisplayeType displayType;
        private DisplayeType OriginalDisplayType { get; set; }

        public DisplayeType DisplayType { get { return displayType; } private set { } }

        private DisplaySubType displaySubType = DisplaySubType.DST_CLOSE;
        [SerializeField] protected InOutMoveType MoveType = InOutMoveType.DTM_NONE;
        [SerializeField] protected InOutRotateType RotateType = InOutRotateType.DTR_NONE;
        [SerializeField] protected InOutScaleType ScaleType = InOutScaleType.DTS_NONE;
        [SerializeField] protected InOutAlphaType AlphaType = InOutAlphaType.DTA_NONE;
        [SerializeField] protected BlockRaycastType BlockRaycastType = BlockRaycastType.BRT_BLOCK;
        private bool BlockRaycast { get { return BlockRaycastType == BlockRaycastType.BRT_BLOCK; } }
        [SerializeField] private Ease MoveEase = Ease.Linear;
        [SerializeField] private Ease RotateEase = Ease.Linear;
        private Vector2 _closePos = Vector2.zero;
        [SerializeField] private Vector2 _openPos = Vector2.zero;

        const float ROTATE_ANGLE = 75f;
        const float ANIM_TIME = 0.15f;
        const float JELLY_TIME = 0.06f;

        private FormType _formType;
        public FormType FormType { get { return _formType; } private set { } }

        private RectTransform _rect;
        private CanvasGroup _canvasGroup;

        /// <summary>
        /// 界面是否是可用状态
        /// </summary>
        public bool IsActive
        {
            get
            {
                bool active = false;
                switch (displayType)
                {
                    case DisplayeType.DT_RESIDENT:
                        {
                            active = true;
                        }
                        break;
                    case DisplayeType.DT_DYNAMIC:
                        {
                            switch (displaySubType)
                            {
                                case DisplaySubType.DST_OPEN:
                                    {
                                        active = true;
                                    }
                                    break;
                                case DisplaySubType.DST_CLOSE:
                                    {
                                        active = false;
                                    }
                                    break;
                            }
                        }
                        break;
                }
                return active;
            }

            private set { }
        }

        public void OnInitialzed(FormType formType)
        {
            this._formType = formType;

            OriginalDisplayType = displayType;

            displaySubType = DisplaySubType.DST_CLOSE;

            CalculateClosePos();

            SurveilEvent();

            _rect = GetComponent<RectTransform>();
            _rect.anchorMin = Vector2.zero;
            _rect.anchorMax = Vector2.one;
            _rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Utility.ScreenVector().x);
            _rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Utility.ScreenVector().y);

            _rect.pivot = Vector2.one * 0.5f;
            _rect.anchoredPosition = _closePos;

            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();

            ForceActivateInOut();

            if (formType == FormType.FT_TIP || formType == FormType.FT_LOADING)
            {
                BlockRaycastType = BlockRaycastType.BRT_NONE;
            }

            if (formType == FormType.FT_LOADING)
            {
                SetAsFirstSibling();
            }
        }

        public void Open()
        {
            if (!IsActive)
            {
                OnOpen();
                Activate(true);
            }
        }

        public void Close()
        {
            if (IsActive)
            {
                OnClose();
                Activate(false);
            }
        }

        protected abstract void OnOpen();

        protected abstract void OnClose();

        protected abstract void OnReset();

        protected override void Dirty()
        {
            OnReset();
        }

        /// <summary>
        /// 界面的开关调用这个
        /// </summary>
        public void ActivteInOut()
        {
            bool nextState = !IsActive;

            if (nextState)
            {
                if (_formType == FormType.FT_LOADING)
                {
                    _rect.SetAsFirstSibling();
                }
                OnOpen();
            }
            else
            {
                OnClose();
            }

            Activate(nextState);
        }

        /// <summary>
        /// 如果需要开关常驻界面，调用这个。动态界面也可以调用
        /// </summary>
        public void ForceActivateInOut()
        {
            SetDisplayType();

            ActivteInOut();

            if (OriginalDisplayType == DisplayeType.DT_RESIDENT)
            {
                displaySubType = IsActive ? DisplaySubType.DST_OPEN : DisplaySubType.DST_CLOSE;
                if (IsActive)
                {
                    displayType = OriginalDisplayType;
                }
            }
        }

        /// <summary>
        /// 显示关闭窗口，true显示，false隐藏
        /// </summary>
        private void Activate(bool active)
        {
            if (displayType == DisplayeType.DT_RESIDENT)
            {
                // 常驻页面不需要打开关闭
                return;
            }

            if (IsActive == active)
            {
                return;
            }

            displaySubType = active ? DisplaySubType.DST_OPEN : DisplaySubType.DST_CLOSE;

            if (active)
            {
                SetAsFirstSibling();
            }

            ActivateAnim();
        }

        /// <summary>
        /// 对应界面自定义动画，注意用 IsActive 属性判断界面开关状态
        /// </summary>
        protected virtual void ActivateAnim()
        {
            bool active = IsActive;

            if (active)
                Utility.SetActive(this.gameObject, true);

            _canvasGroup.blocksRaycasts = false;

            Vector2 screen = Utility.ScreenVector();

            Sequence sequence = DOTween.Sequence().SetAutoKill(false);

            if (MoveType != InOutMoveType.DTM_NONE)
            {
                CalculateClosePos();
                Vector3 endpos = active ? _openPos : _closePos;
                sequence.Append(_rect.DOLocalMove(endpos, ANIM_TIME).SetEase(MoveEase));
            }

            if (RotateType != InOutRotateType.DTR_NONE)
            {
                Vector3 rotate = RotateType == InOutRotateType.DTR_HORIZONTAL ? new Vector3(0f, ROTATE_ANGLE, 0f) : new Vector3(ROTATE_ANGLE, 0f, 0f);
                if (active)
                    rotate = Vector3.zero;
                sequence.Join(_rect.DORotate(rotate, ANIM_TIME)).SetEase(RotateEase);
            }

            if (ScaleType != InOutScaleType.DTS_NONE)
            {
                Vector3 scale = active ? Vector3.one : Vector3.zero;
                sequence.Join(_rect.DOScale(scale, ANIM_TIME));
            }

            if (AlphaType != InOutAlphaType.DTA_NONE)
            {
                float endAlpha = active ? 1f : 0f;
                sequence.Join(DOTween.To(() => _canvasGroup.alpha, v => _canvasGroup.alpha = v, endAlpha, ANIM_TIME));
            }

            if (ScaleType == InOutScaleType.DTS_SCALE_ZERO_JELLY && active)
            {
                sequence.AppendCallback(() =>
                {
                    _rect.DOScale(Vector3.one * 1.2f, JELLY_TIME)
                    .OnComplete(() =>
                    {
                        _rect.DOScale(Vector3.one, JELLY_TIME)
                        .OnComplete(() =>
                        {
                            Utility.SetActive(this.gameObject, active);
                            _canvasGroup.blocksRaycasts = active && BlockRaycast;
                        });
                    });
                });
            }
            else
            {
                sequence.AppendCallback(() =>
                {
                    Utility.SetActive(this.gameObject, active);
                    _canvasGroup.blocksRaycasts = active && BlockRaycast;
                });
            }
        }

        /// <summary>
        /// 可以再Form.cs里面监听窗口事件，也可以在界面里面重写该方法去监听窗口事件
        /// </summary>
        public virtual void SurveilEvent(EventHandle ets = EventHandle.ET_NONE)
        {
            EventHandle eventType = ets == EventHandle.ET_NONE ? EventHandle.ET_NONE : ets;
            switch (_formType)
            {
                case FormType.FT_EXCAMPLE:
                    eventType = EventHandle.ET_FORM_EXCAMPLE;
                    break;
            }

            if (eventType > EventHandle.FORM_BEGIN && eventType < EventHandle.FORM_END)
            {
                EventCentre.Instance.SurveilEvent(onEventHandler, eventType);
            }
        }

        protected abstract void onEventHandler(EventParam eventData);

        private void CalculateClosePos()
        {
            switch (MoveType)
            {
                case InOutMoveType.DTM_NONE:
                    _closePos = _openPos;
                    break;
                case InOutMoveType.DTM_MOVE_HORIZONTAL_RIGHT:
                    _closePos = new Vector2(Utility.ScreenVector().x, _openPos.y);
                    break;
                case InOutMoveType.DTM_MOVE_HORIZONTAL_LEFT:
                    _closePos = new Vector2(-Utility.ScreenVector().x, _openPos.y);
                    break;
                case InOutMoveType.DTM_MOVE_VERTICAL_UP:
                    _closePos = new Vector2(_openPos.x, Utility.ScreenVector().y);
                    break;
                case InOutMoveType.DTM_MOVE_VERTICAL_DOWN:
                    _closePos = new Vector2(_openPos.x, -Utility.ScreenVector().y);
                    break;
            }
        }

        private void SetDisplayType()
        {
            displayType = OriginalDisplayType == DisplayeType.DT_RESIDENT ? DisplayeType.DT_DYNAMIC : OriginalDisplayType;
        }

        public void SetAsFirstSibling()
        {
            if (OriginalDisplayType == DisplayeType.DT_RESIDENT)
                return;
            this.transform.SetAsFirstSibling();
        }
    }
}