using UnityEngine;
using UnityEngine.UI;

namespace qiankanglai.LoopScrollRectManager
{
    public class SizeHelper : MonoBehaviour, LoopScrollSizeHelper
    {
        [HideInInspector]
        public LoopScrollRect m_LoopScrollRect;

        [HideInInspector]
        public LoopListBankBase m_BaseListBank;

        // Start is called before the first frame update
        void Start()
        {
            m_LoopScrollRect = GetComponent<LoopScrollRect>();
            m_LoopScrollRect.sizeHelper = this;

            m_BaseListBank = GetComponent<LoopListBankBase>();
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
                sum += t * m_BaseListBank.GetCellPreferredSize(i);
            }
            return sum;
        }
    }
}