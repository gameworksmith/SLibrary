using DG.Tweening;
using UnityEngine;
#if USE_DOTWEEN
#endif

namespace SLibrary.Tweener
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseTweener : MonoBehaviour, ITweener
    {
        public GameObject Target { get; set; }
        public float Duration { get; set; }
        public bool IgnoreTimeScale { get; set; }
        public int EaseType { get; set; }
        public bool PlayOnAwake { get; set; }
        public AnimationCurve Curve { get; set; }


        

        [Header("目标")]
        [SerializeField]
        private GameObject target;
        
        [Header("时长（秒）")]
        [SerializeField]
        public float duration = 1;
        
        
        [Header("忽略时间常数")]
        [SerializeField]
        private bool ignoreTimeScale;
        
        [Header("播放速率")]
        [SerializeField]
        private float timeScale = 1;
        
        [Header("初始延迟（秒）")]
        [SerializeField]
        private float startDelay = 0;
        
        [Header("重复播放次数")]
        public int loopTimes = 1;
        
        /// <summary>
        /// 设置为0使用自定义曲线
        /// </summary>
        [HideInInspector]
        [Header("曲线类型")]
        [SerializeField]
        public int easeType = 1;
        
        [Header("是否开启即播放")]
        [SerializeField]
        private bool playOnAwake = true;
        
        [HideInInspector]
        [Header("自定义曲线")]
        [SerializeField]
        public AnimationCurve animationCurve;
        
#if USE_DOTWEEN
        private Tween _tween;
#endif
        /// <summary>
        /// 用于做初始化
        /// </summary>
        protected virtual void Init()
        {
        }

        private void OnEnable()
        {
            if (PlayOnAwake)
            {
                TweenAction();
            }
        }

        private void TweenAction()
        {
#if USE_DOTWEEN
            ProgressSetter(0);
            _tween?.Kill(true);
            _tween = CreateTween();
#endif
        }

#if USE_DOTWEEN
        /// <summary>
        /// 创建Tween
        /// </summary>
        /// <returns></returns>
        protected virtual Tween CreateTween()
        {
            Tween ret = DOTween.To(ProgressSetter, 0, 1, Duration);
            ret.SetLoops(loopTimes);
            ret.timeScale = timeScale;
            ret.OnUpdate(OnUpdate);
            ret.SetDelay(startDelay);
            if (IgnoreTimeScale)
            {
                ret.SetUpdate(true);
            }

            if (EaseType == (int)Ease.INTERNAL_Custom)
            {
                ret.SetEase(Curve);
            }
            else
            {
                ret.SetEase((Ease) EaseType);
            }
            ret.OnComplete(OnComplete);
            return ret;
        }


#endif
        
        protected virtual void OnUpdate()
        {
        }
        
        protected virtual void OnComplete()
        {
        }
        
        protected virtual void ProgressSetter(float pnewvalue)
        {
        }

        private void OnDisable()
        {
#if USE_DOTWEEN
            _tween?.Kill(true);
#endif
        }

        private void Awake()
        {
            SetParams();
            Init();
        }

        private void SetParams()
        {
            IgnoreTimeScale = ignoreTimeScale;
            Duration = duration;
            PlayOnAwake = playOnAwake;
            Target = target;
            EaseType = easeType;
            Curve = animationCurve;
        }

    }
}