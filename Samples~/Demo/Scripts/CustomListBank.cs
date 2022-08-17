using System.Collections.Generic;
using UnityEngine;

namespace Demo
{

    public class CustomListBank : LoopListBankBase
    {
        private List<int> m_ContentsForInitData = new List<int>
        {
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
            11, 12, 13, 14, 15, 16, 17, 18, 19, 20
        };

        private List<LoopListBankData> _LoopListBankDataList;
        private List<LoopListBankData> m_LoopListBankDataList
        {
            get
            {
                if (_LoopListBankDataList == null)
                {
                    _LoopListBankDataList = new List<LoopListBankData>();
                    _LoopListBankDataList = InitLoopListBankDataList();
                }

                return _LoopListBankDataList;
            }
            set { _LoopListBankDataList = value; }
        }

        // Cell Sizes
        public List<Vector2> m_CellSizes = new List<Vector2> 
        {
            new Vector2(120, 120),
            new Vector2(170, 120),
            new Vector2(220, 120)
        };

        public override List<LoopListBankData> InitLoopListBankDataList()
        {
            m_LoopListBankDataList.Clear();
            LoopListBankData TempCustomData = null;
            for (int i = 0; i < m_ContentsForInitData.Count; ++i)
            {
                TempCustomData = new LoopListBankData();
                TempCustomData.Content = m_ContentsForInitData[i];
                TempCustomData.UniqueID = System.Guid.NewGuid().ToString();
                m_LoopListBankDataList.Add(TempCustomData);
            }

            return m_LoopListBankDataList;
        }

        public override int GetListLength()
        {
            return m_LoopListBankDataList.Count;
        }

        public override LoopListBankData GetLoopListBankData(int index)
        {
            if(m_LoopListBankDataList.Count == 0)
            {
                return new LoopListBankData();
            }
            index = index % m_LoopListBankDataList.Count;
            if (index < 0)
            {
                index += m_LoopListBankDataList.Count;
            }
            return m_LoopListBankDataList[index];
        }

        public override List<LoopListBankData> GetLoopListBankDatas()
        {
            return m_LoopListBankDataList;
        }

        public override void SetLoopListBankDatas(List<LoopListBankData> newDatas)
        {
            m_LoopListBankDataList = newDatas;
        }

        public int FindUniqueID(string UniqueID)
        {
            if (string.IsNullOrEmpty(UniqueID))
            {
                return -1;
            }

            for (int i = 0; i < m_LoopListBankDataList.Count; ++i)
            {
                if (m_LoopListBankDataList[i].UniqueID == UniqueID)
                {
                    return i;
                }
            }

            return -1;
        }

        public void AddContent(object newContent)
        {
            LoopListBankData TempCustomData = new LoopListBankData();
            TempCustomData.Content = newContent;
            TempCustomData.UniqueID = System.Guid.NewGuid().ToString();
            m_LoopListBankDataList.Add(TempCustomData);
        }

        public void DelContentByIndex(int index)
        {
            if (m_LoopListBankDataList.Count <= index)
            {
                return;
            }
            m_LoopListBankDataList.RemoveAt(index);
        }

        public void SetContents(List<int> newContents)
        {
            m_ContentsForInitData = newContents;
            InitLoopListBankDataList();
        }

        public override int GetCellPreferredTypeIndex(int index)
        {
            var TempConten = GetLoopListBankData(index).Content;

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
    }
}