using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Demo
{
    public class ScrollIndexCallbackBase : MonoBehaviour
    {
        public LayoutElement m_Element;
        public Button m_Button;
        private int m_IndexID = 0;
        private string m_UniqueID = "";
        private string m_PrefabName = "";
        private object m_Content;
        private bool m_IsUpdateGameObjectName = true;

        [Serializable]
        public class ButtonClickedEvent : UnityEvent { }

        // Event delegates triggered on click for Base.
        [FormerlySerializedAs("onClick_InitOnStart")]
        [SerializeField]
        private ButtonClickedEvent m_OnClick_InitOnStart = new ButtonClickedEvent();

        public ButtonClickedEvent onClick_InitOnStart
        {
            get { return m_OnClick_InitOnStart; }
            set { m_OnClick_InitOnStart = value; }
        }

        // Event delegates triggered on click for Custom.
        [FormerlySerializedAs("onClick_Custom")]
        [SerializeField]
        private ButtonClickedEvent m_OnClick_Custom = new ButtonClickedEvent();

        public ButtonClickedEvent onClick_Custom
        {
            get { return m_OnClick_Custom; }
            set { m_OnClick_Custom = value; }
        }

        protected virtual void Awake()
        {
            m_Button.onClick.AddListener(OnButtonClickCallBack);
        }

        protected virtual void OnDestroy()
        {
            m_Button.onClick.RemoveAllListeners();
        }

        private void OnButtonClickCallBack()
        {
            m_OnClick_InitOnStart.Invoke();
            m_OnClick_Custom.Invoke();
        }

        // Get IndexID
        public int GetIndexID()
        {
            return m_IndexID;
        }

        public string GetUniqueID()
        {
            return m_UniqueID;
        }

        public void SetUniqueID(string UniqueID)
        {
            m_UniqueID = UniqueID;
        }

        public void SetPrefabName(string name)
        {
            m_PrefabName = name;
        }

        // Get PrefabName
        public string GetPrefabName()
        {
            return m_PrefabName;
        }

        public void SetIsUpdateGameObjectName(bool value)
        {
            m_IsUpdateGameObjectName = value;
        }

        public object GetContent()
        {
            return m_Content;
        }

        // Set Element PreferredWidth
        public virtual void SetLayoutElementPreferredWidth(float value)
        {
            m_Element.preferredWidth = value;
        }

        // Set Element PreferredHeight
        public virtual void SetLayoutElementPreferredHeight(float value)
        {
            m_Element.preferredHeight = value;
        }

        public virtual void ScrollCellIndex(int idx, object content, string ClickUniqueID = "", object ClickObject = null)
        {
            m_IndexID = idx;
            m_Content = content;

            if (m_IsUpdateGameObjectName)
            {
                gameObject.name = string.Format("{0} Cell {1}", m_PrefabName, idx.ToString());
            }
        }

        public virtual void RefreshUI(string ClickUniqueID, object ClickContent)
        {

        }
    }
}
