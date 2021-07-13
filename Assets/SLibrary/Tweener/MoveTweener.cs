using SLibrary.UITweener;
using UnityEngine;

namespace SLibrary.Tweener
{
    public class MoveTweener : BaseTweener
    {
        [Header("初始位置")]
        public Vector3 StartPos;
        [Header("结束位置")]
        public Vector3 EndPos;

        private Transform _transform;
        protected override void Init()
        {
            base.Init();
            _transform = Target.GetComponent<Transform>();
        }

#if USE_DOTWEEN
        protected override void ProgressSetter(float pnewvalue)
        {
            _transform.position = StartPos + (EndPos - StartPos) * pnewvalue;
        }

#endif
    }
}