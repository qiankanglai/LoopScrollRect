using UnityEngine;
using UnityEngine.UI;

namespace qiankanglai.LoopScrollRect
{
    public class ScrollIndexCallbackBase : MonoBehaviour
    {
        public LayoutElement m_Element;
        private int m_IndexID = 0;
        private string m_PrefabName = "";

        protected virtual void Awake()
        {
            m_PrefabName = gameObject.name;
        }

        // Get IndexID
        public int GetIndexID()
        {
            return m_IndexID;
        }

        // Get PrefabName
        public string GetPrefabName()
        {
            return m_PrefabName;
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

            string name = "Cell " + idx.ToString();
            gameObject.name = m_PrefabName + name;
        }
    }
}
