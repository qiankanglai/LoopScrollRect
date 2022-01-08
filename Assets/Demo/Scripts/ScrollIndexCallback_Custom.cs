using UnityEngine;
using UnityEngine.UI;

namespace Demo
{
    public class ScrollIndexCallback_Custom : ScrollIndexCallbackBase
    {
        public Text m_CellText;
        public Text m_ValueText;

        public override void ScrollCellIndex(int idx, object content)
        {
            base.ScrollCellIndex(idx, content);

            m_Button.onClick.RemoveListener(() => ScrollIndexCallback_Custom.OnButtonScrollIndexCallbackClick(idx, content));
            m_Button.onClick.AddListener(() => ScrollIndexCallback_Custom.OnButtonScrollIndexCallbackClick(idx, content));

            var Tempcontent = (int)content;

            string name = "Cell " + idx.ToString();
            if (m_CellText != null)
            {
                m_CellText.text = name;
            }

            if(m_ValueText != null)
            {
                m_ValueText.text = string.Format("Value: {0}", Tempcontent);
            }
        }

        public static void OnButtonScrollIndexCallbackClick(int index, object content)
        {
            Debug.LogWarningFormat("ScrollIndexCallback_Custom => Click index: {0}, content: {1}", index, content);
        }
    }
}