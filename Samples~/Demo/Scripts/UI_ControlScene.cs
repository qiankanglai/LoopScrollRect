using UnityEngine;
using UnityEngine.UI;

namespace Demo
{
    public class UI_ControlScene : MonoBehaviour
    {
        public Button m_ButtonFPS;
        public InputField m_InputField_FPS;

        private void Awake()
        {
            m_ButtonFPS.onClick.AddListener(OnButtonFPSClick);
        }

        private void OnButtonFPSClick()
        {
            int FPS = 0;
            int.TryParse(m_InputField_FPS.text, out FPS);

            int TargetCount = Mathf.Max(1, FPS);
            Application.targetFrameRate = FPS;
        }
    }
}