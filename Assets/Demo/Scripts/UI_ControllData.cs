using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    public class UI_ControllData : MonoBehaviour
    {
        public Button m_ButtonAddData;
        public Button m_ButtonSetData;
        public Button m_ButtonDelData;
        public Button m_ButtonSortData;
        public Button m_ButtonReverseSortData;
        public Button m_ButtonSrollToCell;

        public InputField m_InputFieldSrollToCell_CellIndex;
        public InputField m_InputButtonSrollToCell_MoveSpeed;

        public InitOnStart_Custom m_InitOnStart_Custom;

        private CustomListBank m_CustomListBank;

        // Start is called before the first frame update
        void Start()
        {
            m_CustomListBank = m_InitOnStart_Custom.m_ListBank as CustomListBank;

            m_ButtonAddData.onClick.AddListener(OnButtonAddDataClick);
            m_ButtonSetData.onClick.AddListener(OnButtonSetDataClick);
            m_ButtonDelData.onClick.AddListener(OnButtonDelDataClick);
            m_ButtonSortData.onClick.AddListener(OnButtonSortDataClick);
            m_ButtonReverseSortData.onClick.AddListener(OnButtonReverseSortDataClick);
            m_ButtonSrollToCell.onClick.AddListener(OnButtonSrollToCellClick);
        }

        private void OnDestroy()
        {
            m_ButtonAddData.onClick.RemoveListener(OnButtonAddDataClick);
            m_ButtonSetData.onClick.RemoveListener(OnButtonSetDataClick);
            m_ButtonDelData.onClick.RemoveListener(OnButtonDelDataClick);
            m_ButtonSortData.onClick.RemoveListener(OnButtonSortDataClick);
            m_ButtonReverseSortData.onClick.RemoveListener(OnButtonReverseSortDataClick);
            m_ButtonSrollToCell.onClick.RemoveListener(OnButtonSrollToCellClick);
        }

        private void OnButtonAddDataClick()
        {
            int RandomResult = Random.Range(0, 10);
            m_CustomListBank.AddContent(RandomResult);

            m_InitOnStart_Custom.m_LoopScrollRect.totalCount = m_InitOnStart_Custom.m_ListBank.GetListLength();
            m_InitOnStart_Custom.m_LoopScrollRect.RefreshCells();
        }

        private void OnButtonSetDataClick()
        {
            List<int> contents = new List<int>
            {
                3, 4, 5, 6, 7
            };

            m_CustomListBank.SetContents(contents);
            m_InitOnStart_Custom.m_LoopScrollRect.totalCount = m_InitOnStart_Custom.m_ListBank.GetListLength();
            m_InitOnStart_Custom.m_LoopScrollRect.RefillCells();
        }

        private void OnButtonDelDataClick()
        {
            m_CustomListBank.DelContentByIndex(0);

            int ShowLeftIndex = m_InitOnStart_Custom.m_LoopScrollRect.GetItemTypeStart();

            m_InitOnStart_Custom.m_LoopScrollRect.ClearCells();
            m_InitOnStart_Custom.m_LoopScrollRect.totalCount = m_InitOnStart_Custom.m_ListBank.GetListLength();
            m_InitOnStart_Custom.m_LoopScrollRect.RefillCells();

            m_InitOnStart_Custom.m_LoopScrollRect.SrollToCell(ShowLeftIndex, -1);
            // No effect
            //m_InitOnStart_Custom.m_LoopScrollRect.SrollToCell(ShowLeftIndex, 10);
        }

        private void OnButtonSortDataClick()
        {
            var TempContent = m_CustomListBank.GetContents();
            TempContent.Sort((x, y) => x.CompareTo(y));

            m_InitOnStart_Custom.m_LoopScrollRect.ClearCells();
            m_InitOnStart_Custom.m_LoopScrollRect.totalCount = m_InitOnStart_Custom.m_ListBank.GetListLength();
            m_InitOnStart_Custom.m_LoopScrollRect.RefillCells();
        }

        private void OnButtonReverseSortDataClick()
        {
            var TempContent = m_CustomListBank.GetContents();
            TempContent.Sort((x, y) => -x.CompareTo(y));

            m_InitOnStart_Custom.m_LoopScrollRect.ClearCells();
            m_InitOnStart_Custom.m_LoopScrollRect.totalCount = m_InitOnStart_Custom.m_ListBank.GetListLength();
            m_InitOnStart_Custom.m_LoopScrollRect.RefillCells();
        }

        private void OnButtonSrollToCellClick()
        {
            int Index = 0;
            int.TryParse(m_InputFieldSrollToCell_CellIndex.text, out Index);

            int MoveSpeed = 0;
            int.TryParse(m_InputButtonSrollToCell_MoveSpeed.text, out MoveSpeed);

            m_InitOnStart_Custom.m_LoopScrollRect.SrollToCell(Index, MoveSpeed);
        }
    }
}