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
            if (fromStart)
            {
                for (int i = 0; i < count; i++)
                {
                    prefabSource.ReturnObject(content.GetChild(0));
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    prefabSource.ReturnObject(content.GetChild(content.childCount - 1));
                }
            }
        }

        protected override void ClearTempPool()
        {
        }
    }
}