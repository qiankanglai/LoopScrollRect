using UnityEngine;
using System.Collections;

namespace UnityEngine.UI
{
    public interface LoopScrollPrefabSource
    {
        public GameObject GetObject();

        public void ReturnObject(Transform trans);
    }
}
