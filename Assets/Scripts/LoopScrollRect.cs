using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.UI
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public abstract class LoopScrollRect : UIBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollHandler, ICanvasElement, ILayoutElement, ILayoutGroup
    {
        //==========LoopScrollRect==========
        [Tooltip("Prefab Source")]
        public LoopScrollPrefabSource prefabSource;

        [Tooltip("Total count, negative means INFINITE mode")]
        public int totalCount;

        [HideInInspector]
        [NonSerialized]
        public LoopScrollDataSource dataSource = LoopScrollSendIndexSource.Instance;
        public object[] objectsToFill
        {
            // wrapper for forward compatbility
            set
            {
                if (value != null)
                    dataSource = new LoopScrollArraySource<object>(value);
                else
                    dataSource = LoopScrollSendIndexSource.Instance;
            }
        }

        protected float threshold = 0;
        [Tooltip("Reverse direction for dragging")]
        public bool reverseDirection = false;
        [Tooltip("Rubber scale for outside")]
        public float rubberScale = 1;

        protected int itemTypeStart = 0;
        protected int itemTypeEnd = 0;

        protected abstract float GetSize(RectTransform item);
        protected abstract float GetDimension(Vector2 vector);
        protected abstract Vector2 GetVector(float value);
        protected enum LoopScrollRectDirection
        {
            Vertical,
            Horizontal,
        }
        protected LoopScrollRectDirection direction = LoopScrollRectDirection.Horizontal;

        private bool m_ContentSpaceInit = false;
        private float m_ContentSpacing = 0;
        protected float m_ContentLeftPadding = 0;
        protected float m_ContentRightPadding = 0;
        protected float m_ContentTopPadding = 0;
        protected float m_ContentBottomPadding = 0;
        protected GridLayoutGroup m_GridLayout = null;
        protected float contentSpacing
        {
            get
            {
                if (m_ContentSpaceInit)
                {
                    return m_ContentSpacing;
                }
                m_ContentSpaceInit = true;
                m_ContentSpacing = 0;
                if (content != null)
                {
                    HorizontalOrVerticalLayoutGroup layout1 = content.GetComponent<HorizontalOrVerticalLayoutGroup>();
                    if (layout1 != null)
                    {
                        m_ContentSpacing = layout1.spacing;
                        m_ContentLeftPadding = layout1.padding.left;
                        m_ContentRightPadding = layout1.padding.right;
                        m_ContentTopPadding = layout1.padding.top;
                        m_ContentBottomPadding = layout1.padding.bottom;
                    }
                    m_GridLayout = content.GetComponent<GridLayoutGroup>();
                    if (m_GridLayout != null)
                    {
                        m_ContentSpacing = Mathf.Abs(GetDimension(m_GridLayout.spacing));
                        m_ContentLeftPadding = m_GridLayout.padding.left;
                        m_ContentRightPadding = m_GridLayout.padding.right;
                        m_ContentTopPadding = m_GridLayout.padding.top;
                        m_ContentBottomPadding = m_GridLayout.padding.bottom;
                    }
                }
                return m_ContentSpacing;
            }
        }

        private bool m_ContentConstraintCountInit = false;
        private int m_ContentConstraintCount = 0;
        protected int contentConstraintCount
        {
            get
            {
                if (m_ContentConstraintCountInit)
                {
                    return m_ContentConstraintCount;
                }
                m_ContentConstraintCountInit = true;
                m_ContentConstraintCount = 1;
                if (content != null)
                {
                    GridLayoutGroup layout2 = content.GetComponent<GridLayoutGroup>();
                    if (layout2 != null)
                    {
                        if (layout2.constraint == GridLayoutGroup.Constraint.Flexible)
                        {
                            Debug.LogWarning("[LoopScrollRect] Flexible not supported yet");
                        }
                        m_ContentConstraintCount = layout2.constraintCount;
                    }
                }
                return m_ContentConstraintCount;
            }
        }

        // the first line
        protected int StartLine
        {
            get
            {
                return Mathf.CeilToInt((float)(itemTypeStart) / contentConstraintCount);
            }
        }

        // how many lines we have for now
        protected int CurrentLines
        {
            get
            {
                return Mathf.CeilToInt((float)(itemTypeEnd - itemTypeStart) / contentConstraintCount);
            }
        }

        // how many lines we have in total
        protected int TotalLines
        {
            get
            {
                return Mathf.CeilToInt((float)(totalCount) / contentConstraintCount);
            }
        }

        protected virtual bool UpdateItems(Bounds viewBounds, Bounds contentBounds) { return false; }
        //==========LoopScrollRect==========

        public enum MovementType
        {
            Unrestricted, // Unrestricted movement -- can scroll forever
            Elastic, // Restricted but flexible -- can go past the edges, but springs back in place
            Clamped, // Restricted movement where it's not possible to go past the edges
        }

        public enum ScrollbarVisibility
        {
            Permanent,
            AutoHide,
            AutoHideAndExpandViewport,
        }

        [Serializable]
        public class ScrollRectEvent : UnityEvent<Vector2> { }

        [SerializeField]
        private RectTransform m_Content;
        public RectTransform content { get { return m_Content; } set { m_Content = value; } }

        [SerializeField]
        private bool m_Horizontal = true;
        public bool horizontal { get { return m_Horizontal; } set { m_Horizontal = value; } }

        [SerializeField]
        private bool m_Vertical = true;
        public bool vertical { get { return m_Vertical; } set { m_Vertical = value; } }

        [SerializeField]
        private MovementType m_MovementType = MovementType.Elastic;
        public MovementType movementType { get { return m_MovementType; } set { m_MovementType = value; } }

        [SerializeField]
        private float m_Elasticity = 0.1f; // Only used for MovementType.Elastic
        public float elasticity { get { return m_Elasticity; } set { m_Elasticity = value; } }

        [SerializeField]
        private bool m_Inertia = true;
        public bool inertia { get { return m_Inertia; } set { m_Inertia = value; } }

        [SerializeField]
        private float m_DecelerationRate = 0.135f; // Only used when inertia is enabled
        public float decelerationRate { get { return m_DecelerationRate; } set { m_DecelerationRate = value; } }

        [SerializeField]
        private float m_ScrollSensitivity = 1.0f;
        public float scrollSensitivity { get { return m_ScrollSensitivity; } set { m_ScrollSensitivity = value; } }

        [SerializeField]
        private RectTransform m_Viewport;
        public RectTransform viewport { get { return m_Viewport; } set { m_Viewport = value; SetDirtyCaching(); } }

        [SerializeField]
        private Scrollbar m_HorizontalScrollbar;
        public Scrollbar horizontalScrollbar
        {
            get
            {
                return m_HorizontalScrollbar;
            }
            set
            {
                if (m_HorizontalScrollbar)
                    m_HorizontalScrollbar.onValueChanged.RemoveListener(SetHorizontalNormalizedPosition);
                m_HorizontalScrollbar = value;
                if (m_HorizontalScrollbar)
                    m_HorizontalScrollbar.onValueChanged.AddListener(SetHorizontalNormalizedPosition);
                SetDirtyCaching();
            }
        }

        [SerializeField]
        private Scrollbar m_VerticalScrollbar;
        public Scrollbar verticalScrollbar
        {
            get
            {
                return m_VerticalScrollbar;
            }
            set
            {
                if (m_VerticalScrollbar)
                    m_VerticalScrollbar.onValueChanged.RemoveListener(SetVerticalNormalizedPosition);
                m_VerticalScrollbar = value;
                if (m_VerticalScrollbar)
                    m_VerticalScrollbar.onValueChanged.AddListener(SetVerticalNormalizedPosition);
                SetDirtyCaching();
            }
        }

        [SerializeField]
        private ScrollbarVisibility m_HorizontalScrollbarVisibility;
        public ScrollbarVisibility horizontalScrollbarVisibility { get { return m_HorizontalScrollbarVisibility; } set { m_HorizontalScrollbarVisibility = value; SetDirtyCaching(); } }

        [SerializeField]
        private ScrollbarVisibility m_VerticalScrollbarVisibility;
        public ScrollbarVisibility verticalScrollbarVisibility { get { return m_VerticalScrollbarVisibility; } set { m_VerticalScrollbarVisibility = value; SetDirtyCaching(); } }

        [SerializeField]
        private float m_HorizontalScrollbarSpacing;
        public float horizontalScrollbarSpacing { get { return m_HorizontalScrollbarSpacing; } set { m_HorizontalScrollbarSpacing = value; SetDirty(); } }

        [SerializeField]
        private float m_VerticalScrollbarSpacing;
        public float verticalScrollbarSpacing { get { return m_VerticalScrollbarSpacing; } set { m_VerticalScrollbarSpacing = value; SetDirty(); } }

        [SerializeField]
        private ScrollRectEvent m_OnValueChanged = new ScrollRectEvent();
        public ScrollRectEvent onValueChanged { get { return m_OnValueChanged; } set { m_OnValueChanged = value; } }

        // The offset from handle position to mouse down position
        private Vector2 m_PointerStartLocalCursor = Vector2.zero;
        private Vector2 m_ContentStartPosition = Vector2.zero;

        private RectTransform m_ViewRect;

        protected RectTransform viewRect
        {
            get
            {
                if (m_ViewRect == null)
                    m_ViewRect = m_Viewport;
                if (m_ViewRect == null)
                    m_ViewRect = (RectTransform)transform;
                return m_ViewRect;
            }
        }

        private Bounds m_ContentBounds;
        private Bounds m_ViewBounds;

        private Vector2 m_Velocity;
        public Vector2 velocity { get { return m_Velocity; } set { m_Velocity = value; } }

        private bool m_Dragging;

        private Vector2 m_PrevPosition = Vector2.zero;
        private Bounds m_PrevContentBounds;
        private Bounds m_PrevViewBounds;
        [NonSerialized]
        private bool m_HasRebuiltLayout = false;

        private bool m_HSliderExpand;
        private bool m_VSliderExpand;
        private float m_HSliderHeight;
        private float m_VSliderWidth;

        [System.NonSerialized]
        private RectTransform m_Rect;
        private RectTransform rectTransform
        {
            get
            {
                if (m_Rect == null)
                    m_Rect = GetComponent<RectTransform>();
                return m_Rect;
            }
        }

        private RectTransform m_HorizontalScrollbarRect;
        private RectTransform m_VerticalScrollbarRect;

        private DrivenRectTransformTracker m_Tracker;

        protected LoopScrollRect()
        {
            flexibleWidth = -1;
        }

        //==========LoopScrollRect==========
#if UNITY_EDITOR
        protected override void Awake()
        {
            base.Awake();
            if (Application.isPlaying)
            {
                float value = (reverseDirection ^ (direction == LoopScrollRectDirection.Horizontal)) ? 0 : 1;
                Debug.Assert(Mathf.Abs(GetDimension(content.pivot)) == value, this);
                Debug.Assert(Mathf.Abs(GetDimension(content.anchorMin)) == value, this);
                Debug.Assert(Mathf.Abs(GetDimension(content.anchorMax)) == value, this);
            }
        }
#endif

        public void ClearCells()
        {
            if (Application.isPlaying)
            {
                itemTypeStart = 0;
                itemTypeEnd = 0;
                totalCount = 0;
                objectsToFill = null;
                for (int i = content.childCount - 1; i >= 0; i--)
                {
                    prefabSource.ReturnObject(content.GetChild(i));
                }
            }
        }

        public void SrollToCell(int index, float speed)
        {
            if (totalCount >= 0 && (index < 0 || index >= totalCount))
            {
                Debug.LogErrorFormat("invalid index {0}", index);
                return;
            }
            StopAllCoroutines();
            if (speed <= 0)
            {
                RefillCells(index);
                return;
            }
            StartCoroutine(ScrollToCellCoroutine(index, speed));
        }

        IEnumerator ScrollToCellCoroutine(int index, float speed)
        {
            bool needMoving = true;
            while (needMoving)
            {
                yield return null;
                if (!m_Dragging)
                {
                    float move = 0;
                    if (index < itemTypeStart)
                    {
                        move = -Time.deltaTime * speed;
                    }
                    else if (index >= itemTypeEnd)
                    {
                        move = Time.deltaTime * speed;
                    }
                    else
                    {
                        m_ViewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
                        var m_ItemBounds = GetBounds4Item(index);
                        var offset = 0.0f;
                        if (direction == LoopScrollRectDirection.Vertical)
                            offset = reverseDirection ? (m_ViewBounds.min.y - m_ItemBounds.min.y) : (m_ViewBounds.max.y - m_ItemBounds.max.y);
                        else
                            offset = reverseDirection ? (m_ItemBounds.max.x - m_ViewBounds.max.x) : (m_ItemBounds.min.x - m_ViewBounds.min.x);
                        // check if we cannot move on
                        if (totalCount >= 0)
                        {
                            if (offset > 0 && itemTypeEnd == totalCount && !reverseDirection)
                            {
                                m_ItemBounds = GetBounds4Item(totalCount - 1);
                                // reach bottom
                                if ((direction == LoopScrollRectDirection.Vertical && m_ItemBounds.min.y > m_ViewBounds.min.y) ||
                                    (direction == LoopScrollRectDirection.Horizontal && m_ItemBounds.max.x < m_ViewBounds.max.x))
                                {
                                    needMoving = false;
                                    break;
                                }
                            }
                            else if (offset < 0 && itemTypeStart == 0 && reverseDirection)
                            {
                                m_ItemBounds = GetBounds4Item(0);
                                if ((direction == LoopScrollRectDirection.Vertical && m_ItemBounds.max.y < m_ViewBounds.max.y) ||
                                    (direction == LoopScrollRectDirection.Horizontal && m_ItemBounds.min.x > m_ViewBounds.min.x))
                                {
                                    needMoving = false;
                                    break;
                                }
                            }
                        }

                        float maxMove = Time.deltaTime * speed;
                        if (Mathf.Abs(offset) < maxMove)
                        {
                            needMoving = false;
                            move = offset;
                        }
                        else
                            move = Mathf.Sign(offset) * maxMove;
                    }
                    if (move != 0)
                    {
                        Vector2 offset = GetVector(move);
                        content.anchoredPosition += offset;
                        m_PrevPosition += offset;
                        m_ContentStartPosition += offset;
                        UpdateBounds(true);
                    }
                }
            }
            StopMovement();
            UpdatePrevData();
        }

        public void RefreshCells()
        {
            if (Application.isPlaying && this.isActiveAndEnabled)
            {
                itemTypeEnd = itemTypeStart;
                // recycle items if we can
                for (int i = 0; i < content.childCount; i++)
                {
                    if (itemTypeEnd < totalCount)
                    {
                        dataSource.ProvideData(content.GetChild(i), itemTypeEnd);
                        itemTypeEnd++;
                    }
                    else
                    {
                        prefabSource.ReturnObject(content.GetChild(i));
                        i--;
                    }
                }
            }
        }

        public void RefillCellsFromEnd(int offset = 0, bool alignStart = false)
        {
            if (!Application.isPlaying || prefabSource == null)
                return;

            StopMovement();
            itemTypeEnd = reverseDirection ? offset : totalCount - offset;
            itemTypeStart = itemTypeEnd;

            if (totalCount >= 0 && itemTypeStart % contentConstraintCount != 0)
            {
                itemTypeStart = (itemTypeStart / contentConstraintCount) * contentConstraintCount;
            }

            ReturnToTempPool(!reverseDirection, m_Content.childCount);

            float sizeToFill = Mathf.Abs(GetDimension(viewRect.rect.size)), sizeFilled = 0;

            while (sizeToFill > sizeFilled)
            {
                float size = reverseDirection ? NewItemAtEnd() : NewItemAtStart();
                if (size <= 0)
                    break;
                sizeFilled += size;
            }

            // refill from start in case not full yet
            while (sizeToFill > sizeFilled)
            {
                float size = reverseDirection ? NewItemAtStart() : NewItemAtEnd();
                if (size <= 0)
                    break;
                sizeFilled += size;
            }

            Vector2 pos = m_Content.anchoredPosition;
            float dist = alignStart ? 0 : Mathf.Max(0, sizeFilled - sizeToFill);
            if (reverseDirection)
                dist = -dist;
            if (direction == LoopScrollRectDirection.Vertical)
                pos.y = dist;
            else
                pos.x = -dist;
            m_Content.anchoredPosition = pos;
            m_ContentStartPosition = pos;

            ClearTempPool();
            UpdateScrollbars(Vector2.zero);
        }

        public void RefillCells(int offset = 0, bool fillViewRect = false)
        {
            if (!Application.isPlaying || prefabSource == null)
                return;

            StopMovement();
            itemTypeStart = reverseDirection ? totalCount - offset : offset;
            if (totalCount >= 0 && itemTypeStart % contentConstraintCount != 0)
            {
                itemTypeStart = (itemTypeStart / contentConstraintCount) * contentConstraintCount;
            }
            itemTypeEnd = itemTypeStart;

            // Don't `Canvas.ForceUpdateCanvases();` here, or it will new/delete cells to change itemTypeStart/End
            ReturnToTempPool(reverseDirection, m_Content.childCount);

            float sizeToFill = Mathf.Abs(GetDimension(viewRect.rect.size)), sizeFilled = 0;
            // m_ViewBounds may be not ready when RefillCells on Start

            float itemSize = 0;

            while (sizeToFill > sizeFilled)
            {
                float size = reverseDirection ? NewItemAtStart() : NewItemAtEnd();
                if (size <= 0)
                    break;
                itemSize = size;
                sizeFilled += size;
            }

            // refill from start in case not full yet
            while (sizeToFill > sizeFilled)
            {
                float size = reverseDirection ? NewItemAtEnd() : NewItemAtStart();
                if (size <= 0)
                    break;
                sizeFilled += size;
            }

            if (fillViewRect && itemSize > 0 && sizeFilled < sizeToFill)
            {
                int itemsToAddCount = (int)((sizeToFill - sizeFilled) / itemSize);        //calculate how many items can be added above the offset, so it still is visible in the view
                int newOffset = offset - itemsToAddCount;
                if (newOffset < 0) newOffset = 0;
                if (newOffset != offset) RefillCells(newOffset);                 //refill again, with the new offset value, and now with fillViewRect disabled.
            }

            Vector2 pos = m_Content.anchoredPosition;
            if (direction == LoopScrollRectDirection.Vertical)
                pos.y = 0;
            else
                pos.x = 0;
            m_Content.anchoredPosition = pos;
            m_ContentStartPosition = pos;

            ClearTempPool();
            UpdateScrollbars(Vector2.zero);
        }

        protected float NewItemAtStart()
        {
            if (totalCount >= 0 && itemTypeStart - contentConstraintCount < 0)
            {
                return 0;
            }
            float size = 0;
            for (int i = 0; i < contentConstraintCount; i++)
            {
                itemTypeStart--;
                RectTransform newItem = GetFromTempPool(itemTypeStart);
                newItem.SetSiblingIndex(deletedItemTypeStart);
                size = Mathf.Max(GetSize(newItem), size);
            }
            threshold = Mathf.Max(threshold, size * 1.5f);

            if (!reverseDirection)
            {
                Vector2 offset = GetVector(size);
                content.anchoredPosition += offset;
                m_PrevPosition += offset;
                m_ContentStartPosition += offset;
            }

            return size;
        }

        protected float DeleteItemAtStart()
        {
            // special case: when moving or dragging, we cannot simply delete start when we've reached the end
            if ((m_Dragging || m_Velocity != Vector2.zero) && totalCount >= 0 && itemTypeEnd >= totalCount - contentConstraintCount)
            {
                return 0;
            }
            int availableChilds = content.childCount - deletedItemTypeStart - deletedItemTypeEnd;
            Debug.Assert(availableChilds >= 0);
            if (availableChilds == 0)
            {
                return 0;
            }

            float size = 0;
            for (int i = 0; i < contentConstraintCount; i++)
            {
                RectTransform oldItem = content.GetChild(deletedItemTypeStart) as RectTransform;
                size = Mathf.Max(GetSize(oldItem), size);
                ReturnToTempPool(true);
                availableChilds--;
                itemTypeStart++;

                if (availableChilds == 0)
                {
                    break;
                }
            }

            if (!reverseDirection)
            {
                Vector2 offset = GetVector(size);
                content.anchoredPosition -= offset;
                m_PrevPosition -= offset;
                m_ContentStartPosition -= offset;
            }
            return size;
        }


        protected float NewItemAtEnd()
        {
            if (totalCount >= 0 && itemTypeEnd >= totalCount)
            {
                return 0;
            }
            float size = 0;
            // issue 4: fill lines to end first
            int availableChilds = content.childCount - deletedItemTypeStart - deletedItemTypeEnd;
            int count = contentConstraintCount - (availableChilds % contentConstraintCount);
            for (int i = 0; i < count; i++)
            {
                RectTransform newItem = GetFromTempPool(itemTypeEnd);
                newItem.SetSiblingIndex(content.childCount - deletedItemTypeEnd - 1);
                size = Mathf.Max(GetSize(newItem), size);
                itemTypeEnd++;
                if (totalCount >= 0 && itemTypeEnd >= totalCount)
                {
                    break;
                }
            }
            threshold = Mathf.Max(threshold, size * 1.5f);

            if (reverseDirection)
            {
                Vector2 offset = GetVector(size);
                content.anchoredPosition -= offset;
                m_PrevPosition -= offset;
                m_ContentStartPosition -= offset;
            }

            return size;
        }

        protected float DeleteItemAtEnd()
        {
            if ((m_Dragging || m_Velocity != Vector2.zero) && totalCount >= 0 && itemTypeStart < contentConstraintCount)
            {
                return 0;
            }
            int availableChilds = content.childCount - deletedItemTypeStart - deletedItemTypeEnd;
            Debug.Assert(availableChilds >= 0);
            if (availableChilds == 0)
            {
                return 0;
            }

            float size = 0;
            for (int i = 0; i < contentConstraintCount; i++)
            {
                RectTransform oldItem = content.GetChild(content.childCount - deletedItemTypeEnd - 1) as RectTransform;
                size = Mathf.Max(GetSize(oldItem), size);
                ReturnToTempPool(false);
                availableChilds--;
                itemTypeEnd--;
                if (itemTypeEnd % contentConstraintCount == 0 || availableChilds == 0)
                {
                    break;  //just delete the whole row
                }
            }

            if (reverseDirection)
            {
                Vector2 offset = GetVector(size);
                content.anchoredPosition += offset;
                m_PrevPosition += offset;
                m_ContentStartPosition += offset;
            }
            return size;
        }

        int deletedItemTypeStart = 0;
        int deletedItemTypeEnd = 0;
        protected RectTransform GetFromTempPool(int itemIdx)
        {
            RectTransform nextItem = null;
            if (deletedItemTypeStart > 0)
            {
                deletedItemTypeStart--;
                nextItem = content.GetChild(0) as RectTransform;
                nextItem.SetSiblingIndex(itemIdx - itemTypeStart + deletedItemTypeStart);
            }
            else if (deletedItemTypeEnd > 0)
            {
                deletedItemTypeEnd--;
                nextItem = content.GetChild(content.childCount - 1) as RectTransform;
                nextItem.SetSiblingIndex(itemIdx - itemTypeStart + deletedItemTypeStart);
            }
            else
            {
                nextItem = prefabSource.GetObject().transform as RectTransform;
                nextItem.transform.SetParent(content, false);
                nextItem.gameObject.SetActive(true);
            }
            dataSource.ProvideData(nextItem, itemIdx);
            return nextItem;
        }
        protected void ReturnToTempPool(bool fromStart, int count = 1)
        {
            if (fromStart)
                deletedItemTypeStart += count;
            else
                deletedItemTypeEnd += count;
        }
        protected void ClearTempPool()
        {
            while (deletedItemTypeStart > 0)
            {
                deletedItemTypeStart--;
                prefabSource.ReturnObject(content.GetChild(0));
            }
            while (deletedItemTypeEnd > 0)
            {
                deletedItemTypeEnd--;
                prefabSource.ReturnObject(content.GetChild(content.childCount - 1));
            }
        }
        //==========LoopScrollRect==========

        public virtual void Rebuild(CanvasUpdate executing)
        {
            if (executing == CanvasUpdate.Prelayout)
            {
                UpdateCachedData();
            }

            if (executing == CanvasUpdate.PostLayout)
            {
                UpdateBounds();
                UpdateScrollbars(Vector2.zero);
                UpdatePrevData();

                m_HasRebuiltLayout = true;
            }
        }

        public virtual void LayoutComplete()
        { }

        public virtual void GraphicUpdateComplete()
        { }

        void UpdateCachedData()
        {
            Transform transform = this.transform;
            m_HorizontalScrollbarRect = m_HorizontalScrollbar == null ? null : m_HorizontalScrollbar.transform as RectTransform;
            m_VerticalScrollbarRect = m_VerticalScrollbar == null ? null : m_VerticalScrollbar.transform as RectTransform;

            // These are true if either the elements are children, or they don't exist at all.
            bool viewIsChild = (viewRect.parent == transform);
            bool hScrollbarIsChild = (!m_HorizontalScrollbarRect || m_HorizontalScrollbarRect.parent == transform);
            bool vScrollbarIsChild = (!m_VerticalScrollbarRect || m_VerticalScrollbarRect.parent == transform);
            bool allAreChildren = (viewIsChild && hScrollbarIsChild && vScrollbarIsChild);

            m_HSliderExpand = allAreChildren && m_HorizontalScrollbarRect && horizontalScrollbarVisibility == ScrollbarVisibility.AutoHideAndExpandViewport;
            m_VSliderExpand = allAreChildren && m_VerticalScrollbarRect && verticalScrollbarVisibility == ScrollbarVisibility.AutoHideAndExpandViewport;
            m_HSliderHeight = (m_HorizontalScrollbarRect == null ? 0 : m_HorizontalScrollbarRect.rect.height);
            m_VSliderWidth = (m_VerticalScrollbarRect == null ? 0 : m_VerticalScrollbarRect.rect.width);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (m_HorizontalScrollbar)
                m_HorizontalScrollbar.onValueChanged.AddListener(SetHorizontalNormalizedPosition);
            if (m_VerticalScrollbar)
                m_VerticalScrollbar.onValueChanged.AddListener(SetVerticalNormalizedPosition);

            CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
        }

        protected override void OnDisable()
        {
            CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);

            if (m_HorizontalScrollbar)
                m_HorizontalScrollbar.onValueChanged.RemoveListener(SetHorizontalNormalizedPosition);
            if (m_VerticalScrollbar)
                m_VerticalScrollbar.onValueChanged.RemoveListener(SetVerticalNormalizedPosition);

            m_HasRebuiltLayout = false;
            m_Tracker.Clear();
            m_Velocity = Vector2.zero;
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            base.OnDisable();
        }

        public override bool IsActive()
        {
            return base.IsActive() && m_Content != null;
        }

        private void EnsureLayoutHasRebuilt()
        {
            if (!m_HasRebuiltLayout && !CanvasUpdateRegistry.IsRebuildingLayout())
                Canvas.ForceUpdateCanvases();
        }

        public virtual void StopMovement()
        {
            m_Velocity = Vector2.zero;
        }

        public virtual void OnScroll(PointerEventData data)
        {
            if (!IsActive())
                return;

            EnsureLayoutHasRebuilt();
            UpdateBounds();

            Vector2 delta = data.scrollDelta;
            // Down is positive for scroll events, while in UI system up is positive.
            delta.y *= -1;
            if (vertical && !horizontal)
            {
                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                    delta.y = delta.x;
                delta.x = 0;
            }
            if (horizontal && !vertical)
            {
                if (Mathf.Abs(delta.y) > Mathf.Abs(delta.x))
                    delta.x = delta.y;
                delta.y = 0;
            }

            Vector2 position = m_Content.anchoredPosition;
            position += delta * m_ScrollSensitivity;
            if (m_MovementType == MovementType.Clamped)
                position += CalculateOffset(position - m_Content.anchoredPosition);

            SetContentAnchoredPosition(position);
            UpdateBounds();
        }

        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            m_Velocity = Vector2.zero;
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (!IsActive())
                return;

            UpdateBounds();

            m_PointerStartLocalCursor = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(viewRect, eventData.position, eventData.pressEventCamera, out m_PointerStartLocalCursor);
            m_ContentStartPosition = m_Content.anchoredPosition;
            m_Dragging = true;
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            m_Dragging = false;
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (!IsActive())
                return;

            Vector2 localCursor;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(viewRect, eventData.position, eventData.pressEventCamera, out localCursor))
                return;

            UpdateBounds();

            var pointerDelta = localCursor - m_PointerStartLocalCursor;
            Vector2 position = m_ContentStartPosition + pointerDelta;

            // Offset to get content into place in the view.
            Vector2 offset = CalculateOffset(position - m_Content.anchoredPosition);
            position += offset;
            if (m_MovementType == MovementType.Elastic)
            {
                //==========LoopScrollRect==========
                if (offset.x != 0)
                    position.x = position.x - RubberDelta(offset.x, m_ViewBounds.size.x) * rubberScale;
                if (offset.y != 0)
                    position.y = position.y - RubberDelta(offset.y, m_ViewBounds.size.y) * rubberScale;
                //==========LoopScrollRect==========
            }

            SetContentAnchoredPosition(position);
        }

        protected virtual void SetContentAnchoredPosition(Vector2 position)
        {
            if (!m_Horizontal)
                position.x = m_Content.anchoredPosition.x;
            if (!m_Vertical)
                position.y = m_Content.anchoredPosition.y;

            if ((position - m_Content.anchoredPosition).sqrMagnitude > 0.001f)
            {
                m_Content.anchoredPosition = position;
                UpdateBounds(true);
            }
        }

        protected virtual void LateUpdate()
        {
            if (!m_Content)
                return;

            EnsureLayoutHasRebuilt();
            UpdateScrollbarVisibility();
            UpdateBounds();
            float deltaTime = Time.unscaledDeltaTime;
            Vector2 offset = CalculateOffset(Vector2.zero);
            if (!m_Dragging && (offset != Vector2.zero || m_Velocity != Vector2.zero))
            {
                Vector2 position = m_Content.anchoredPosition;
                for (int axis = 0; axis < 2; axis++)
                {
                    // Apply spring physics if movement is elastic and content has an offset from the view.
                    if (m_MovementType == MovementType.Elastic && offset[axis] != 0)
                    {
                        float speed = m_Velocity[axis];
                        position[axis] = Mathf.SmoothDamp(m_Content.anchoredPosition[axis], m_Content.anchoredPosition[axis] + offset[axis], ref speed, m_Elasticity, Mathf.Infinity, deltaTime);
                        m_Velocity[axis] = speed;
                    }
                    // Else move content according to velocity with deceleration applied.
                    else if (m_Inertia)
                    {
                        m_Velocity[axis] *= Mathf.Pow(m_DecelerationRate, deltaTime);
                        if (Mathf.Abs(m_Velocity[axis]) < 1)
                            m_Velocity[axis] = 0;
                        position[axis] += m_Velocity[axis] * deltaTime;
                    }
                    // If we have neither elaticity or friction, there shouldn't be any velocity.
                    else
                    {
                        m_Velocity[axis] = 0;
                    }
                }

                if (m_Velocity != Vector2.zero)
                {
                    if (m_MovementType == MovementType.Clamped)
                    {
                        offset = CalculateOffset(position - m_Content.anchoredPosition);
                        position += offset;
                    }

                    SetContentAnchoredPosition(position);
                }
            }

            if (m_Dragging && m_Inertia)
            {
                Vector3 newVelocity = (m_Content.anchoredPosition - m_PrevPosition) / deltaTime;
                m_Velocity = Vector3.Lerp(m_Velocity, newVelocity, deltaTime * 10);
            }

            if (m_ViewBounds != m_PrevViewBounds || m_ContentBounds != m_PrevContentBounds || m_Content.anchoredPosition != m_PrevPosition)
            {
                UpdateScrollbars(offset);
                m_OnValueChanged.Invoke(normalizedPosition);
                UpdatePrevData();
            }
        }

        private void UpdatePrevData()
        {
            if (m_Content == null)
                m_PrevPosition = Vector2.zero;
            else
                m_PrevPosition = m_Content.anchoredPosition;
            m_PrevViewBounds = m_ViewBounds;
            m_PrevContentBounds = m_ContentBounds;
        }

        private void UpdateScrollbars(Vector2 offset)
        {
            if (m_HorizontalScrollbar)
            {
                //==========LoopScrollRect==========
                if (m_ContentBounds.size.x > 0 && totalCount > 0)
                {
                    float elementSize = (m_ContentBounds.size.x - contentSpacing * (CurrentLines - 1)) / CurrentLines;
                    float totalSize = elementSize * TotalLines + contentSpacing * (TotalLines - 1);
                    m_HorizontalScrollbar.size = Mathf.Clamp01((m_ViewBounds.size.x - Mathf.Abs(offset.x)) / totalSize);
                }
                //==========LoopScrollRect==========
                else
                    m_HorizontalScrollbar.size = 1;

                m_HorizontalScrollbar.value = horizontalNormalizedPosition;
            }

            if (m_VerticalScrollbar)
            {
                //==========LoopScrollRect==========
                if (m_ContentBounds.size.y > 0 && totalCount > 0)
                {
                    float elementSize = (m_ContentBounds.size.y - contentSpacing * (CurrentLines - 1)) / CurrentLines;
                    float totalSize = elementSize * TotalLines + contentSpacing * (TotalLines - 1);
                    m_VerticalScrollbar.size = Mathf.Clamp01((m_ViewBounds.size.y - Mathf.Abs(offset.y)) / totalSize);
                }
                //==========LoopScrollRect==========
                else
                    m_VerticalScrollbar.size = 1;

                m_VerticalScrollbar.value = verticalNormalizedPosition;
            }
        }

        public Vector2 normalizedPosition
        {
            get
            {
                return new Vector2(horizontalNormalizedPosition, verticalNormalizedPosition);
            }
            set
            {
                SetNormalizedPosition(value.x, 0);
                SetNormalizedPosition(value.y, 1);
            }
        }

        public float horizontalNormalizedPosition
        {
            get
            {
                UpdateBounds();
                //==========LoopScrollRect==========
                if (totalCount > 0 && itemTypeEnd > itemTypeStart)
                {
                    float elementSize = (m_ContentBounds.size.x - contentSpacing * (CurrentLines - 1)) / CurrentLines;
                    float totalSize = elementSize * TotalLines + contentSpacing * (TotalLines - 1);
                    float offset = m_ContentBounds.min.x - elementSize * StartLine - contentSpacing * StartLine;

                    if (totalSize <= m_ViewBounds.size.x)
                        return (m_ViewBounds.min.x > offset) ? 1 : 0;
                    return (m_ViewBounds.min.x - offset) / (totalSize - m_ViewBounds.size.x);
                }
                else
                    return 0.5f;
                //==========LoopScrollRect==========
            }
            set
            {
                SetNormalizedPosition(value, 0);
            }
        }

        public float verticalNormalizedPosition
        {
            get
            {
                UpdateBounds();
                //==========LoopScrollRect==========
                if (totalCount > 0 && itemTypeEnd > itemTypeStart)
                {
                    float elementSize = (m_ContentBounds.size.y - contentSpacing * (CurrentLines - 1)) / CurrentLines;
                    float totalSize = elementSize * TotalLines + contentSpacing * (TotalLines - 1);
                    float offset = m_ContentBounds.max.y + elementSize * StartLine + contentSpacing * StartLine;

                    if (totalSize <= m_ViewBounds.size.y)
                        return (offset > m_ViewBounds.max.y) ? 1 : 0;
                    return (offset - m_ViewBounds.max.y) / (totalSize - m_ViewBounds.size.y);
                }
                else
                    return 0.5f;
                //==========LoopScrollRect==========
            }
            set
            {
                SetNormalizedPosition(value, 1);
            }
        }

        private void SetHorizontalNormalizedPosition(float value) { SetNormalizedPosition(value, 0); }
        private void SetVerticalNormalizedPosition(float value) { SetNormalizedPosition(value, 1); }

        private void SetNormalizedPosition(float value, int axis)
        {
            //==========LoopScrollRect==========
            if (totalCount <= 0 || itemTypeEnd <= itemTypeStart)
                return;
            //==========LoopScrollRect==========

            EnsureLayoutHasRebuilt();
            UpdateBounds();

            //==========LoopScrollRect==========
            Vector3 localPosition = m_Content.localPosition;
            float newLocalPosition = localPosition[axis];
            if (axis == 0)
            {
                float elementSize = (m_ContentBounds.size.x - contentSpacing * (CurrentLines - 1)) / CurrentLines;
                float totalSize = elementSize * TotalLines + contentSpacing * (TotalLines - 1);
                float offset = m_ContentBounds.min.x - elementSize * StartLine - contentSpacing * StartLine;

                newLocalPosition += m_ViewBounds.min.x - value * (totalSize - m_ViewBounds.size[axis]) - offset;
            }
            else if (axis == 1)
            {
                float elementSize = (m_ContentBounds.size.y - contentSpacing * (CurrentLines - 1)) / CurrentLines;
                float totalSize = elementSize * TotalLines + contentSpacing * (TotalLines - 1);
                float offset = m_ContentBounds.max.y + elementSize * StartLine + contentSpacing * StartLine;

                newLocalPosition -= offset - value * (totalSize - m_ViewBounds.size.y) - m_ViewBounds.max.y;
            }
            //==========LoopScrollRect==========

            if (Mathf.Abs(localPosition[axis] - newLocalPosition) > 0.01f)
            {
                localPosition[axis] = newLocalPosition;
                m_Content.localPosition = localPosition;
                m_Velocity[axis] = 0;
                UpdateBounds(true);
            }
        }

        private static float RubberDelta(float overStretching, float viewSize)
        {
            return (1 - (1 / ((Mathf.Abs(overStretching) * 0.55f / viewSize) + 1))) * viewSize * Mathf.Sign(overStretching);
        }

        protected override void OnRectTransformDimensionsChange()
        {
            SetDirty();
        }

        private bool hScrollingNeeded
        {
            get
            {
                if (Application.isPlaying)
                    return m_ContentBounds.size.x > m_ViewBounds.size.x + 0.01f;
                return true;
            }
        }
        private bool vScrollingNeeded
        {
            get
            {
                if (Application.isPlaying)
                    return m_ContentBounds.size.y > m_ViewBounds.size.y + 0.01f;
                return true;
            }
        }

        public virtual void CalculateLayoutInputHorizontal() { }
        public virtual void CalculateLayoutInputVertical() { }

        public virtual float minWidth { get { return -1; } }
        public virtual float preferredWidth { get { return -1; } }
        public virtual float flexibleWidth { get; private set; }

        public virtual float minHeight { get { return -1; } }
        public virtual float preferredHeight { get { return -1; } }
        public virtual float flexibleHeight { get { return -1; } }

        public virtual int layoutPriority { get { return -1; } }

        public virtual void SetLayoutHorizontal()
        {
            m_Tracker.Clear();

            if (m_HSliderExpand || m_VSliderExpand)
            {
                m_Tracker.Add(this, viewRect,
                    DrivenTransformProperties.Anchors |
                    DrivenTransformProperties.SizeDelta |
                    DrivenTransformProperties.AnchoredPosition);

                // Make view full size to see if content fits.
                viewRect.anchorMin = Vector2.zero;
                viewRect.anchorMax = Vector2.one;
                viewRect.sizeDelta = Vector2.zero;
                viewRect.anchoredPosition = Vector2.zero;

                // Recalculate content layout with this size to see if it fits when there are no scrollbars.
                LayoutRebuilder.ForceRebuildLayoutImmediate(content);
                m_ViewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
                m_ContentBounds = GetBounds();
            }

            // If it doesn't fit vertically, enable vertical scrollbar and shrink view horizontally to make room for it.
            if (m_VSliderExpand && vScrollingNeeded)
            {
                viewRect.sizeDelta = new Vector2(-(m_VSliderWidth + m_VerticalScrollbarSpacing), viewRect.sizeDelta.y);

                // Recalculate content layout with this size to see if it fits vertically
                // when there is a vertical scrollbar (which may reflowed the content to make it taller).
                LayoutRebuilder.ForceRebuildLayoutImmediate(content);
                m_ViewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
                m_ContentBounds = GetBounds();
            }

            // If it doesn't fit horizontally, enable horizontal scrollbar and shrink view vertically to make room for it.
            if (m_HSliderExpand && hScrollingNeeded)
            {
                viewRect.sizeDelta = new Vector2(viewRect.sizeDelta.x, -(m_HSliderHeight + m_HorizontalScrollbarSpacing));
                m_ViewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
                m_ContentBounds = GetBounds();
            }

            // If the vertical slider didn't kick in the first time, and the horizontal one did,
            // we need to check again if the vertical slider now needs to kick in.
            // If it doesn't fit vertically, enable vertical scrollbar and shrink view horizontally to make room for it.
            if (m_VSliderExpand && vScrollingNeeded && viewRect.sizeDelta.x == 0 && viewRect.sizeDelta.y < 0)
            {
                viewRect.sizeDelta = new Vector2(-(m_VSliderWidth + m_VerticalScrollbarSpacing), viewRect.sizeDelta.y);
            }
        }

        public virtual void SetLayoutVertical()
        {
            UpdateScrollbarLayout();
            m_ViewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
            m_ContentBounds = GetBounds();
        }

        void UpdateScrollbarVisibility()
        {
            if (m_VerticalScrollbar && m_VerticalScrollbarVisibility != ScrollbarVisibility.Permanent && m_VerticalScrollbar.gameObject.activeSelf != vScrollingNeeded)
                m_VerticalScrollbar.gameObject.SetActive(vScrollingNeeded);

            if (m_HorizontalScrollbar && m_HorizontalScrollbarVisibility != ScrollbarVisibility.Permanent && m_HorizontalScrollbar.gameObject.activeSelf != hScrollingNeeded)
                m_HorizontalScrollbar.gameObject.SetActive(hScrollingNeeded);
        }

        void UpdateScrollbarLayout()
        {
            if (m_VSliderExpand && m_HorizontalScrollbar)
            {
                m_Tracker.Add(this, m_HorizontalScrollbarRect,
                              DrivenTransformProperties.AnchorMinX |
                              DrivenTransformProperties.AnchorMaxX |
                              DrivenTransformProperties.SizeDeltaX |
                              DrivenTransformProperties.AnchoredPositionX);
                m_HorizontalScrollbarRect.anchorMin = new Vector2(0, m_HorizontalScrollbarRect.anchorMin.y);
                m_HorizontalScrollbarRect.anchorMax = new Vector2(1, m_HorizontalScrollbarRect.anchorMax.y);
                m_HorizontalScrollbarRect.anchoredPosition = new Vector2(0, m_HorizontalScrollbarRect.anchoredPosition.y);
                if (vScrollingNeeded)
                    m_HorizontalScrollbarRect.sizeDelta = new Vector2(-(m_VSliderWidth + m_VerticalScrollbarSpacing), m_HorizontalScrollbarRect.sizeDelta.y);
                else
                    m_HorizontalScrollbarRect.sizeDelta = new Vector2(0, m_HorizontalScrollbarRect.sizeDelta.y);
            }

            if (m_HSliderExpand && m_VerticalScrollbar)
            {
                m_Tracker.Add(this, m_VerticalScrollbarRect,
                              DrivenTransformProperties.AnchorMinY |
                              DrivenTransformProperties.AnchorMaxY |
                              DrivenTransformProperties.SizeDeltaY |
                              DrivenTransformProperties.AnchoredPositionY);
                m_VerticalScrollbarRect.anchorMin = new Vector2(m_VerticalScrollbarRect.anchorMin.x, 0);
                m_VerticalScrollbarRect.anchorMax = new Vector2(m_VerticalScrollbarRect.anchorMax.x, 1);
                m_VerticalScrollbarRect.anchoredPosition = new Vector2(m_VerticalScrollbarRect.anchoredPosition.x, 0);
                if (hScrollingNeeded)
                    m_VerticalScrollbarRect.sizeDelta = new Vector2(m_VerticalScrollbarRect.sizeDelta.x, -(m_HSliderHeight + m_HorizontalScrollbarSpacing));
                else
                    m_VerticalScrollbarRect.sizeDelta = new Vector2(m_VerticalScrollbarRect.sizeDelta.x, 0);
            }
        }

        private void UpdateBounds(bool updateItems = false)
        {
            m_ViewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
            m_ContentBounds = GetBounds();

            if (m_Content == null)
                return;

            // ============LoopScrollRect============
            // Don't do this in Rebuild
            if (Application.isPlaying && updateItems && UpdateItems(m_ViewBounds, m_ContentBounds))
            {
                Canvas.ForceUpdateCanvases();
                m_ContentBounds = GetBounds();
            }
            // ============LoopScrollRect============

            // Make sure content bounds are at least as large as view by adding padding if not.
            // One might think at first that if the content is smaller than the view, scrolling should be allowed.
            // However, that's not how scroll views normally work.
            // Scrolling is *only* possible when content is *larger* than view.
            // We use the pivot of the content rect to decide in which directions the content bounds should be expanded.
            // E.g. if pivot is at top, bounds are expanded downwards.
            // This also works nicely when ContentSizeFitter is used on the content.
            Vector3 contentSize = m_ContentBounds.size;
            Vector3 contentPos = m_ContentBounds.center;
            Vector3 excess = m_ViewBounds.size - contentSize;
            if (excess.x > 0)
            {
                contentPos.x -= excess.x * (m_Content.pivot.x - 0.5f);
                contentSize.x = m_ViewBounds.size.x;
            }
            if (excess.y > 0)
            {
                contentPos.y -= excess.y * (m_Content.pivot.y - 0.5f);
                contentSize.y = m_ViewBounds.size.y;
            }

            m_ContentBounds.size = contentSize;
            m_ContentBounds.center = contentPos;
        }

        private readonly Vector3[] m_Corners = new Vector3[4];
        private Bounds GetBounds()
        {
            if (m_Content == null)
                return new Bounds();

            var vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var vMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            var toLocal = viewRect.worldToLocalMatrix;
            m_Content.GetWorldCorners(m_Corners);
            for (int j = 0; j < 4; j++)
            {
                Vector3 v = toLocal.MultiplyPoint3x4(m_Corners[j]);
                vMin = Vector3.Min(v, vMin);
                vMax = Vector3.Max(v, vMax);
            }

            var bounds = new Bounds(vMin, Vector3.zero);
            bounds.Encapsulate(vMax);
            return bounds;
        }

        private Bounds GetBounds4Item(int index)
        {
            if (m_Content == null)
                return new Bounds();

            var vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var vMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            var toLocal = viewRect.worldToLocalMatrix;
            int offset = index - itemTypeStart;
            if (offset < 0 || offset >= m_Content.childCount)
                return new Bounds();
            var rt = m_Content.GetChild(offset) as RectTransform;
            if (rt == null)
                return new Bounds();
            rt.GetWorldCorners(m_Corners);
            for (int j = 0; j < 4; j++)
            {
                Vector3 v = toLocal.MultiplyPoint3x4(m_Corners[j]);
                vMin = Vector3.Min(v, vMin);
                vMax = Vector3.Max(v, vMax);
            }

            var bounds = new Bounds(vMin, Vector3.zero);
            bounds.Encapsulate(vMax);
            return bounds;
        }

        private Vector2 CalculateOffset(Vector2 delta)
        {
            Vector2 offset = Vector2.zero;
            if (m_MovementType == MovementType.Unrestricted)
                return offset;
            if (m_MovementType == MovementType.Clamped)
            {
                if (totalCount < 0)
                    return offset;
                if (GetDimension(delta) < 0 && itemTypeStart > 0)
                    return offset;
                if (GetDimension(delta) > 0 && itemTypeEnd < totalCount)
                    return offset;
            }

            Vector2 min = m_ContentBounds.min;
            Vector2 max = m_ContentBounds.max;

            if (m_Horizontal)
            {
                min.x += delta.x;
                max.x += delta.x;
                if (min.x > m_ViewBounds.min.x)
                    offset.x = m_ViewBounds.min.x - min.x;
                else if (max.x < m_ViewBounds.max.x)
                    offset.x = m_ViewBounds.max.x - max.x;
            }

            if (m_Vertical)
            {
                min.y += delta.y;
                max.y += delta.y;
                if (max.y < m_ViewBounds.max.y)
                    offset.y = m_ViewBounds.max.y - max.y;
                else if (min.y > m_ViewBounds.min.y)
                    offset.y = m_ViewBounds.min.y - min.y;
            }

            return offset;
        }

        protected void SetDirty()
        {
            if (!IsActive())
                return;

            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }

        protected void SetDirtyCaching()
        {
            if (!IsActive())
                return;

            CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetDirtyCaching();
        }
#endif
    }
}