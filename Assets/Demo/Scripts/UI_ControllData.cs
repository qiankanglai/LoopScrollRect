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

        public InitOnStart_Custom m_InitOnStart_Custom;

        // Start is called before the first frame update
        void Start()
        {
            m_ButtonAddData.onClick.AddListener(OnButtonAddDataClick);
            m_ButtonSetData.onClick.AddListener(OnButtonSetDataClick);
            m_ButtonDelData.onClick.AddListener(OnButtonDelDataClick);
            m_ButtonSortData.onClick.AddListener(OnButtonSortDataClick);
            m_ButtonReverseSortData.onClick.AddListener(OnButtonReverseSortDataClick);
        }

        private void OnDestroy()
        {
            m_ButtonAddData.onClick.RemoveListener(OnButtonAddDataClick);
            m_ButtonSetData.onClick.RemoveListener(OnButtonSetDataClick);
            m_ButtonDelData.onClick.RemoveListener(OnButtonDelDataClick);
            m_ButtonSortData.onClick.RemoveListener(OnButtonSortDataClick);
            m_ButtonReverseSortData.onClick.RemoveListener(OnButtonReverseSortDataClick);
        }

        private void OnButtonAddDataClick()
        {
            int RandomResult = Random.Range(0, 10);
            m_InitOnStart_Custom.m_CustomListBank.AddContent(RandomResult);

            m_InitOnStart_Custom.m_LoopScrollRect.totalCount = m_InitOnStart_Custom.m_CustomListBank.GetListLength();
            m_InitOnStart_Custom.m_LoopScrollRect.RefreshCells();
        }

        private void OnButtonSetDataClick()
        {
            List<int> contents = new List<int>{
                3, 4, 5, 6, 7
            };

            m_InitOnStart_Custom.m_CustomListBank.SetContents(contents);
            m_InitOnStart_Custom.m_LoopScrollRect.totalCount = m_InitOnStart_Custom.m_CustomListBank.GetListLength();
            m_InitOnStart_Custom.m_LoopScrollRect.RefillCells();
        }

        private void OnButtonDelDataClick()
        {
            m_InitOnStart_Custom.m_CustomListBank.DelContentByIndex(0);
            m_InitOnStart_Custom.m_LoopScrollRect.ClearCells();
            m_InitOnStart_Custom.m_LoopScrollRect.totalCount = m_InitOnStart_Custom.m_CustomListBank.GetListLength();
            m_InitOnStart_Custom.m_LoopScrollRect.RefillCells();
        }

        private void OnButtonSortDataClick()
        {
            var TempContent = m_InitOnStart_Custom.m_CustomListBank.GetContents();
            TempContent.Sort((x, y) => x.CompareTo(y));

            m_InitOnStart_Custom.m_LoopScrollRect.ClearCells();
            m_InitOnStart_Custom.m_LoopScrollRect.totalCount = m_InitOnStart_Custom.m_CustomListBank.GetListLength();
            m_InitOnStart_Custom.m_LoopScrollRect.RefillCells();
        }

        private void OnButtonReverseSortDataClick()
        {
            var TempContent = m_InitOnStart_Custom.m_CustomListBank.GetContents();
            TempContent.Sort((x, y) => -x.CompareTo(y));

            m_InitOnStart_Custom.m_LoopScrollRect.ClearCells();
            m_InitOnStart_Custom.m_LoopScrollRect.totalCount = m_InitOnStart_Custom.m_CustomListBank.GetListLength();
            m_InitOnStart_Custom.m_LoopScrollRect.RefillCells();
        }
    }
}