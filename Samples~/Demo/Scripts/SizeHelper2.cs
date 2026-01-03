using UnityEngine;
using UnityEngine.UI;

public class SizeHelper2 : MonoBehaviour, LoopScrollSizeHelper
{
    // Start is called before the first frame update
    void Start()
    {
        var ls = GetComponent<LoopScrollRect>();
        ls.sizeHelper = this;
    }
    
    public float GetItemsSize(int itemStart, int itemEnd)
    {
        if (itemEnd <= itemStart) return 0;
        int count = ScrollIndexCallback2.randomWidths.Length;
        float sum = 0;
        for (int i = itemStart; i < itemEnd; i++)
        {
            sum += ScrollIndexCallback2.randomWidths[Mathf.Abs(i) % count];
        }
        return sum;
    }
}
