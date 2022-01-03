using UnityEngine;
using UnityEngine.UI;

namespace Demo
{
    public class ScrollIndexCallbackBase : MonoBehaviour
    {
        public LayoutElement m_Element;
        public Button m_Button;
        private int m_IndexID = 0;
        private string m_PrefabName = "";
        private object m_Content;

        protected virtual void Awake()
        {
            m_Button = GetComponent<Button>();
        }

        // Get IndexID
        public int GetIndexID()
        {
            return m_IndexID;
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

        public virtual void ScrollCellIndex(int idx, object content)
        {
            m_IndexID = idx;
            m_Content = content;

            string name = "Cell " + idx.ToString();
            gameObject.name = m_PrefabName + name;
        }
    }
}
