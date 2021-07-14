using SLibrary.Tweener;
using UnityEngine;
using UnityEngine.UI;

namespace SLibrary.Tweener{
    public class GraphicAlphaTweener : BaseTweener
    {
        [Header("初始alpha")]
        public float StartAlpha;
        [Header("结束alpha")]
        public float EndAlpha;

        private Graphic _graphics;
        
        protected override void Init()
        {
            base.Init();
            _graphics = Target.GetComponent<MaskableGraphic>();
        }

#if USE_DOTWEEN
        protected override void ProgressSetter(float pnewvalue)
        {
            float alpha = StartAlpha + (EndAlpha - StartAlpha) * pnewvalue;
            var color = _graphics.color;
            color = new Color(color.r, color.g, color.b, alpha);
            _graphics.color = color;
        }
#endif
    }
}