using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

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
            if (deletedItemTypeStart > 0)
            {
                deletedItemTypeStart--;
                nextItem = content.GetChild(0) as RectTransform;
                nextItem.SetSiblingIndex(itemIdx - itemTypeStart + deletedItemTypeStart);
            }
            else if (deletedItemTypeEnd > 0)
            {
                deletedItemTypeEnd--;
                nextItem = content.GetChild(content.childCount - 1) as RectTransform;
                nextItem.SetSiblingIndex(itemIdx - itemTypeStart + deletedItemTypeStart);
            }
            else
            {
                nextItem = prefabSource.GetObject(itemIdx).transform as RectTransform;
                nextItem.transform.SetParent(content, false);
                nextItem.gameObject.SetActive(true);
            }
            ProvideData(nextItem, itemIdx);
            return nextItem;
        }

        protected override void ReturnToTempPool(bool fromStart, int count)
        {
            if (fromStart)
                deletedItemTypeStart += count;
            else
                deletedItemTypeEnd += count;
        }

        protected override void ClearTempPool()
        {
            Debug.Assert(content.childCount >= deletedItemTypeStart + deletedItemTypeEnd);
            if (deletedItemTypeStart > 0)
            {
                for (int i = deletedItemTypeStart - 1; i >= 0; i--)
                {
                    prefabSource.ReturnObject(content.GetChild(i));
                }
                deletedItemTypeStart = 0;
            }
            if (deletedItemTypeEnd > 0)
            {
                int t = content.childCount - deletedItemTypeEnd;
                for (int i = content.childCount - 1; i >= t; i--)
                {
                    prefabSource.ReturnObject(content.GetChild(i));
                }
                deletedItemTypeEnd = 0;
            }
        }
    }
}