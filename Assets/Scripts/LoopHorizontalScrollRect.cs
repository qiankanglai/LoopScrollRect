using UnityEngine;
using System.Collections;

using UnityEngine.UI;

public class LoopHorizontalScrollRect : LoopScrollRect
{
	protected override float GetSize (RectTransform item)
	{
        return item.GetComponent<LayoutElement>().preferredWidth + contentSpacing;
	}

	protected override float GetDimension (Vector2 vector)
	{
		return vector.x;
	}

    protected override float GetDimension(Vector3 vector)
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
        if(content.pivot.x != 0)
        {
            Debug.LogError("[LoopHorizontalScrollRect] Content pivot x should be zero");
        }
	}
}
