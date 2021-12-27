using UnityEngine;
using System.Collections;

namespace UnityEngine.UI
{
    public interface LoopScrollSizeHelper
    {
        Vector2 GetItemsSize(int itemsCount);
    }
}
