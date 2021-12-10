using System.Collections.Generic;
using UnityEngine;

namespace SLibrary.NetSocket.Extensions
{
    /// <summary>
    /// 用于所有的Behavior类继承
    /// </summary>
    public class BaseBehaviour : MonoBehaviour
    {
        //    // 缓存的Component
        //    protected Map<string, Component> mComponents = new Map<string, Component>();
        //    // 缓存的GameObject
        //    protected GameObject mGameObject = null;

        /**
     * 启动的协程列表
     */
        protected List<long> mCoroutines = new List<long>();

        //    /// <summary>
        //    /// 获得一个组件，如果该组件不存在，则添加一个到对象上
        //    /// </summary>
        //    /// <typeparam name="T"></typeparam>
        //    /// <returns></returns>
        //    public T getOrAddComponent<T>() where T : Component
        //    {
        //        string key = typeof(T).Name;
        //        Component exists = mComponents.get(key);
        //        if (exists == null)
        //        {
        //            exists = GameObjectExt.getOrAddComponent<T>();
        //            mComponents.add(key, exists);
        //        }
        //        return (T)exists;
        //    }


        //    /// <summary>
        //    /// 只从缓存的组件中获取，如果该组件不存在，则添加一个到缓存中
        //    /// </summary>
        //    /// <typeparam name="T"></typeparam>
        //    /// <returns></returns>
        //    public T getComponent<T>() where T : Component
        //    {
        //        string key = typeof(T).Name;
        //        Component exists = mComponents.get(key);
        //        if (exists == null)
        //        {
        //            exists = GameObjectExt.GetComponent<T>();
        //            if (exists != null)
        //            {
        //                mComponents.add(key, exists);
        //            }
        //        }
        //        return (T)exists;
        //    }

        //    /// <summary>
        //    /// 缓存的Transform
        //    /// </summary>
        //    public Transform TransformExt
        //    {
        //        get { return getOrAddComponent<Transform>(); }
        //    }

        //    /// <summary>
        //    /// 当前gameObj
        //    /// </summary>
        //    public GameObject GameObjectExt
        //    {
        //        get
        //        {
        //            if (mGameObject == null){
        //                // Debug.LogError("------------");
        //                // Debug.LogError(this.gameObject);
        //                // Debug.LogError("------------");
        //                mGameObject = this.gameObject;
        //            }
        //            return mGameObject;
        //        }
        //    }

        //    public virtual void OnDrawGizmos()
        //    {
        //    }



        public virtual void Awake()
        {
        }

        //    public virtual void Start()
        //    {

        //    }

        public virtual void Update()
        {

        }

        //    public virtual void OnEnable()
        //    {

        //    }

        //    public virtual void OnApplicationPause(bool paused)
        //    {

        //    }

        //    public virtual void OnApplicationQuit()
        //    {

        //    }

        //    public virtual void OnDisable()
        //    {
        //        stopAllCoroutine();
        //    }

        protected virtual void OnDestroy()
        {
            stopAllCoroutine();
            removeAllListeners();
        }

        protected void stopAllCoroutine()
        {
            foreach (long id in mCoroutines)
            {
                //CoroutineManager.Singleton.stopCoroutine(id);
            }
            mCoroutines.Clear();
        }

        //    public virtual void OnLevelWasLoaded(int level)
        //    {

        //    }

        //    /// <summary>
        //    /// 延迟调用
        //    /// </summary>
        //    /// <param name="delayedTime"></param>
        //    /// <param name="callback"></param>
        //    /// <returns></returns>
        //    public long delayCall(float delayedTime, Action callback)
        //    {
        //        long ret = CoroutineManager.Singleton.delayedCall(delayedTime, callback);
        //        mCoroutines.Add(ret);
        //        return ret;
        //    }

        //    public long delayCall(float delayedTime, Action<object> callback, object param)
        //    {
        //        long ret = CoroutineManager.Singleton.delayedCall(delayedTime, callback, param);
        //        mCoroutines.Add(ret);
        //        return ret;
        //    }

        //    /// <summary>
        //    /// 取消一个延迟调用
        //    /// </summary>
        //    /// <param name="id"></param>
        //    public void cancelDelayCall(long id)
        //    {
        //        stopCoroutine(id);  
        //    }

        //    /// <summary>
        //    /// 启动一个协程
        //    /// </summary>
        //    /// <param name="co"></param>
        //    /// <returns></returns>
        //    public long startCoroutine(IEnumerator co)
        //    {
        //        long ret = CoroutineManager.Singleton.startCoroutine(co);
        //        mCoroutines.Add(ret);
        //        return ret;
        //    }

        //    /// <summary>
        //    /// 停止一个协程
        //    /// </summary>
        //    /// <param name="id"></param>
        //    public void stopCoroutine(long id)
        //    {
        //        CoroutineManager.Singleton.stopCoroutine(id);
        //        int idx = mCoroutines.IndexOf(id);
        //        if (idx >= 0)
        //        {
        //            mCoroutines.RemoveAt(idx);
        //        }
        //    }

        //    /// <summary>
        //    /// 刷新界面 
        //    /// </summary>
        //    public virtual void refreshView( object obj = null )
        //    {

        //    }

        //    //protected virtual T getActorByCollider<T>(Collider other) where T : ActorBase 
        //    //{
        //    //    ActorBehavior behavior = other.GetComponent<ActorBehavior>();
        //    //    if(behavior == null && other.transform != null && other.transform.parent != null)
        //    //    {
        //    //        behavior = other.transform.parent.GetComponent<ActorBehavior>();
        //    //    }
        //    //    if (behavior != null)
        //    //        return behavior.Owner as T;
        //    //    return null;
        //    //}

        //    //protected virtual T getActorByGameObj<T>(GameObject obj) where T : ActorBase
        //    //{
        //    //    ActorBehavior behavior = obj.GetComponent<ActorBehavior>();
        //    //    if (behavior != null)
        //    //        return behavior.Owner as T;
        //    //    return null;
        //    //}

        //    /// <summary>
        //    /// 根据路径获取子节点 
        //    /// </summary>
        //    /// <param name="path"></param>
        //    /// <returns></returns>
        //    public Transform getChild(string path)
        //    {
        //        return TransformExt.Find(path);
        //    }

        //    /// <summary>
        //    /// 获取特效节点
        //    /// </summary>
        //    /// <param name="path"></param>
        //    /// <returns></returns>
        //    public GameObject getEffectChild(string path)
        //    {
        //        GameObject res = null;
        //        Transform trans = TransformExt.Find(path);
        //        if (trans != null)
        //        {
        //            EffectConfigHolder comp = trans.GetComponent<EffectConfigHolder>();
        //            if (comp != null)
        //                res = comp.EffectObj;
        //            else
        //                Logger.err("无法找到特效组件：" + path);
        //        }
        //        else
        //        {
        //            Logger.err("无法找到特效节点：" + path);
        //        }
        //        return res;
        //    } 

        //    private Dictionary<int, List<Action<BaseEvent>>> evtMap = new Dictionary<int, List<Action<BaseEvent>>>();
        //    /// <summary>
        //    /// 添加事件
        //    /// </summary>
        //    protected void addListener( EventId evtId, Action<BaseEvent> callBack )
        //    {
        //        addListener( (int)evtId, callBack );
        //    }

        //    /// <summary>
        //    /// 添加事件
        //    /// </summary>
        //    protected void addListener( int evtId, Action<BaseEvent> callBack )
        //    {
        //        GED.Singleton.ED.addListener( evtId, callBack );
        //        if( !evtMap.ContainsKey( evtId ) )
        //            evtMap.Add( evtId, new List<Action<BaseEvent>>() );
        //        if( !evtMap[evtId].Contains( callBack ) )
        //            evtMap[evtId].Add( callBack );
        //    }

        //    /// <summary>
        //    /// 移除事件
        //    /// </summary>
        //    protected void removeListener( EventId evtId, Action<BaseEvent> callBack )
        //    {
        //        removeListener( (int)evtId, callBack );
        //    }

        //    /// <summary>
        //    /// 移除事件
        //    /// </summary>
        //    protected void removeListener( int evtId, Action<BaseEvent> callBack )
        //    {
        //        GED.Singleton.ED.removeListener( (int)evtId, callBack );
        //        if( evtMap.ContainsKey( evtId ) )
        //        {
        //            if( evtMap[evtId].Contains( callBack ) )
        //                evtMap[evtId].Remove( callBack );
        //        }
        //    }

        protected void removeAllListeners()
        {
            //foreach (int id in evtMap.Keys) {
            //    if (evtMap.ContainsKey(id)) {
            //        List<Action<BaseEvent>> list = evtMap[id];
            //        foreach (Action<BaseEvent> act in list)
            //            GED.Singleton.ED.removeListener(id, act);
            //    }
            //}
            //evtMap.Clear();
        }
    }
}


