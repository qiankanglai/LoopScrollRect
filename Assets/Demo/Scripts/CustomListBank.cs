using System.Collections.Generic;
using UnityEngine;

namespace Demo
{
    public class CustomListBank : LoopListBankBase
    {
        private List<int> m_Contents = new List<int>
        {
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
            11, 12, 13, 14, 15, 16, 17, 18, 19, 20
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

        // Cell Sizes
        public List<Vector2> m_CellSizes = new List<Vector2> 
        {
            new Vector2(120, 120),
            new Vector2(170, 120),
            new Vector2(220, 120)
        };

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
            if(m_UniqueIDList.Count <= index)
            {
                return "";
            }
            return m_UniqueIDList[index];
        }

        public override int GetCellPreferredTypeIndex(int index)
        {
            var TempConten = GetListContent(index);

            int TempData = (int)TempConten;
            int ResultIndex = Mathf.Abs(TempData) % 3;

            return ResultIndex;
        }

        public override Vector2 GetCellPreferredSize(int index)
        {
            int ResultIndex = GetCellPreferredTypeIndex(index);

            Vector2 FinalValue = m_CellSizes[ResultIndex];

            return FinalValue;
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