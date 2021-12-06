#if UNITY_EDITOR

using SLibrary.Tweener;
using UnityEditor;

namespace SLibrary.Editor.Tweener
{
    [CustomEditor(typeof(TextPrinterTweener))]
    public class TextPrinterTweenerEditor : BaseTweenerEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}
#endif
