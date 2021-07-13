using SLibrary.Tweener;
using UnityEditor;

#if USE_DOTWEEN
using DG.Tweening;
#endif

namespace SLibrary.Editor.Tweener
{
    [CustomEditor(typeof(BaseTweener))]
    public class BaseTweenerEditor : UnityEditor.Editor
    {
#if USE_DOTWEEN
        private static int[] _values;
        private static string[] _displays;

#endif
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            BaseTweener tweener = target as BaseTweener;
            if (_values == null || _displays == null)
            {
                _displays = GetDisplayEaseType(out _values);
            }
            tweener.easeType = EditorGUILayout.IntPopup("时间曲线类型", tweener.easeType, _displays, _values);
            if (tweener.easeType == _values[_values.Length - 1])
            {
                tweener.animationCurve =  EditorGUILayout.CurveField("自定义动画曲线", tweener.animationCurve);
            }
        }

#if USE_DOTWEEN
        public static string[] GetDisplayEaseType(out int[] values)
        {
            string[] allNames = System.Enum.GetNames(typeof(Ease));
            values = new int[allNames.Length];
                
            var easeArray = System.Enum.GetValues(typeof(Ease));
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = (int)easeArray.GetValue(i);
            }

            return  allNames;
        }
#else
        public string[] GetDisplayEaseType()
        {
            return new string[] {"0", "1", "2"};
        }
#endif
    }
}