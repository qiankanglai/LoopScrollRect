using UnityEngine;
using System.Collections;

namespace SG
{
    [RequireComponent(typeof(UnityEngine.UI.LoopScrollRect))]
    [DisallowMultipleComponent]
    public class InitOnStart : MonoBehaviour
    {
        void Start()
        {
            GetComponent<UnityEngine.UI.LoopScrollRect>().RefillCells();
        }
    }
}