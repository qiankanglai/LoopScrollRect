using UnityEngine;
using System.Collections;

namespace UnityEngine.UI
{
    [System.Serializable]
    public class LoopScrollPrefabSource 
    {
        public string prefabName;
        public int poolSize = 5;

        public LoopScrollPrefabSource()
        {}

        public LoopScrollPrefabSource(string prefabName, int poolSize = 5)
        {
            this.prefabName = prefabName;
            this.poolSize = poolSize;
        }

        public virtual void InitPool()
        {
            SG.ResourceManager.Instance.InitPool(prefabName, poolSize);
        }
            
        public virtual GameObject GetObject()
        {
            return SG.ResourceManager.Instance.GetObjectFromPool(prefabName);
        }
    }
}
