using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.UI
{
    public abstract class LoopScrollRectMulti : LoopScrollRectBase
    {
        [HideInInspector]
        [NonSerialized]
        public LoopScrollMultiDataSource dataSource = null;
        
        protected override void ProvideData(Transform transform, int index)
        {
            dataSource.ProvideData(transform, index);
        }
        
        // Multi Data Source cannot support TempPool
        protected override RectTransform GetFromTempPool(int itemIdx)
        {
            RectTransform nextItem = prefabSource.GetObject(itemIdx).transform as RectTransform;
            nextItem.transform.SetParent(content, false);
            nextItem.gameObject.SetActive(true);

            ProvideData(nextItem, itemIdx);
            return nextItem;
        }

        protected override void ReturnToTempPool(bool fromStart, int count)
        {
            Debug.Assert(content.childCount >= count);
            if (fromStart)
            {
                for (int i = count - 1; i >= 0; i--)
                {
                    prefabSource.ReturnObject(content.GetChild(i));
                }
            }
            else
            {
                int t = content.childCount - count;
                for (int i = content.childCount - 1; i >= t; i--)
                {
                    prefabSource.ReturnObject(content.GetChild(i));
                }
            }
        }

        protected override void ClearTempPool()
        {
        }
    }
}