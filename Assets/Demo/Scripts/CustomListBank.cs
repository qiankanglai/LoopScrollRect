using qiankanglai.LoopScrollRectManager;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class CustomListBank : LoopListBankBase
    {
        private List<int> contents = new List<int>{
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
            11, 12, 13, 14, 15, 16, 17, 18, 19, 20
        };

        // Cell Sizes
        public List<Vector2> m_CellSizes = new List<Vector2>
    {
        new Vector2(120, 120),
        new Vector2(170, 120),
        new Vector2(220, 120)
    };

        public override object GetListContent(int index)
        {
            return contents[index];
        }

        public override int GetListLength()
        {
            return contents.Count;
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
            contents.Add(newContent);
        }

        public void DelContentByIndex(int index)
        {
            if (contents.Count <= index)
            {
                return;
            }
            contents.RemoveAt(index);
        }

        public void SetContents(List<int> newContents)
        {
            contents = newContents;
        }

        public List<int> GetContents()
        {
            return contents;
        }
    }
}