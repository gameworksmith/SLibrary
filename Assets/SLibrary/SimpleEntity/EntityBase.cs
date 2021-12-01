using System;
using System.Collections.Generic;
using SLibrary.Interfaces;
using UnityEngine;

namespace SLibrary.SimpleEntity {
    public class EntityBase : IEntity {
        public IEntityPool PoolInstance { set; private get; }
        public IGameLoop GameLoop;
        public GameObject Owner { set; private get; }

        public EntityBase()
        {
            
        }
        
        private readonly Dictionary<int, System.Action<EntityParam>> _receiveActions = new Dictionary<int, Action<EntityParam>>();



        #region IEntity implementation

        public virtual void Destroy()
        {
        }

        public EntityParamPool ParamPool { get; set; }

        public void SelfInit(object param = null)
        {
            this.OnCreate(param);
            this.RegisterReceiveActions();
        }
        /// <summary>
        /// 向实体池注册此实体。同时将OnUpdate给予UpdateDispatcher，优化update性能。
        /// </summary>
        public void Register () {
            PoolInstance.Register(this);
            GameLoop.AddListener(OnUpdate, 1);
        }

        /// <summary>
        /// 向实体池反注册此实体
        /// </summary>
        public void UnRegister () {
            _receiveActions.Clear();
            PoolInstance?.UnRegister(this);
            GameLoop.RemoveListener(OnUpdate);
            OnRemove();
        }

        /// <summary>
        /// 发送事件信息
        /// </summary>
        /// <param name="actionId">事件名</param>
        /// <param name="param">事件参数</param>
        /// <param name="autoRelease"></param>
        public void Send (int actionId, EntityParam param = null, bool autoRelease = true) {
            // LogManager.Log($"{actionId} 消息传递");
            PoolInstance.Deal(actionId, param, autoRelease);
        }

        public EntityParam GetParam()
        {
            return ParamPool.GetParam();
        }

        /// <summary>
        /// 接收事件信息
        /// </summary>
        /// <param name="actionId">事件名</param>
        /// <param name="param">事件参数</param>
        public void Receive(int actionId, EntityParam param) {
            if (!_receiveActions.ContainsKey(actionId)) return;
            _receiveActions[actionId](param);
        }


        /// <summary>
        /// 替代以前的receive，直接添加方法
        /// </summary>
        protected virtual void RegisterReceiveActions() {
        }

        protected virtual void OnCreate (object param = null) { }

        protected virtual void OnRemove () { }
        #endregion

        protected virtual void OnDestroyed () { }

        /// <summary>
        /// 用来处理Update相关的功能，放到UpdateDispatcher里面
        /// </summary>
        protected virtual void OnUpdate() {
            
        }
        
        protected virtual void LateUpdate() {
            
        }

        /// <summary>
        /// 在重载中调用
        /// </summary>
        /// <param name="actionId"></param>
        /// <param name="action"></param>
        protected void SubscribeAction(int actionId, System.Action<EntityParam> action) {
            if (!_receiveActions.ContainsKey(actionId)) {
                _receiveActions.Add(actionId, action);
            }
        }

        private void OnDestroy () {
            UnRegister();
            OnDestroyed();
        }
    }
}
