using UnityEngine;

namespace MobileConsole
{
    public class MobileGUIBase : MonoBehaviour {
        public bool                 Visible = false;
        public static float         ScreenResolution = 960f;
        [SerializeField]
        protected int               CustonFontSize = 20;

        protected float             BottomSpacing = 100f;
        public static int GetSizeAdaptScreen(int originSize) {
            float factor = Screen.height / ScreenResolution;
            int resultSize = (int) (factor*originSize);
            return resultSize;
        }
    }
}
