using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace SG
{
    [RequireComponent(typeof(UnityEngine.UI.LoopScrollRect))]
    [DisallowMultipleComponent]
    public class InitOnStart : MonoBehaviour, LoopScrollPrefabSource, LoopScrollDataSource
    {
        public GameObject item;
        public int totalCount = -1;

        // Implement your own Cache Pool here. The following is just for example.
        Stack<Transform> pool = new Stack<Transform>();
        public GameObject GetObject()
        {
            if (pool.Count == 0)
            {
                return Instantiate(item);
            }
            Transform candidate = pool.Pop();
            candidate.gameObject.SetActive(true);
            return candidate.gameObject;
        }

        public void ReturnObject(Transform trans)
        {
            trans.SendMessage("ScrollCellReturn", SendMessageOptions.DontRequireReceiver);
            trans.gameObject.SetActive(false);
            trans.SetParent(transform, false);
            pool.Push(trans);
        }

        public void ProvideData(Transform transform, int idx)
        {
            transform.SendMessage("ScrollCellIndex", idx);
        }

        void Start()
        {
            var ls = GetComponent<LoopScrollRect>();
            ls.prefabSource = this;
            ls.dataSource = this;
            ls.totalCount = totalCount;
            ls.RefillCells();
        }
    }
}