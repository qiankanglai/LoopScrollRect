using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Demo
{
    [RequireComponent(typeof(UnityEngine.UI.LoopScrollRectMulti))]
    [DisallowMultipleComponent]
    public class InitOnStartMulti : MonoBehaviour, LoopScrollPrefabSource, LoopScrollMultiDataSource
    {
        public LoopScrollRectMulti m_LoopScrollRect;

        public LoopListBankBase m_LoopListBank;

        // Is Use MulitiPrefab
        public bool m_IsUseMultiPrefabs = false;
        // Cell Prefab
        public GameObject m_Item;
        // Cell MulitiPrefab
        public List<GameObject> m_ItemList = new List<GameObject>();

        [HideInInspector]
        public string m_ClickUniqueID = "";
        [HideInInspector]
        public object m_ClickObject;

        protected virtual void Awake()
        {
            m_LoopScrollRect.prefabSource = this;
            m_LoopScrollRect.dataSource = this;
            m_LoopScrollRect.totalCount = m_LoopListBank.GetListLength();
            m_LoopScrollRect.RefillCells();
        }

        protected virtual void Start()
        {

        }

        // Implement your own Cache Pool here. The following is just for example.
        Stack<Transform> pool = new Stack<Transform>();
        Dictionary<string, Stack<Transform>> m_Pool_Type = new Dictionary<string, Stack<Transform>>();
        public virtual GameObject GetObject(int index)
        {
            Transform candidate = null;
            ScrollIndexCallbackBase TempScrollIndexCallbackBase = null;
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
                TempScrollIndexCallbackBase = candidate.GetComponent<ScrollIndexCallbackBase>();
                if (null != TempScrollIndexCallbackBase)
                {
                    TempScrollIndexCallbackBase.SetPrefabName(m_Item.name);
                    if (m_LoopScrollRect.horizontal)
                    {
                        float RandomWidth = m_LoopListBank.GetCellPreferredSize(index).x;
                        TempScrollIndexCallbackBase.SetLayoutElementPreferredWidth(RandomWidth);
                    }

                    if (m_LoopScrollRect.vertical)
                    {
                        float RandomHeight = m_LoopListBank.GetCellPreferredSize(index).y;
                        TempScrollIndexCallbackBase.SetLayoutElementPreferredHeight(RandomHeight);
                    }
                }
            }
            else
            {
                // Cell MulitiPrefab, Get Cell Preferred Type by custom data
                int CellTypeIndex = m_LoopListBank.GetCellPreferredTypeIndex(index);
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
                    TempScrollIndexCallbackBase = candidate.GetComponent<ScrollIndexCallbackBase>();
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

            TempScrollIndexCallbackBase = candidate.gameObject.GetComponent<ScrollIndexCallbackBase>();
            if (null != TempScrollIndexCallbackBase)
            {
                TempScrollIndexCallbackBase.SetUniqueID(m_LoopListBank.GetLoopListBankData(index).UniqueID);
                TempScrollIndexCallbackBase.onClick_InitOnStart.RemoveAllListeners();
                TempScrollIndexCallbackBase.onClick_InitOnStart.AddListener(() => OnButtonScrollIndexCallbackClick(TempScrollIndexCallbackBase, index, TempScrollIndexCallbackBase.GetContent(), TempScrollIndexCallbackBase.GetUniqueID()));
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
            transform.GetComponent<ScrollIndexCallbackBase>()?.ScrollCellIndex(idx, m_LoopListBank.GetLoopListBankData(idx).Content, m_ClickUniqueID, m_ClickObject);
        }

        private void OnButtonScrollIndexCallbackClick(ScrollIndexCallbackBase ScrollIndexCallback, int index, object content, string ClickUniqueID)
        {
            //Debug.LogWarningFormat("InitOnStartMulti => Click index: {0}, content: {1}, ClickUniqueID: {2}", index, content, ClickUniqueID);

            m_ClickUniqueID = ClickUniqueID;
            m_ClickObject = content;

            foreach (var TempScrollIndexCallback in m_LoopScrollRect.content.GetComponentsInChildren<ScrollIndexCallbackBase>())
            {
                TempScrollIndexCallback.RefreshUI(ClickUniqueID, m_ClickObject);
            }
        }

        public virtual ScrollIndexCallbackBase GetScrollIndexCallbackByIndex(int idx)
        {
            foreach (var TempScrollIndexCallback in m_LoopScrollRect.content.GetComponentsInChildren<ScrollIndexCallbackBase>())
            {
                if (TempScrollIndexCallback.GetIndexID() == idx)
                {
                    return TempScrollIndexCallback;
                }
            }
            return null;
        }
    }
}