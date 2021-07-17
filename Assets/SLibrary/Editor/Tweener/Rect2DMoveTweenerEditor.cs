using SLibrary.Tweener;
using UnityEditor;
using UnityEngine;

namespace SLibrary.Editor.Tweener
{
    [CustomEditor(typeof(Rect2DMoveTweener))]
    public class Rect2DMoveTweenerEditor : BaseTweenerEditor
    {
        public override void OnInspectorGUI()
        {
            
            base.OnInspectorGUI();
            if (GUILayout.Button("设为当前位置"))
            {
                Rect2DMoveTweener tweener = target as Rect2DMoveTweener;
                var rectTrans = tweener.transform as RectTransform;
                tweener.StartPos = rectTrans.anchoredPosition;
                tweener.EndPos = rectTrans.anchoredPosition;
            }
        }
    }
}