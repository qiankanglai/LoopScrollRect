using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Demo
{
    public class UI_ControllData : MonoBehaviour
    {
        public Button m_ButtonAddData;
        public Button m_ButtonSetData;
        public Button m_ButtonDelData;
        public Button m_ButtonSortData;
        public Button m_ButtonReverseSortData;
        public Button m_ButtonSrollToCell;
        public Button m_ButtonSrollToCellWithTime;

        public InputField m_InputFieldSrollToCell_CellIndex;
        public InputField m_InputButtonSrollToCell_MoveSpeed;

        public InputField m_InputFieldSrollToCellWithTime_CellIndex;
        public InputField m_InputButtonSrollToCellWithTime_MoveSpeed;

        public InitOnStartMulti m_InitOnStart;

        private CustomListBank m_ListBank;

        private void Awake()
        {
            m_ListBank = m_InitOnStart.m_LoopListBank as CustomListBank;

            m_ButtonAddData.onClick.AddListener(OnButtonAddDataClick);
            m_ButtonSetData.onClick.AddListener(OnButtonSetDataClick);
            m_ButtonDelData.onClick.AddListener(OnButtonDelDataClick);
            m_ButtonSortData.onClick.AddListener(OnButtonSortDataClick);
            m_ButtonReverseSortData.onClick.AddListener(OnButtonReverseSortDataClick);
            m_ButtonSrollToCell.onClick.AddListener(OnButtonSrollToCellClick);
            m_ButtonSrollToCellWithTime.onClick.AddListener(OnButtonSrollToCellWithTimeClick);
        }

        private void OnDestroy()
        {
            m_ButtonAddData.onClick.RemoveListener(OnButtonAddDataClick);
            m_ButtonSetData.onClick.RemoveListener(OnButtonSetDataClick);
            m_ButtonDelData.onClick.RemoveListener(OnButtonDelDataClick);
            m_ButtonSortData.onClick.RemoveListener(OnButtonSortDataClick);
            m_ButtonReverseSortData.onClick.RemoveListener(OnButtonReverseSortDataClick);
            m_ButtonSrollToCell.onClick.RemoveListener(OnButtonSrollToCellClick);
            m_ButtonSrollToCellWithTime.onClick.RemoveListener(OnButtonSrollToCellWithTimeClick);
        }

        private void OnButtonAddDataClick()
        {
            int RandomResult = Random.Range(0, 10);
            m_ListBank.AddContent(RandomResult);
            m_InitOnStart.m_LoopScrollRect.totalCount = m_InitOnStart.m_LoopListBank.GetListLength();
            m_InitOnStart.m_LoopScrollRect.RefreshCells();
        }

        private void OnButtonSetDataClick()
        {
            List<int> contents = new List<int>{
                3, 4, 5, 6, 7
            };

            m_ListBank.SetContents(contents);
            m_InitOnStart.m_LoopScrollRect.totalCount = m_InitOnStart.m_LoopListBank.GetListLength();
            m_InitOnStart.m_LoopScrollRect.RefillCells();
        }

        private void OnButtonDelDataClick()
        {
            m_ListBank.DelContentByIndex(0);

            float offset;
            int LeftIndex = m_InitOnStart.m_LoopScrollRect.GetFirstItem(out offset);

            m_InitOnStart.m_LoopScrollRect.ClearCells();
            m_InitOnStart.m_LoopScrollRect.totalCount = m_InitOnStart.m_LoopListBank.GetListLength();
            if (LeftIndex > 0)
                // try keep the same position
                m_InitOnStart.m_LoopScrollRect.RefillCells(LeftIndex - 1, false, offset);
            else
                m_InitOnStart.m_LoopScrollRect.RefillCells();
        }

        private void OnButtonSortDataClick()
        {
            var TempContent = m_ListBank.GetContents();
            TempContent.Sort((x, y) => x.CompareTo(y));

            m_InitOnStart.m_LoopScrollRect.ClearCells();
            m_InitOnStart.m_LoopScrollRect.totalCount = m_InitOnStart.m_LoopListBank.GetListLength();
            m_InitOnStart.m_LoopScrollRect.RefillCells();
        }

        private void OnButtonReverseSortDataClick()
        {
            var TempContent = m_ListBank.GetContents();
            TempContent.Sort((x, y) => -x.CompareTo(y));

            m_InitOnStart.m_LoopScrollRect.ClearCells();
            m_InitOnStart.m_LoopScrollRect.totalCount = m_InitOnStart.m_LoopListBank.GetListLength();
            m_InitOnStart.m_LoopScrollRect.RefillCells();
        }

        private void OnButtonSrollToCellClick()
        {
            int Index = 0;
            int.TryParse(m_InputFieldSrollToCell_CellIndex.text, out Index);

            float MoveSpeed = 0;
            float.TryParse(m_InputButtonSrollToCell_MoveSpeed.text, out MoveSpeed);

            m_InitOnStart.m_LoopScrollRect.SrollToCell(Index, MoveSpeed);
        }

        private void OnButtonSrollToCellWithTimeClick()
        {
            int Index = 0;
            int.TryParse(m_InputFieldSrollToCellWithTime_CellIndex.text, out Index);

            float MoveTime = 0;
            float.TryParse(m_InputButtonSrollToCellWithTime_MoveSpeed.text, out MoveTime);

            m_InitOnStart.m_LoopScrollRect.SrollToCellWithinTime(Index, MoveTime);
        }
    }
}