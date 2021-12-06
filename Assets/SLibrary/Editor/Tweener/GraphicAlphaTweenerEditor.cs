#if UNITY_EDITOR

using SLibrary.Tweener;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace SLibrary.Editor.Tweener
{
    [CustomEditor(typeof(GraphicAlphaTweener))]
    public class GraphicAlphaTweenerEditor : BaseTweenerEditor
    {
        private static void SetGraphicAlpha(Graphic graphic, float alpha)
        {
            var color = graphic.color;
            color = new Color(color.r, color.g, color.b, alpha);
            graphic.color = color;
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("复制当前alpha到初始"))
            {
                GraphicAlphaTweener tweener = target as GraphicAlphaTweener;
                var graphics = tweener.GetComponent<Graphic>();
                tweener.StartAlpha = graphics.color.a;
            }
            
            if (GUILayout.Button("复制当前alpha到结束"))
            {
                GraphicAlphaTweener tweener = target as GraphicAlphaTweener;
                var graphics = tweener.GetComponent<Graphic>();
                tweener.EndAlpha = graphics.color.a;
            }

            if (GUILayout.Button("设置为初始alpha"))
            {
                GraphicAlphaTweener tweener = target as GraphicAlphaTweener;
                var graphics = tweener.GetComponent<Graphic>();
                SetGraphicAlpha(graphics, tweener.StartAlpha);
                
            }
            
            if (GUILayout.Button("设置为结束alpha"))
            {
                GraphicAlphaTweener tweener = target as GraphicAlphaTweener;
                var graphics = tweener.GetComponent<Graphic>();
                SetGraphicAlpha(graphics, tweener.EndAlpha);
            }
        }
    }
}
#endif