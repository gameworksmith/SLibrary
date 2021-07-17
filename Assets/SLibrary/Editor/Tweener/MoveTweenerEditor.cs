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
            if (GUILayout.Button("设为当前位置"))
            {
                MoveTweener tweener = target as MoveTweener;
                tweener.StartPos = tweener.transform.position;
                tweener.EndPos = tweener.transform.position;
            }
        }
    }
}