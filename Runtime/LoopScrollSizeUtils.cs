using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.UI
{
    public static class LoopScrollSizeUtils
    {
        public static float GetPreferredHeight(RectTransform item)
        {
            ILayoutElement minLayoutElement;
            ILayoutElement preferredLayoutElement;
            var minHeight = LayoutUtility.GetLayoutProperty(item, e => e.minHeight, 0, out minLayoutElement);
            var preferredHeight = LayoutUtility.GetLayoutProperty(item, e => e.preferredHeight, 0, out preferredLayoutElement);
            var result = Mathf.Max(minHeight, preferredHeight);
            if (preferredLayoutElement == null && minLayoutElement == null)
            {
                result = item.rect.height;
            }

            return result;
        }
        
        public static float GetPreferredWidth(RectTransform item)
        {
            ILayoutElement minLayoutElement;
            ILayoutElement preferredLayoutElement;
            var minWidth = LayoutUtility.GetLayoutProperty(item, e => e.minWidth, 0, out minLayoutElement);
            var preferredWidth = LayoutUtility.GetLayoutProperty(item, e => e.preferredWidth, 0, out preferredLayoutElement);
            var result = Mathf.Max(minWidth, preferredWidth);
            if (preferredLayoutElement == null && minLayoutElement == null)
            {
                result = item.rect.width;
            }

            return result;
        }
    }
}