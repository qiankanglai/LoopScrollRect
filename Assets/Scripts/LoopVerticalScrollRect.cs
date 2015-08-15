using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoopVerticalScrollRect : LoopScrollRect
{
	protected override float GetSize (RectTransform item)
	{
        return item.GetComponent<LayoutElement>().preferredHeight + contentSpacing;
	}

	protected override float GetDimension (Vector2 vector)
	{
		return vector.y;
	}

    protected override float GetDimension(Vector3 vector)
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
        if (content.pivot.y != 1)
        {
            Debug.LogError("[LoopHorizontalScrollRect] Content pivot y should be one");
        }
    }
}
