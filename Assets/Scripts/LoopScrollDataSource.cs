using UnityEngine;
using System.Collections;

namespace UnityEngine.UI
{
    public interface LoopScrollDataSource
    {
        public void ProvideData(Transform transform, int idx);
    }
}