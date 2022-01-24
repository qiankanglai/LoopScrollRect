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
            if(string.IsNullOrEmpty(UniqueID))
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
        // Get content in list by index
        public abstract object GetListContent(int index);
        // Get content count in list
        public abstract int GetListLength();

        public abstract List<LoopListBankData> InitLoopListBankDataList();

        public abstract LoopListBankData GetLoopListBankData(int index);

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

        public override List<LoopListBankData> InitLoopListBankDataList()
        {
            m_LoopListBankDataList.Clear();
            LoopListBankData TempCustomData = null;
            for (int i = 0; i < m_Contents.Count; ++i)
            {
                TempCustomData = new LoopListBankData();
                TempCustomData.Content = m_Contents[i];
                TempCustomData.UniqueID = System.Guid.NewGuid().ToString();
                m_LoopListBankDataList.Add(TempCustomData);
            }

            return m_LoopListBankDataList;
        }

        public override LoopListBankData GetLoopListBankData(int index)
        {
            if (m_LoopListBankDataList.Count <= index)
            {
                return new LoopListBankData();
            }
            return m_LoopListBankDataList[index];
        }

        public void AddContent(int newContent)
        {
            m_Contents.Add(newContent);

            LoopListBankData TempCustomData = new LoopListBankData();
            TempCustomData.Content = newContent;
            TempCustomData.UniqueID = System.Guid.NewGuid().ToString();
            m_LoopListBankDataList.Add(TempCustomData);
        }

        public void DelContentByIndex(int index)
        {
            if (m_Contents.Count <= index)
            {
                return;
            }
            m_Contents.RemoveAt(index);
            m_LoopListBankDataList.RemoveAt(index);
        }

        public void SetContents(List<int> newContents)
        {
            m_Contents = newContents;
            InitLoopListBankDataList();
        }

        public void SetLoopListBankDatas(List<LoopListBankData> newDatas)
        {
            m_LoopListBankDataList = newDatas;
        }

        public List<int> GetContents()
        {
            return m_Contents;
        }

        public List<LoopListBankData> GetLoopListBankDatas()
        {
            return m_LoopListBankDataList;
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

