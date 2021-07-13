using SLibrary.Editor.UITweener.Editor;
using SLibrary.Tweener;
using UnityEditor;

namespace SLibrary.Editor.Tweener.Editor
{
    [CustomEditor(typeof(MoveTweener))]
    public class MoveTweenerEditor : BaseTweenerEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}