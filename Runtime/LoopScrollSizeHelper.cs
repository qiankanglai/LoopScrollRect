using UnityEngine;
using System.Collections;

namespace UnityEngine.UI
{
    // optional class for better scroll support
    public interface LoopScrollSizeHelper
    {
        Vector2 GetItemsSize(int itemsCount);
    }

    public static class LoopScrollSizeUtils
    {
        public static float GetPreferredHeight(RectTransform item)
        {
            ILayoutElement minLayoutElement;
            ILayoutElement maxLayoutElement;
            var minHeight = LayoutUtility.GetLayoutProperty(item, e => e.minHeight, 0, out minLayoutElement);
            var preferredHeight = LayoutUtility.GetLayoutProperty(item, e => e.preferredHeight, 0, out maxLayoutElement);
            var result = Mathf.Max(minHeight, preferredHeight);
            if (maxLayoutElement == null && minLayoutElement == null)
            {
                result = item.rect.height;
            }

            return result;
        }
        
        public static float GetPreferredWidth(RectTransform item)
        {
            ILayoutElement minLayoutElement;
            ILayoutElement maxLayoutElement;
            var minWidth = LayoutUtility.GetLayoutProperty(item, e => e.minWidth, 0, out minLayoutElement);
            var preferredWidth = LayoutUtility.GetLayoutProperty(item, e => e.preferredWidth, 0, out maxLayoutElement);
            var result = Mathf.Max(minWidth, preferredWidth);
            if (maxLayoutElement == null && minLayoutElement == null)
            {
                result = item.rect.width;
            }

            return result;
        }
    }
}
