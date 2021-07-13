using UnityEngine;

namespace SLibrary.Tweener
{
    public class Rect2DMoveTweener : BaseTweener
    {
        [Header("初始位置")]
        public Vector2 StartPos;
        [Header("结束位置")]
        public Vector2 EndPos;

        private RectTransform _transform;
        protected override void Init()
        {
            base.Init();
            _transform = Target.GetComponent<RectTransform>();
        }

#if USE_DOTWEEN
        protected override void ProgressSetter(float pnewvalue)
        {
            _transform.anchoredPosition = StartPos + (EndPos - StartPos) * pnewvalue;
        }
#endif
    }
}