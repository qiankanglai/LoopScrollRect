using qiankanglai.LoopScrollRect;
using UnityEngine;
using UnityEngine.UI;

public class SizeHelper : MonoBehaviour, LoopScrollSizeHelper
{
    [HideInInspector]
    public LoopScrollRect m_LoopScrollRect;

    [HideInInspector]
    public BaseListBank m_BaseListBank;

    // Start is called before the first frame update
    void Start()
    {
        m_LoopScrollRect = GetComponent<LoopScrollRect>();
        m_LoopScrollRect.sizeHelper = this;

        m_BaseListBank = GetComponent<BaseListBank>();
    }

    public Vector2 GetItemsSize(int itemsCount)
    {
        if (itemsCount <= 0) return new Vector2();
        int count = m_BaseListBank.GetListLength();
        Vector2 sum = new Vector2();
        for (int i = 0; i < count; i++)
        {
            if (itemsCount <= i) break;
            int t = (itemsCount - 1 - i) / count + 1;
            if(m_LoopScrollRect.horizontal)
            {
                sum.x += t * m_BaseListBank.GetCellPreferredSize(i).x;
            }
            if (m_LoopScrollRect.vertical)
            {
                sum.y += t * m_BaseListBank.GetCellPreferredSize(i).y;
            }
        }

        // Check and Set base PreferredSize
        if (m_LoopScrollRect.horizontal)
        {
            if(sum.y <= 0)
            {
                sum.y = m_BaseListBank.GetCellPreferredSize(0).y;
            }
        }
        if (m_LoopScrollRect.vertical)
        {
            if (sum.x <= 0)
            {
                sum.x = m_BaseListBank.GetCellPreferredSize(0).x;
            }
        }
        return sum;
    }
}
