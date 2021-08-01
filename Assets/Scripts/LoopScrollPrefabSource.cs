using UnityEngine;
using System.Collections;

namespace UnityEngine.UI
{
    public interface LoopScrollPrefabSource
    {
        GameObject GetObject();

        void ReturnObject(Transform trans);
    }
}
