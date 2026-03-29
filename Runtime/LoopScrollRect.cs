using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.UI
{
    public abstract class LoopScrollRect : LoopScrollRectBase
    {
        [HideInInspector]
        [NonSerialized]
        public LoopScrollDataSource dataSource = null;
        
        protected override void ProvideData(Transform transform, int index)
        {
            dataSource.ProvideData(transform, index);
        }
        
        protected override RectTransform GetFromTempPool(int itemIdx)
        {
            RectTransform nextItem = null;
            bool existingData = false;
            if (deletedItemsAtStart.TryGetValue(itemIdx, out nextItem))
            {
                deletedItemsAtStart.Remove(itemIdx);
                deletedItemTypeStart--;
                existingData = true;
            }
            else if (deletedItemsAtEnd.TryGetValue(itemIdx, out nextItem))
            {
                deletedItemsAtEnd.Remove(itemIdx);
                deletedItemTypeEnd--;
                existingData = true;
            }
            else if (deletedItemsAtStart.Count > 0)
            {
                var first = deletedItemsAtStart.First();
                deletedItemsAtStart.Remove(first.Key);
                deletedItemTypeStart--;
                nextItem = first.Value;
            }
            else if (deletedItemTypeEnd > 0)
            {
                var first = deletedItemsAtEnd.First();
                deletedItemsAtEnd.Remove(first.Key);
                deletedItemTypeEnd--;
                nextItem = first.Value;
            }
            else
            {
                nextItem = prefabSource.GetObject(itemIdx).transform as RectTransform;
                nextItem.transform.SetParent(m_Content, false);
                nextItem.gameObject.SetActive(true);
            }
            if (!existingData)
            {
                ProvideData(nextItem, itemIdx);
            }
            return nextItem;
        }

        protected Dictionary<int, RectTransform> deletedItemsAtStart = new Dictionary<int, RectTransform>();
        protected Dictionary<int, RectTransform> deletedItemsAtEnd = new Dictionary<int, RectTransform>();
        protected override void ReturnToTempPool(bool fromStart, int count)
        {
            if (fromStart)
            {
                for (int i = 0; i < count; i++)
                {
                    deletedItemsAtStart.Add(itemTypeStart + i, m_Content.GetChild(deletedItemTypeStart + i) as RectTransform);
                }
                deletedItemTypeStart += count;
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    deletedItemsAtEnd.Add(itemTypeEnd - 1 - i, m_Content.GetChild(m_Content.childCount - 1 - deletedItemTypeEnd - i) as RectTransform);
                }
                deletedItemTypeEnd += count;
            }
        }

        protected override void ClearTempPool()
        {
            Debug.Assert(m_Content.childCount >= deletedItemTypeStart + deletedItemTypeEnd);
            deletedItemsAtStart.Clear();
            deletedItemsAtEnd.Clear();
            if (deletedItemTypeStart > 0)
            {
                for (int i = deletedItemTypeStart - 1; i >= 0; i--)
                {
                    prefabSource.ReturnObject(m_Content.GetChild(i));
                }
                deletedItemTypeStart = 0;
            }
            if (deletedItemTypeEnd > 0)
            {
                int t = m_Content.childCount - deletedItemTypeEnd;
                for (int i = m_Content.childCount - 1; i >= t; i--)
                {
                    prefabSource.ReturnObject(m_Content.GetChild(i));
                }
                deletedItemTypeEnd = 0;
            }
        }
    }
}