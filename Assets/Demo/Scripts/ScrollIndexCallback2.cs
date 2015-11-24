using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScrollIndexCallback2 : MonoBehaviour 
{
    public Text text;
    public LayoutElement element;
    private static float[] randomWidths = new float[3] { 100, 150, 50 };
    void ScrollCellIndex(int idx)
    {
        string name = "Cell " + idx.ToString();
        if (text != null)
        {
            text.text = name;
        }
        element.preferredWidth = randomWidths[Mathf.Abs(idx) % 3];
        gameObject.name = name;
    }
}
