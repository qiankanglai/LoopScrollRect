using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.UI;

[SelectionBase]
[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
[AddComponentMenu("UI/Effects/Loop Scroll Rect", 16)]
public abstract class LoopScrollRect : UIBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollHandler, ICanvasElement
{
    //==========LoopScrollRect==========
    [HideInInspector]
	public MarchingBytes.EasyObjectPool prefabPool;
	[HideInInspector]
	public string prefabPoolName;
	[HideInInspector]
    public int totalCount, cacheExtendPixels = 5;
    [HideInInspector]
    public bool initInStart = true;

    private int itemTypeStart = 0;
    private int itemTypeEnd = 0;

    private Vector3[] fourCornersArray = new Vector3[4];
    protected abstract float GetSize(RectTransform item);
    protected abstract float GetDimension(Vector2 vector);
    protected abstract float GetDimension(Vector3 vector);
    protected abstract Vector2 GetVector(float value);
    protected int directionSign = 0;

    private float m_ContentSpacing = -1;
    protected float contentSpacing
    {
        get
        {
            if(m_ContentSpacing >= 0)
            {
                return m_ContentSpacing;
            }
            m_ContentSpacing = 0;
            if(content != null)
            {
                HorizontalOrVerticalLayoutGroup layout1 = content.GetComponent<HorizontalOrVerticalLayoutGroup>();
                if (layout1 != null)
                {
                    m_ContentSpacing = layout1.spacing;
                }
                else
                {
                    GridLayoutGroup layout2 = content.GetComponent<GridLayoutGroup>();
                    if (layout2 != null)
                    {
                        m_ContentSpacing = GetDimension(layout2.spacing);
                    }
                }
            }
            return m_ContentSpacing;
        }
    }

    private float m_ContentConstraintCount = 0;
    protected float contentConstraintCount
    {
        get
        {
            if (m_ContentConstraintCount > 0)
            {
                return m_ContentConstraintCount;
            }
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
    //==========LoopScrollRect==========

    public enum MovementType
    {
        Unrestricted, // Unrestricted movement -- can scroll forever
        Elastic, // Restricted but flexible -- can go past the edges, but springs back in place
        Clamped, // Restricted movement where it's not possible to go past the edges
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
        }
    }

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

    protected LoopScrollRect()
    { }

    public virtual void Rebuild(CanvasUpdate executing)
    {
        if (executing != CanvasUpdate.PostLayout)
            return;

        UpdateBounds();
        UpdateScrollbars(Vector2.zero);
        UpdatePrevData();
        m_HasRebuiltLayout = true;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    //==========LoopScrollRect==========
    private bool LoopScollInitialized = false;

    protected override void Start()
    {
        base.Start();
        if (initInStart)
        {
            RefillCells();
        }
    }

    public void ClearCells()
    {
        if (Application.isPlaying)
        {
            LoopScollInitialized = false;
            itemTypeStart = 0;
            itemTypeEnd = 0;
            for (int i = content.childCount - 1; i >= 0; i--)
            {
                prefabPool.ReturnObjectToPool(content.GetChild(i).gameObject);
            }
        }
    }
    public void RefillCells()
    {
        if (Application.isPlaying)
        {
            LoopScollInitialized = true;

            viewRect.GetLocalCorners(fourCornersArray);
            float viewRectSize = GetDimension(fourCornersArray[2]) - GetDimension(fourCornersArray[0]);
            
            float containerSize = 0;
            //Filling up the scrollview with initial items
            while (itemTypeEnd < totalCount && containerSize < viewRectSize)
            {
                containerSize += NewItemAtEnd();
            }
        }
    }
    private float NewItemAtStart()
    {
        if (itemTypeStart - contentConstraintCount < 0)
        {
            Debug.LogWarning("[LoopScrollRect] not enough condidates at start");
            return 0;
        }

        RectTransform newItem = null;
        for (int i = 0; i < contentConstraintCount; i++)
        {
            itemTypeStart--;
            newItem = InstantiateNextItem(itemTypeStart);
            newItem.SetAsFirstSibling();
        }

        float size = GetSize(newItem);
        Vector2 offset = GetVector(size);
        content.localPosition += new Vector3(offset.x, offset.y, 0);
        // adapte to UGUI
        m_PrevPosition += offset;
        m_ContentStartPosition += offset;

        return size;
    }

    private float DeleteItemAtStart()
    {
        RectTransform oldItem = null;
        for (int i = 0; i < contentConstraintCount; i++)
        {
            oldItem = content.GetChild(0) as RectTransform;
            prefabPool.ReturnObjectToPool(oldItem.gameObject);

            itemTypeStart++;
        }

        return GetSize(oldItem);
    }

    private float NewItemAtEnd()
    {
        if (itemTypeEnd >= totalCount)
        {
            Debug.LogWarning("[LoopScrollRect] not enough condidates at end");
            return 0;
        }

        RectTransform newItem = null;
        for (int i = 0; i < contentConstraintCount; i++)
        {
            newItem = InstantiateNextItem(itemTypeEnd);
            itemTypeEnd++;
            if (itemTypeEnd >= totalCount)
            {
                break;
            }
        }

        return GetSize(newItem);
    }

    private float DeleteItemAtEnd()
    {
        RectTransform oldItem = null;
        for (int i = 0; i < contentConstraintCount; i++)
        {
            oldItem = content.GetChild(content.childCount - 1) as RectTransform;
            prefabPool.ReturnObjectToPool(oldItem.gameObject);

            itemTypeEnd--;
            if (itemTypeEnd % contentConstraintCount == 0)
            {
                break;  //just delete the whole row
            }
        }

        return GetSize(oldItem);
    }

    private RectTransform InstantiateNextItem(int itemIdx)
    {
        RectTransform nextItem = prefabPool.GetObjectFromPool(prefabPoolName).GetComponent<RectTransform>();
        nextItem.transform.SetParent(content, false);
        nextItem.gameObject.SetActive(true);
        //nextItem.name = itemIdx.ToString();
        nextItem.SendMessage("ScrollCellIndex", itemIdx);
        return nextItem;
    }

    //==========LoopScrollRect==========

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
            if (offset.x != 0)
                position.x = position.x - RubberDelta(offset.x, m_ViewBounds.size.x);
            if (offset.y != 0)
                position.y = position.y - RubberDelta(offset.y, m_ViewBounds.size.y);
        }

        SetContentAnchoredPosition(position);
    }

    protected virtual void SetContentAnchoredPosition(Vector2 position)
    {
        if (!m_Horizontal)
            position.x = m_Content.anchoredPosition.x;
        if (!m_Vertical)
            position.y = m_Content.anchoredPosition.y;

        if (position != m_Content.anchoredPosition)
        {
            m_Content.anchoredPosition = position;
            UpdateBounds();
        }
    }

    protected virtual void LateUpdate()
    {
        if (!m_Content)
            return;

        //==========LoopScrollRect==========
        //TODO: check anchor
        if (Application.isPlaying && LoopScollInitialized)
        {
            viewRect.GetWorldCorners(fourCornersArray);
            float viewRectEnd = GetDimension(fourCornersArray[3]);
            float viewRectStart = GetDimension(fourCornersArray[1]);

            content.GetWorldCorners(fourCornersArray);
            float contentEnd = GetDimension(fourCornersArray[3]);
            float contentStart = GetDimension(fourCornersArray[1]);
            if (itemTypeEnd < totalCount && contentEnd * directionSign < (viewRectEnd + cacheExtendPixels * directionSign) * directionSign)
            {
                NewItemAtEnd();
            }
            else if (itemTypeStart > contentConstraintCount - 1 && contentStart * directionSign > (viewRectStart - cacheExtendPixels * directionSign) * directionSign)
            {
                NewItemAtStart();
            }
            else if (!m_Dragging)
            {
                if (content.childCount > 0)
                {
                    RectTransform child = content.GetChild(0) as RectTransform;
                    child.GetWorldCorners(fourCornersArray);
                    float childEnd = GetDimension(fourCornersArray[3]);
                    if (childEnd * directionSign < (viewRectStart - cacheExtendPixels * 1.5f * directionSign) * directionSign)
                    {
                        Vector2 offset2 = GetVector(DeleteItemAtStart());
                        content.localPosition -= new Vector3(offset2.x, offset2.y, 0);

                        m_PrevPosition -= offset2;
                        m_ContentStartPosition -= offset2;
                    }
                }
                if (content.childCount > 0)
                {
                    RectTransform child = content.GetChild(content.childCount - 1) as RectTransform;
                    child.GetWorldCorners(fourCornersArray);
                    float childStart = GetDimension(fourCornersArray[1]);
                    if (childStart * directionSign > (viewRectEnd + cacheExtendPixels * 1.5f * directionSign) * directionSign)
                    {
                        DeleteItemAtEnd();
                    }
                }
            }
        }
        //==========LoopScrollRect==========

        EnsureLayoutHasRebuilt();
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
            if (m_ContentBounds.size.x > 0)
                m_HorizontalScrollbar.size = Mathf.Clamp01((m_ViewBounds.size.x - Mathf.Abs(offset.x)) / m_ContentBounds.size.x);
            else
                m_HorizontalScrollbar.size = 1;

            m_HorizontalScrollbar.value = horizontalNormalizedPosition;
        }

        if (m_VerticalScrollbar)
        {
            if (m_ContentBounds.size.y > 0)
                m_VerticalScrollbar.size = Mathf.Clamp01((m_ViewBounds.size.y - Mathf.Abs(offset.y)) / m_ContentBounds.size.y);
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
            if (m_ContentBounds.size.x <= m_ViewBounds.size.x)
                return (m_ViewBounds.min.x > m_ContentBounds.min.x) ? 1 : 0;
            return (m_ViewBounds.min.x - m_ContentBounds.min.x) / (m_ContentBounds.size.x - m_ViewBounds.size.x);
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
            if (m_ContentBounds.size.y <= m_ViewBounds.size.y)
                return (m_ViewBounds.min.y > m_ContentBounds.min.y) ? 1 : 0;
            ;
            return (m_ViewBounds.min.y - m_ContentBounds.min.y) / (m_ContentBounds.size.y - m_ViewBounds.size.y);
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
        EnsureLayoutHasRebuilt();
        UpdateBounds();
        // How much the content is larger than the view.
        float hiddenLength = m_ContentBounds.size[axis] - m_ViewBounds.size[axis];
        // Where the position of the lower left corner of the content bounds should be, in the space of the view.
        float contentBoundsMinPosition = m_ViewBounds.min[axis] - value * hiddenLength;
        // The new content localPosition, in the space of the view.
        float newLocalPosition = m_Content.localPosition[axis] + contentBoundsMinPosition - m_ContentBounds.min[axis];

        Vector3 localPosition = m_Content.localPosition;
        if (Mathf.Abs(localPosition[axis] - newLocalPosition) > 0.01f)
        {
            localPosition[axis] = newLocalPosition;
            m_Content.localPosition = localPosition;
            m_Velocity[axis] = 0;
            UpdateBounds();
        }
    }

    private static float RubberDelta(float overStretching, float viewSize)
    {
        return (1 - (1 / ((Mathf.Abs(overStretching) * 0.55f / viewSize) + 1))) * viewSize * Mathf.Sign(overStretching);
    }

    private void UpdateBounds()
    {
        m_ViewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);  
        m_ContentBounds = GetBounds();

        if (m_Content == null)
            return;

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

    private Vector2 CalculateOffset(Vector2 delta)
    {
        Vector2 offset = Vector2.zero;
        if (m_MovementType == MovementType.Unrestricted)
            return offset;

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
}
