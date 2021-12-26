using System.Collections;
using System.Collections.Generic;
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
    
    public float GetItemsSize(int itemsCount)
    {
        if (itemsCount <= 0) return 0;
        int count = ScrollIndexCallback2.randomWidths.Length;
        float sum = 0;
        for (int i = 0; i < count; i++)
        {
            if (itemsCount <= i) break;
            int t = (itemsCount - 1 - i) / count + 1;
            sum += t * ScrollIndexCallback2.randomWidths[i];
        }
        return sum;
    }
}
