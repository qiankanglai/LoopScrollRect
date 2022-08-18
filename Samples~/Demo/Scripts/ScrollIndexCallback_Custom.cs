using UnityEngine;
using UnityEngine.UI;

namespace Demo
{
    public class ScrollIndexCallback_Custom : ScrollIndexCallbackBase
    {
        public Text m_CellText;
        public Text m_ValueText;

        public override void ScrollCellIndex(int idx, object content, string ClickUniqueID = "", object ClickObject = null)
        {
            base.ScrollCellIndex(idx, content, ClickUniqueID, ClickObject);

            onClick_Custom.RemoveAllListeners();
            onClick_Custom.AddListener(() => OnButtonScrollIndexCallbackClick(this, idx, content));

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

            if (GetUniqueID() == ClickUniqueID)
            {
                SetClickedColor(true);
            }
            else
            {
                SetClickedColor(false);
            }
        }

        public static void OnButtonScrollIndexCallbackClick(ScrollIndexCallback_Custom ScrollIndexCallback, int index, object content)
        {
            //Debug.LogWarningFormat("ScrollIndexCallback_Custom => Click index: {0}, content: {1}, ClickUniqueID: {2}", index, content, ScrollIndexCallback.GetUniqueID());
        }

        public override void RefreshUI(string ClickUniqueID, object ClickContent)
        {
            base.RefreshUI(ClickUniqueID, ClickContent);

            if(GetUniqueID() == ClickUniqueID)
            {
                SetClickedColor(true);
            }
            else
            {
                SetClickedColor(false);
            }
        }

        public void SetClickedColor(bool IsClicked)
        {
            if (IsClicked)
            {
                m_Button.image.color = Color.cyan;
            }
            else
            {
                m_Button.image.color = Color.white;
            }
        }
    }
}