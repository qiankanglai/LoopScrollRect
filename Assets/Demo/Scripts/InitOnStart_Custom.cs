using UnityEngine;

namespace Demo
{
    public class InitOnStart_Custom : InitOnStartMultiBase
    {
        public override GameObject GetObject(int index)
        {
            var TempGameObject = base.GetObject(index);

            var TempScrollIndexCallbackBase = TempGameObject.GetComponent<ScrollIndexCallbackBase>();
            if(null != TempScrollIndexCallbackBase)
            {
                TempScrollIndexCallbackBase.onClick.RemoveListener(() => OnButtonScrollIndexCallbackClick(index, TempScrollIndexCallbackBase.GetContent()));
                TempScrollIndexCallbackBase.onClick.AddListener(()=> OnButtonScrollIndexCallbackClick(index, TempScrollIndexCallbackBase.GetContent()));
            }

            return TempGameObject;
        }

        private void OnButtonScrollIndexCallbackClick(int index, object content)
        {
            //Debug.LogWarningFormat("InitOnStart_Custom => Click index: {0}, content: {1}", index, content);
        }
    }
}