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

        public float GetItemsSize(int itemStart, int itemEnd)
        {
            if (itemEnd <= itemStart) return 0;
            float sum = 0;
            for (int i = itemStart; i < itemEnd; i++)
            {
                Vector2 size = m_LoopListBank.GetCellPreferredSize(i);
                if (m_LoopScrollRect.horizontal)
                    sum += size.x;
                else
                    sum += size.y;
            }
            return sum;
        }
    }
}