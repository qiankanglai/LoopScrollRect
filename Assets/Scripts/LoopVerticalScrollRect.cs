using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoopVerticalScrollRect : LoopScrollRect
{
	protected override float GetSize (RectTransform item)
	{
        return LayoutUtility.GetPreferredHeight(item) + contentSpacing;
    }

	protected override float GetDimension (Vector2 vector)
	{
		return vector.y;
	}

	protected override Vector2 GetVector (float value)
	{
		return new Vector2 (0, value);
	}

    protected override void Awake()
    {
        base.Awake();
        directionSign = -1;

        GridLayoutGroup layout = content.GetComponent<GridLayoutGroup>();
        if (layout != null && layout.constraint != GridLayoutGroup.Constraint.FixedColumnCount)
        {
            Debug.LogError("[LoopHorizontalScrollRect] unsupported GridLayoutGroup constraint");
        }
    }

    protected override bool UpdateItems(Bounds viewBounds, Bounds contentBounds)
    {
        bool changed = false;
        if (
#if !INFINITE
            (itemTypeEnd < totalCount) &&
#endif
            viewBounds.min.y < contentBounds.min.y)
        {
            float size = NewItemAtEnd();
            if (threshold < size)
            {
                threshold = size * 1.1f;
            }
            changed = true;
        }
        else if(viewBounds.min.y > contentBounds.min.y + threshold)
        {
            DeleteItemAtEnd();
            changed = true;
        }
        if (
#if !INFINITE
            (itemTypeStart >= contentConstraintCount) &&
#endif
            viewBounds.max.y > contentBounds.max.y)
        {
            float size = NewItemAtStart();
            if (threshold < size)
            {
                threshold = size * 1.1f;
            }
            changed = true;
        }
        else if (
#if !INFINITE
                itemTypeEnd < totalCount - 1 &&
#endif
                viewBounds.max.y < contentBounds.max.y - threshold)
        {
            DeleteItemAtStart();
            changed = true;
        }
        return changed;
    }
}
