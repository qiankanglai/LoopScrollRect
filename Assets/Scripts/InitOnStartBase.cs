using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace qiankanglai.LoopScrollRectManager
{
    [RequireComponent(typeof(UnityEngine.UI.LoopScrollRect))]
    [DisallowMultipleComponent]
    public class InitOnStartBase : MonoBehaviour, LoopScrollPrefabSource, LoopScrollDataSource
    {
        [HideInInspector]
        public LoopScrollRect m_LoopScrollRect;
        [HideInInspector]
        public LoopListBankBase m_ListBank;

        // Is Use MulitiPrefab
        public bool m_IsUseMultiPrefabs = false;
        // Cell Prefab
        public GameObject m_Item;
        // Cell MulitiPrefab
        public List<GameObject> m_ItemList = new List<GameObject>();

        protected virtual void Start()
        {
            m_ListBank = GetComponent<LoopListBankBase>();

            m_LoopScrollRect = GetComponent<LoopScrollRect>();
            m_LoopScrollRect.prefabSource = this;
            m_LoopScrollRect.dataSource = this;
            m_LoopScrollRect.totalCount = m_ListBank.GetListLength();
            m_LoopScrollRect.EnableTempPool(!m_IsUseMultiPrefabs);
            m_LoopScrollRect.RefillCells();
        }

        // Implement your own Cache Pool here. The following is just for example.
        Stack<Transform> pool = new Stack<Transform>();
        Dictionary<string, Stack<Transform>> m_Pool_Type = new Dictionary<string, Stack<Transform>>();
        public virtual GameObject GetObject(int index)
        {
            Transform candidate = null;
            // Is Use MulitiPrefab
            if (!m_IsUseMultiPrefabs)
            {
                if (pool.Count == 0)
                {
                    candidate = Instantiate(m_Item.transform);
                }
                else
                {
                    candidate = pool.Pop();
                }

                // One Cell Prefab, Set PreferredSize as runtime.
                ScrollIndexCallbackBase TempScrollIndexCallbackBase = candidate.GetComponent<ScrollIndexCallbackBase>();
                if (null != TempScrollIndexCallbackBase)
                {
                    if (m_LoopScrollRect.horizontal)
                    {
                        float RandomWidth = m_ListBank.GetCellPreferredSize(index).x;
                        TempScrollIndexCallbackBase.SetLayoutElementPreferredWidth(RandomWidth);
                    }

                    if (m_LoopScrollRect.vertical)
                    {
                        float RandomHeight = m_ListBank.GetCellPreferredSize(index).y;
                        TempScrollIndexCallbackBase.SetLayoutElementPreferredHeight(RandomHeight);
                    }
                }
            }
            else
            {
                // Cell MulitiPrefab, Get Cell Preferred Type by custom data
                int CellTypeIndex = m_ListBank.GetCellPreferredTypeIndex(index);
                if (m_ItemList.Count <= CellTypeIndex)
                {
                    Debug.LogWarningFormat("TempPrefab is null! CellTypeIndex: {0}", CellTypeIndex);
                    return null;
                }
                var TempPrefab = m_ItemList[CellTypeIndex];

                Stack<Transform> TempStack = null;
                if (!m_Pool_Type.TryGetValue(TempPrefab.name, out TempStack))
                {
                    TempStack = new Stack<Transform>();
                    m_Pool_Type.Add(TempPrefab.name, TempStack);
                }

                if (TempStack.Count == 0)
                {
                    candidate = Instantiate(TempPrefab).GetComponent<Transform>();
                    ScrollIndexCallbackBase TempScrollIndexCallbackBase = candidate.GetComponent<ScrollIndexCallbackBase>();
                    if (null != TempScrollIndexCallbackBase)
                    {
                        TempScrollIndexCallbackBase.SetPrefabName(TempPrefab.name);
                    }
                }
                else
                {
                    candidate = TempStack.Pop();
                    candidate.gameObject.SetActive(true);
                }
            }

            return candidate.gameObject;
        }

        public virtual void ReturnObject(Transform trans)
        {
            trans.SendMessage("ScrollCellReturn", SendMessageOptions.DontRequireReceiver);
            trans.gameObject.SetActive(false);
            trans.SetParent(transform, false);
            // Is Use MulitiPrefab
            if (!m_IsUseMultiPrefabs)
            {
                pool.Push(trans);
            }
            else
            {
                // Use PrefabName as Key for Pool Manager
                ScrollIndexCallbackBase TempScrollIndexCallbackBase = trans.GetComponent<ScrollIndexCallbackBase>();
                if (null == TempScrollIndexCallbackBase)
                {
                    // Use `DestroyImmediate` here if you don't need Pool
                    DestroyImmediate(trans.gameObject);
                    return;
                }

                Stack<Transform> TempStack = null;
                if (m_Pool_Type.TryGetValue(TempScrollIndexCallbackBase.GetPrefabName(), out TempStack))
                {
                    TempStack.Push(trans);
                }
                else
                {
                    TempStack = new Stack<Transform>();
                    TempStack.Push(trans);

                    m_Pool_Type.Add(TempScrollIndexCallbackBase.GetPrefabName(), TempStack);
                }
            }
        }

        public virtual void ProvideData(Transform transform, int idx)
        {
            //transform.SendMessage("ScrollCellIndex", idx);

            // Use direct call for better performance
            transform.GetComponent<ScrollIndexCallbackBase>()?.ScrollCellIndex(idx, m_ListBank.GetListContent(idx));
        }
    }
}
