/* Store the contents for ListBoxes to display.
 */
using UnityEngine;

namespace Demo
{
    /* The base class of the list content container
     *
     * Create the individual ListBank by inheriting this class
     */
    public abstract class BaseListBank : MonoBehaviour
    {
        // Get content in list by index
        public abstract object GetListContent(int index);
        // Get content count in list
        public abstract int GetListLength();

        // Get cell preferred type index by index
        public abstract int GetCellPreferredTypeIndex(int index);
        // Get cell preferred size by index
        public abstract Vector2 GetCellPreferredSize(int index);
    }

    /* The example of the ListBank
     */
    public class ListBank : BaseListBank
    {
        private int[] contents = {
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10
        };

        public override object GetListContent(int index)
        {
            return contents[index].ToString();
        }

        public override int GetListLength()
        {
            return contents.Length;
        }

        public override int GetCellPreferredTypeIndex(int index)
        {
            return 0;
        }

        public override Vector2 GetCellPreferredSize(int index)
        {
            return new Vector2();
        }
    }
}

