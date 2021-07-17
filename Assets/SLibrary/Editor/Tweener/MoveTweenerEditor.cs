using SLibrary.Tweener;
using UnityEditor;
using UnityEngine;

namespace SLibrary.Editor.Tweener
{
    [CustomEditor(typeof(MoveTweener))]
    public class MoveTweenerEditor : BaseTweenerEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("复制当前位置"))
            {
                MoveTweener tweener = target as MoveTweener;
                tweener.StartPos = tweener.transform.position;
                tweener.EndPos = tweener.transform.position;
            }
            
            if (GUILayout.Button("移动到为初始位置"))
            {
                MoveTweener tweener = target as MoveTweener;
                tweener.transform.position = tweener.StartPos;
            }
            
            if (GUILayout.Button("移动到为结束位置"))
            {
                MoveTweener tweener = target as MoveTweener;
                tweener.transform.position = tweener.EndPos;
            }

        }
    }
}