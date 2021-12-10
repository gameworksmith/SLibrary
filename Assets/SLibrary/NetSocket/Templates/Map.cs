using System;
using System.Collections;
using System.Collections.Generic;

namespace SLibrary.NetSocket.Templates
{

    /// <summary>
    /// Dictonary
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    public class Map<K, V> where K : class
    {
        protected Dictionary<K, V> mContainer;

        public Map()
        {
            mContainer = new Dictionary<K, V>();
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public void add(K key, V val)
        {
            if (!mContainer.ContainsKey(key))
                mContainer.Add(key, val);
            else
                mContainer[key] = val;
        }

        /// <summary>
        /// 是否包含键
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(K key)
        {
            return mContainer.ContainsKey(key);
        }

        /// <summary>
        /// 是否包含值
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool ContainsValue(V val)
        {
            return mContainer.ContainsValue(val);
        }

        /// <summary>
        /// 返回
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public V get(K key)
        {
            V ret;
            return mContainer.TryGetValue(key, out ret) ? ret : default(V);
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key"></param>
        public void remove(K key)
        {
            if (mContainer.ContainsKey(key))
                mContainer.Remove(key);
        }

        /// <summary>
        /// 返回内部容器
        /// </summary>
        public Dictionary<K, V> Container
        {
            get { return mContainer; }
        }

        public int Count
        {
            get
            {
                if (mContainer != null)
                    return mContainer.Count;
                return 0;
            }
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void clear()
        {
            mContainer.Clear();
        }

        /// <summary>
        /// 遍历所有元素，执行action, 不可删除
        /// </summary>
        public void runAction(Action<KeyValuePair<K, V>> action)
        {
            IEnumerator it = mContainer.GetEnumerator();
            while (it.MoveNext())
            {
                KeyValuePair<K, V> kvPair = (KeyValuePair<K, V>) it.Current;
                action(kvPair);
            }

        }

        /// <summary>
        /// 重设值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public void reset(K key, V val)
        {
            mContainer[key] = val;
        }
    }

}