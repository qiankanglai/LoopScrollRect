using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScrollIndexCallback3 : MonoBehaviour
{
    public Text text;
    void ScrollCellIndex(int idx)
    {
        string name = "Cell " + idx.ToString();
        if (text != null)
        {
            text.text = name;
        }
    }
}
