using UnityEngine;
using UnityEngine.UI;

namespace Demo
{
    public class SizeHelper : MonoBehaviour, LoopScrollSizeHelper
    {
        [HideInInspector]
        public LoopScrollRectMulti m_LoopScrollRect;

        [HideInInspector]
        public LoopListBankBase m_LoopListBank;

        // Start is called before the first frame update
        void Start()
        {
            m_LoopScrollRect = GetComponent<LoopScrollRectMulti>();
            m_LoopScrollRect.sizeHelper = this;

            m_LoopListBank = GetComponent<LoopListBankBase>();
        }

        public Vector2 GetItemsSize(int itemsCount)
        {
            if (itemsCount <= 0) return new Vector2();
            int count = m_LoopListBank.GetListLength();
            Vector2 sum = new Vector2();
            for (int i = 0; i < count; i++)
            {
                if (itemsCount <= i) break;
                int t = (itemsCount - 1 - i) / count + 1;
                sum += t * m_LoopListBank.GetCellPreferredSize(i);
            }
            return sum;
        }
    }
}