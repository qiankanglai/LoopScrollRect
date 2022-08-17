/* Store the contents for ListBoxes to display.
 */
using System.Collections.Generic;
using UnityEngine;

namespace Demo
{
    // LoopListBankData
    public class LoopListBankData
    {
        public object Content;
        public string UniqueID = "";

        public bool IsEmpty()
        {
            if(null == Content
            || string.IsNullOrEmpty(UniqueID))
            {
                return true;
            }
            return false;
        }
    }
    /* The base class of the list content container
     *
     * Create the individual ListBank by inheriting this class
     */
    public abstract class LoopListBankBase : MonoBehaviour
    {
        // Init DataList
        public abstract List<LoopListBankData> InitLoopListBankDataList();

        // Get Data count in list
        public abstract int GetListLength();

        // Get Data in list by index
        public abstract LoopListBankData GetLoopListBankData(int index);

        // Get All Data in list
        public abstract List<LoopListBankData> GetLoopListBankDatas();

        // Set Data into list
        public abstract void SetLoopListBankDatas(List<LoopListBankData> newDatas);

        // Get cell preferred type index by index
        public abstract int GetCellPreferredTypeIndex(int index);
        // Get cell preferred size by index
        public abstract Vector2 GetCellPreferredSize(int index);
    }

    /* The example of the ListBank
     */
    public class LoopListBank : LoopListBankBase
    {
        private List<int> m_ContentsForInitData = new List<int>
        {
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10
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
            if (m_LoopListBankDataList.Count <= index)
            {
                return new LoopListBankData();
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
            if(string.IsNullOrEmpty(UniqueID))
            {
                return -1;
            }

            for(int i=0;i<m_LoopListBankDataList.Count;++i)
            {
                if(m_LoopListBankDataList[i].UniqueID == UniqueID)
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
            return 0;
        }

        public override Vector2 GetCellPreferredSize(int index)
        {
            int ResultIndex = GetCellPreferredTypeIndex(index);

            Vector2 FinalValue = m_CellSizes[ResultIndex];

            return FinalValue;
        }
    }
}

