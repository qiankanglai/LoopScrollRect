using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

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
                TempScrollIndexCallbackBase.m_Button.onClick.RemoveListener(() => OnButtonScrollIndexCallbackClick(index, TempScrollIndexCallbackBase.GetContent()));
                TempScrollIndexCallbackBase.m_Button.onClick.AddListener(()=> OnButtonScrollIndexCallbackClick(index, TempScrollIndexCallbackBase.GetContent()));
            }

            return TempGameObject;
        }

        private void OnButtonScrollIndexCallbackClick(int index, object content)
        {
            Debug.LogWarningFormat("Click index: {0}, content: {1}", index, content);
        }
    }
}