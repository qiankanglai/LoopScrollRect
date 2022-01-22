/* Store the contents for ListBoxes to display.
 */
using System.Collections.Generic;
using UnityEngine;

namespace Demo
{
    /* The base class of the list content container
     *
     * Create the individual ListBank by inheriting this class
     */
    public abstract class LoopListBankBase : MonoBehaviour
    {
        // Get content in list by index
        public abstract object GetListContent(int index);
        // Get content count in list
        public abstract int GetListLength();

        public abstract List<string> InitUniqueIDList();

        public abstract string GetListUniqueID(int index);

        // Get cell preferred type index by index
        public abstract int GetCellPreferredTypeIndex(int index);
        // Get cell preferred size by index
        public abstract Vector2 GetCellPreferredSize(int index);
    }

    /* The example of the ListBank
     */
    public class LoopListBank : LoopListBankBase
    {
        private List<int> m_Contents = new List<int>
        {
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10
        };

        private List<string> _UniqueIDList;
        private List<string> m_UniqueIDList
        {
            get
            {
                if (_UniqueIDList == null)
                {
                    _UniqueIDList = new List<string>();
                    _UniqueIDList = InitUniqueIDList();
                }

                return _UniqueIDList;
            }
        }

        public override object GetListContent(int index)
        {
            if (m_Contents.Count <= index)
            {
                return null;
            }
            return m_Contents[index];
        }

        public override int GetListLength()
        {
            return m_Contents.Count;
        }

        public override List<string> InitUniqueIDList()
        {
            m_UniqueIDList.Clear();
            for (int i = 0; i < m_Contents.Count; ++i)
            {
                m_UniqueIDList.Add(System.Guid.NewGuid().ToString());
            }

            return m_UniqueIDList;
        }

        public override string GetListUniqueID(int index)
        {
            if (m_UniqueIDList.Count <= index)
            {
                return "";
            }
            return m_UniqueIDList[index];
        }

        public override int GetCellPreferredTypeIndex(int index)
        {
            return 0;
        }

        public override Vector2 GetCellPreferredSize(int index)
        {
            return new Vector2();
        }

        public void AddContent(int newContent)
        {
            m_Contents.Add(newContent);
            m_UniqueIDList.Add(System.Guid.NewGuid().ToString());
        }

        public void DelContentByIndex(int index)
        {
            if (m_Contents.Count <= index)
            {
                return;
            }
            m_Contents.RemoveAt(index);
            m_UniqueIDList.RemoveAt(index);
        }

        public void SetContents(List<int> newContents)
        {
            m_Contents = newContents;
            InitUniqueIDList();
        }

        public List<int> GetContents()
        {
            return m_Contents;
        }
    }
}

