using UnityEngine;
using System.Collections;

using UnityEngine.UI;

public class LoopHorizontalScrollRect : LoopScrollRect
{
	protected override float GetSize (RectTransform item)
	{
        return LayoutUtility.GetPreferredWidth(item) + contentSpacing;
	}

	protected override float GetDimension (Vector2 vector)
	{
		return vector.x;
	}

	protected override Vector2 GetVector (float value)
	{
		return new Vector2 (-value, 0);
	}

	protected override void Awake ()
	{
        base.Awake();
		directionSign = 1;

        GridLayoutGroup layout = content.GetComponent<GridLayoutGroup>();
        if(layout != null && layout.constraint != GridLayoutGroup.Constraint.FixedRowCount)
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
            viewBounds.max.x > contentBounds.max.x)
        {
            float size = NewItemAtEnd();
            if(threshold < size)
            {
                // Preventing new and delete repeatly...
                threshold = size * 1.1f;
            }
            changed = true;
        }
        else if(viewBounds.max.x < contentBounds.max.x - threshold)
        {
            DeleteItemAtEnd();
            changed = true;
        }
        
        if (
#if !INFINITE
            (itemTypeStart >= contentConstraintCount) &&
#endif
            viewBounds.min.x < contentBounds.min.x)
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
                viewBounds.min.x > contentBounds.min.x + threshold)
        {
            DeleteItemAtStart();
            changed = true;
        }
        return changed;
    }
}
