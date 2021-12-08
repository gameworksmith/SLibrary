using System;
using System.Collections.Generic;
using SLibrary.Core;
using SLibrary.Interfaces;
using UnityEngine;
using Object = UnityEngine.Object;


namespace SLibrary.SimpleEntity {
    public class EntityPoolCenter: MonoBehaviour
    {
        private static EntityPoolCenter _instance;
        public static EntityPoolCenter Instance { get; private set; }

        private readonly Dictionary<int, EntityPool> _pools = new Dictionary<int, EntityPool>();

        public IGameLoop GameLoop;
        public EntityPool GetPool(int poolId)
        {
            if (_pools.TryGetValue(poolId, out var pool))
            {
                return pool;
            }
            pool = new EntityPool();
            _pools.Add(poolId, pool);
            return pool;
        }

        private void Awake()
        {
            Instance = this;
            GameLoop = new UniqueGameLoop();
        }

        protected virtual void Update()
        {
            GameLoop.Update();
        }

        protected void LateUpdate()
        {
            GameLoop.LateUpdate();
        }


        // 跨池发送信息
        public void SendToPool(int poolId, int actionId, EntityParam param = null, bool autoRelease = true)
        {
            if (!_pools.TryGetValue(poolId, out var pool))
            {
                return;
            }
            pool.Deal(actionId, param, autoRelease);
        }

        public void ResetPool(int poolId)
        {
            if (!_pools.TryGetValue(poolId, out var pool))
            {
                return;
            }
            pool.Reset();
        }

        public void RemovePool(int poolId)
        {
            _pools.Remove(poolId);
        }

        public static T CreateEntity<T> (int poolId, GameObject owner = null, object param = null) where T : EntityBase, new()
        {
            // 先设置到pool下，如果要修改parent再在代码里说
            var pool = Instance.GetPool(poolId);
            var instance = new T {Owner = owner, PoolInstance = pool};
            // var instance = go.AddComponent<T>();
            instance.GameLoop = Instance.GameLoop;
            instance.Register();
            instance.SelfInit(param);
            return instance;
        }

        public static bool RegisterExistEntity<T>(T instance, int poolId, GameObject owner = null, object param = null) where  T:EntityBase
        {

            try
            {
                var pool = Instance.GetPool(poolId);
                instance.Owner = owner;
                instance.PoolInstance = pool;
                instance.GameLoop = Instance.GameLoop;
                instance.Register();
                instance.SelfInit(param);
            }
            catch (Exception e)
            {
                Debug.Log("registerError");
                return false;
            }
            return true;

        }
    }
    public class EntityPool : IEntityPool {

        
        private readonly List<IEntity> _entities = new List<IEntity>();

        private EntityParamPool _paramPool = new EntityParamPool();
        #region IEntityPool implementation

        public void Register (IEntity entity) {
            _entities.Add(entity);
            entity.ParamPool = _paramPool;
        }

        public void UnRegister (IEntity entity) {
            _entities.Remove(entity);
        }

        public void UnRegisterAll () {
            for (int i = 0; i < _entities.Count; i++) {
                _entities[i].Destroy();
            }
            _entities.Clear();
        }

        public void Reset () {
            _paramPool.Clear();
            UnRegisterAll();
        }

        public void Deal(int actionId, EntityParam param, bool autoRelease) {
            //Debug.LogFormat("Deal {0} action {1} {2} {3}", entity.GetType().Name, actionName, args[0], args[1]);
            for (int i = 0; i < _entities.Count; i++)
            {
                if (_entities[i] == null)
                {
                    continue;
                }


                _entities[i].Receive(actionId, param);
                // 如果已经被释放了就没必要再传其他的
                if (param != null && !param.IsUsing)
                {
                    continue;
                }
            }

            if (autoRelease)
            {
                param?.Release();
            }
        }



        #endregion
    }
}