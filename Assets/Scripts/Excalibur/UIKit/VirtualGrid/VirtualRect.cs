using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Excalibur;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using NPOI.SS.Formula.Functions;

namespace Excalibur
{
    public enum ScrollAxis
    {
        Horizontal,
        Vertical,
    }

    public delegate void OnScroll();

    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(Mask))]
	public sealed class VirtualRect : MonoBehaviour, IScrollHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private ScrollAxis m_ScrollAxis = ScrollAxis.Vertical;
        public ScrollAxis scrollAxis { get { return m_ScrollAxis; } set { m_ScrollAxis = value; } }

        private float m_ScrollSensitivity = 100f;
        public float ScrollSensitivity { get { return m_ScrollSensitivity; } set { m_ScrollSensitivity = value; } }

        private float m_AccelarateScrollSensitivity = 0.1f;
        public float accelarateScrollSensitivity 
        { get { return m_AccelarateScrollSensitivity; } set { m_AccelarateScrollSensitivity = value; } }

        private RectTransform m_Rect;
        private RectTransform Rect
        {
            get
            {
                if (m_Rect == null)
                    m_Rect = transform as RectTransform;
                return m_Rect;
            }
        }
        private RectTransform m_Content;
        private RectTransform Content
        {
            get
            {
                if (m_Content == null)
                    m_Content = transform.GetChild(0).GetComponent<RectTransform>();
                return m_Content;
            }
        }

        private event OnScroll onScrollEvent;
        public event OnScroll OnScrollEvent  { add { onScrollEvent += value; } remove { onScrollEvent -= value; } }

        private Vector2 m_Velocity = Vector2.zero;

        private bool m_Scrolling;
        private bool m_Dragging;
        private Vector2 m_PointerDragStartPositionLocal;
        private Vector2 m_DragStartPosition;
        private Vector2 m_ContentOffset;

        private void Awake()
		{
        }

        private void OnEnable()
        {
            OnScrollEvent += OnVirtualRectScroll;
        }

        private void OnDisable()
        {
            OnScrollEvent -= OnVirtualRectScroll;
        }

        private void Update()
        {
            if (m_Velocity.magnitude > 0f)
            {
                onScrollEvent.Invoke();
            }

            if (!m_Dragging && m_Scrolling)
            {
                Vector2 delta = Input.mouseScrollDelta;
                delta.y *= -1f;
                int axis = 0;
                switch (scrollAxis)
                {
                    case ScrollAxis.Horizontal:
                        if (Mathf.Abs(delta.y) > Mathf.Abs(delta.x))
                            delta.x = delta.y;
                        delta.y = 0f;
                        break;
                    case ScrollAxis.Vertical:
                        axis = 1;
                        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                            delta.y = delta.x;
                        delta.x = 0f;
                        break;
                }
                m_AccelarateScrollSensitivity += Time.unscaledDeltaTime;
                m_Velocity[axis] += m_AccelarateScrollSensitivity * Time.unscaledDeltaTime;
                Vector2 position = Content.anchoredPosition;
                position += m_Velocity * delta;
                SetContentAnchoredPosition(position);
            }
            else if (!m_Dragging && m_Velocity != Vector2.zero)
            {

                for (int axis = 0; axis < 2; ++axis)
                {
                    m_Velocity[axis] = Mathf.Lerp(m_Velocity[axis], 0f, Time.unscaledDeltaTime);
                    Vector2 position = Content.anchoredPosition;
                    position += m_Velocity * Time.deltaTime;
                    SetContentAnchoredPosition(position);
                }
            }
        }

        private void LateUpdate()
        {
        }

        public void OnScroll(PointerEventData eventData)
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            m_Scrolling = eventData.IsScrolling();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!isActiveAndEnabled || eventData.button != PointerEventData.InputButton.Left)
                return;

            m_Dragging = true;
        }

        public void OnDrag(PointerEventData eventData)
        {

        }

        public void OnEndDrag(PointerEventData eventData)
        {
            m_Dragging = false;
        }

        private void SetContentAnchoredPosition(Vector2 position)
        {
            switch (scrollAxis)
            {
                case ScrollAxis.Horizontal:
                    position.y = Content.anchoredPosition.y;
                    break;
                case ScrollAxis.Vertical:
                    position.x = Content.anchoredPosition.x;
                    break;
            }

            if (m_ContentOffset.magnitude > 0f)
            {

            }

            Content.anchoredPosition = position;
        }

        private void OnVirtualRectScroll()
        {

        }
    }
}
