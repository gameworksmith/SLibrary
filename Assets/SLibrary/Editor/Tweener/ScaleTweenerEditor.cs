#if UNITY_EDITOR

using SLibrary.Tweener;
using UnityEditor;
using UnityEngine;

namespace SLibrary.Editor.Tweener
{
    [CustomEditor(typeof(ScaleTweener))]
    public class ScaleTweenerEditor : BaseTweenerEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("复制当前缩放到初始"))
            {
                ScaleTweener tweener = target as ScaleTweener;
                tweener.StartScale = tweener.transform.localScale;
            }
            
            if (GUILayout.Button("复制当前缩放到结束"))
            {
                ScaleTweener tweener = target as ScaleTweener;
                tweener.EndScale = tweener.transform.localScale;
            }
            
            if (GUILayout.Button("设置为初始缩放"))
            {
                ScaleTweener tweener = target as ScaleTweener;
                tweener.transform.localScale = tweener.StartScale;
            }
            
            if (GUILayout.Button("设置为结束缩放"))
            {
                ScaleTweener tweener = target as ScaleTweener;
                tweener.transform.localScale = tweener.EndScale;
            }
        }
    }
}
#endif
