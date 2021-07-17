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
            if (GUILayout.Button("复制当前位置到初始"))
            {
                Rect2DMoveTweener tweener = target as Rect2DMoveTweener;
                var rectTrans = tweener.transform as RectTransform;
                tweener.StartPos = rectTrans.anchoredPosition;
            }

            if (GUILayout.Button("复制当前位置到结束"))
            {
                Rect2DMoveTweener tweener = target as Rect2DMoveTweener;
                var rectTrans = tweener.transform as RectTransform;
                tweener.EndPos = rectTrans.anchoredPosition;
            }

            if (GUILayout.Button("移动到为初始位置"))
            {
                Rect2DMoveTweener tweener = target as Rect2DMoveTweener;
                var rectTrans = tweener.transform as RectTransform;
                rectTrans.anchoredPosition = tweener.StartPos;
            }

            if (GUILayout.Button("移动到为结束位置"))
            {
                Rect2DMoveTweener tweener = target as Rect2DMoveTweener;
                var rectTrans = tweener.transform as RectTransform;
                rectTrans.anchoredPosition = tweener.EndPos;
            }
        }
    }
}