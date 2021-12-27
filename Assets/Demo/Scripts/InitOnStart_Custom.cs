using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using qiankanglai.LoopScrollRect;

namespace SG
{
    [RequireComponent(typeof(UnityEngine.UI.LoopScrollRect))]
    [DisallowMultipleComponent]
    public class InitOnStart_Custom : MonoBehaviour, LoopScrollPrefabSource, LoopScrollDataSource
    {
        [HideInInspector]
        public LoopScrollRect m_LoopScrollRect;

        // Is Use Muliti-Prefab
        public bool m_IsUseMultiPrefabs = false;
        // Cell Prefab
        public GameObject m_Item;
        // Cell Muliti-Prefab
        public List<GameObject> m_ItemList = new List<GameObject>();

        [HideInInspector]
        public CustomListBank m_CustomListBank;

        private void Awake()
        {

        }

        void Start()
        {
            m_LoopScrollRect = GetComponent<LoopScrollRect>();
            m_LoopScrollRect.prefabSource = this;
            m_LoopScrollRect.dataSource = this;
            m_LoopScrollRect.totalCount = m_CustomListBank.GetListLength();
            m_LoopScrollRect.RefillCells();

            m_CustomListBank = GetComponent<CustomListBank>();
        }

        // Implement your own Cache Pool here. The following is just for example.
        Stack<Transform> pool = new Stack<Transform>();
        Dictionary<string, Stack<Transform>> m_Pool_Type = new Dictionary<string, Stack<Transform>>();
        public GameObject GetObject(int index)
        {
            Transform candidate = null;
            // Is Use Muliti-Prefab
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
                        float RandomWidth = m_CustomListBank.GetCellPreferredSize(index).x;
                        TempScrollIndexCallbackBase.SetLayoutElementPreferredWidth(RandomWidth);
                    }

                    if (m_LoopScrollRect.vertical)
                    {
                        float RandomHeight = m_CustomListBank.GetCellPreferredSize(index).y;
                        TempScrollIndexCallbackBase.SetLayoutElementPreferredHeight(RandomHeight);
                    }
                }
            }
            else
            {
                // Cell Muliti-Prefab, Get Cell Preferred Type by custom data
                int CellTypeIndex = m_CustomListBank.GetCellPreferredTypeIndex(index);
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
                }

                if (TempStack.Count == 0)
                {
                    candidate = Instantiate(TempPrefab).GetComponent<Transform>();
                }
                else
                {
                    candidate = TempStack.Pop();
                    candidate.gameObject.SetActive(true);
                }
            }

            return candidate.gameObject;
        }

        public void ReturnObject(Transform trans)
        {
            // Use `DestroyImmediate` here if you don't need Pool
            trans.SendMessage("ScrollCellReturn", SendMessageOptions.DontRequireReceiver);
            trans.gameObject.SetActive(false);
            trans.SetParent(transform, false);
            // Is Use Muliti-Prefab
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
                    Destroy(trans.gameObject);
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

        public void ProvideData(Transform transform, int idx)
        {
            //Debug.LogWarningFormat("ProvideData()=> transform Name: {0}, IDX: {1}", transform, idx);
            //transform.SendMessage("ScrollCellIndex", idx);

            // Use direct call for better performance
            transform.GetComponent<ScrollIndexCallbackBase>()?.ScrollCellIndex(idx, m_CustomListBank.GetListContent(idx));
        }
    }
}