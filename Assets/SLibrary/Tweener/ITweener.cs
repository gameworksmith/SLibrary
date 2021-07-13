using UnityEngine;

namespace SLibrary.Tweener
{
    public interface ITweener
    {
        /// <summary>
        /// 目标
        /// </summary>
        GameObject Target { get; set; }
        
        float Duration { get; set; }
        
        /// <summary>
        /// 是否忽略时间常数
        /// </summary>
        bool IgnoreTimeScale { get; set; }
        
        /// <summary>
        /// 曲线类型，会根据使用不同的Tween来处理
        /// </summary>
        int EaseType { get; set; }
        
        /// <summary>
        /// 是否每次启动都播放
        /// </summary>
        bool PlayOnAwake { get; set; }
        
        
        /// <summary>
        /// 自定义曲线
        /// </summary>
        AnimationCurve Curve { get; set; }
        
    }
}