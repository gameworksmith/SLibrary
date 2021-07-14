using UnityEngine;

namespace SLibrary.Tweener
{
    public class ScaleTweener : BaseTweener
    {

        [Header("初始缩放")]
        public Vector3 StartScale;
        [Header("最终缩放")]
        public Vector3 EndScale;


        private Transform _transform;
        
        protected override void Init()
        {
            base.Init();
            _transform = Target.GetComponent<Transform>();
        }
        

#if USE_DOTWEEN
        protected override void ProgressSetter(float pnewvalue)
        {
            _transform.localScale = StartScale + (EndScale - StartScale) * pnewvalue;
        }
#endif
    }
}