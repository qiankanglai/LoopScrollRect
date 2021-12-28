using UnityEngine;
using UnityEngine.UI;

namespace Qiankanglai.LoopScrollRectManager
{
    public class ScrollIndexCallbackBase : MonoBehaviour
    {
        public LayoutElement m_Element;
        private int m_IndexID = 0;
        private string m_PrefabName = "";

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
