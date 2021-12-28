using qiankanglai.LoopScrollRectManager;
using UnityEngine.UI;

namespace SG
{
    public class ScrollIndexCallback_Custom : ScrollIndexCallbackBase
    {
        public Text m_CellText;
        public Text m_ValueText;

        public override void ScrollCellIndex(int idx, object content)
        {
            base.ScrollCellIndex(idx, content);

            var Tempcontent = (int)content;

            string name = "Cell " + idx.ToString();
            if (m_CellText != null)
            {
                m_CellText.text = name;
            }

            if (m_ValueText != null)
            {
                m_ValueText.text = string.Format("Value: {0}", Tempcontent);
            }
        }
    }
}