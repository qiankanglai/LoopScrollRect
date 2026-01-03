using UnityEngine;
using System.Collections;

namespace UnityEngine.UI
{
    // optional class for better scroll support
    public interface LoopScrollSizeHelper
    {
        // calculate size for elements within [itemStart, itemEnd)
        float GetItemsSize(int itemStart, int itemEnd);
    }
}
