using System;
using System.Collections.Generic;
using UnityEngine;

namespace SLibrary.SimpleEntity
{
    public class EntityParamPool
    {
        /// <summary>
        /// 公用方法
        /// </summary>
        private List<EntityParam> _params = new List<EntityParam>();

        public void Clear()
        {
            foreach (var entityParam in _params)
            {
                entityParam.Release();
            }

            _params.Clear();
        }


        public EntityParam GetParam()
        {
            foreach (var param in _params)
            {
                if (!param.IsUsing)
                {
                    param.IsUsing = true;
                    return param;
                }
            }

            EntityParam entityParam = new EntityParam {IsUsing = true};
            _params.Add(entityParam);
            return entityParam;
        }
    }

    // 简单对象池获取参数类
    // 为参数基类
    public class EntityParam
    {
        private bool _using = false;

        private Dictionary<string, long> _longs = new Dictionary<string, long>();
        private Dictionary<string, Enum> _enums = new Dictionary<string, Enum>();
        private Dictionary<string, float> _floats = new Dictionary<string, float>();
        private Dictionary<string, string> _strs = new Dictionary<string, string>();
        private Dictionary<string, int> _bools = new Dictionary<string, int>();

        private Dictionary<string, Color> _colors = new Dictionary<string, Color>();

        //这就包含了vector2和vector3
        private Dictionary<string, Vector4> _vectors = new Dictionary<string, Vector4>();

        private Dictionary<string, object> _objs = new Dictionary<string, object>();

        public bool IsUsing
        {
            get => _using;
            set => _using = value;
        }

        public long GetLong(string key)
        {
            return _longs.TryGetValue(key, out var result) ? result : 0;
        }

        public string GetString(string key)
        {
            return _strs.TryGetValue(key, out var result) ? result : "";
        }

        public int GetInt(string key)
        {
            return (int) GetLong(key);
        }

        public Enum GetEnum(string key)
        {
            return _enums.TryGetValue(key, out var result) ? result : default;
        }

        public float GetFloat(string key)
        {
            return _floats.TryGetValue(key, out var result) ? result : 0;
        }

        public bool GetBool(string key)
        {
            if (_bools.TryGetValue(key, out var result))
            {
                return result == 1;
            }

            return false;
        }

        public Color GetColor(string key)
        {
            return _colors.TryGetValue(key, out var result) ? result : default;
        }


        public Vector4 GetVector(string key)
        {
            return _vectors.TryGetValue(key, out var result) ? result : default;
        }

        public object GetObj(string key)
        {
            return _objs.TryGetValue(key, out var result) ? result : null;
        }

        public bool HasKey(string key)
        {
            return _longs.ContainsKey(key) || _enums.ContainsKey(key) || _floats.ContainsKey(key) ||
                   _strs.ContainsKey(key)
                   || _colors.ContainsKey(key) || _bools.ContainsKey(key) || _vectors.ContainsKey(key) ||
                   _objs.ContainsKey(key);
        }

        public void Release()
        {
            _longs.Clear();
            _enums.Clear();
            _floats.Clear();
            _strs.Clear();
            _colors.Clear();
            _bools.Clear();
            _vectors.Clear();
            _objs.Clear();
            _using = false;
        }

        public void Add(string key, long value)
        {
            try
            {
                if (_longs.ContainsKey(key))
                {
                    _longs[key] = value;
                }
                else
                {
                    _longs.Add(key, value);
                }
            }
            catch (Exception e)
            {
                Debug.Log($"添加失败 {e}");
            }
        }

        public void Add(string key, float value)
        {
            try
            {
                if (_floats.ContainsKey(key))
                {
                    _floats[key] = value;
                }
                else
                {
                    _floats.Add(key, value);
                }
            }
            catch (Exception e)
            {
                Debug.Log($"添加失败 {e}");
            }
        }

        public void Add(string key, string value)
        {
            try
            {
                if (_strs.ContainsKey(key))
                {
                    _strs[key] = value;
                }
                else
                {
                    _strs.Add(key, value);
                }
            }
            catch (Exception e)
            {
                Debug.Log($"添加失败 {e}");
            }
        }

        public void Add(string key, Color value)
        {
            try
            {
                if (_colors.ContainsKey(key))
                {
                    _colors[key] = value;
                }
                else
                {
                    _colors.Add(key, value);
                }
            }
            catch (Exception e)
            {
                Debug.Log($"添加失败 {e}");
            }
        }

        // 布尔因为unity的特性不能存，所以用int来表示
        // addBool要单独一个api的原因是UnityEngine.Object会被自动转换为bool，坑死了
        public void AddBool(string key, bool value)
        {
            try
            {
                if (_bools.ContainsKey(key))
                {
                    _bools[key] = value ? 1 : -1;
                }
                else
                {
                    _bools.Add(key, value ? 1 : -1);
                }
            }
            catch (Exception e)
            {
                Debug.Log($"添加失败 {e}");
            }
        }

        public void Add(string key, Enum eu)
        {
            try
            {
                if (_enums.ContainsKey(key))
                {
                    _enums[key] = eu;
                }
                else
                {
                    _enums.Add(key, eu);
                }
            }
            catch (Exception e)
            {
                Debug.Log($"添加失败 {e}");
            }
        }

        public void Add(string key, Vector4 value)
        {
            try
            {
                if (_vectors.ContainsKey(key))
                {
                    _vectors[key] = value;
                }
                else
                {
                    _vectors.Add(key, value);
                }
            }
            catch (Exception e)
            {
                Debug.Log($"添加失败 {e}");
            }
        }

        public void Add(string key, object value)
        {
            try
            {
                if (_objs.ContainsKey(key))
                {
                    _objs[key] = value;
                }
                else
                {
                    _objs.Add(key, value);
                }
            }
            catch (Exception e)
            {
                Debug.Log($"添加失败 {e}");
            }
        }

        #region 静态方法，用于外部调用

        #endregion
    }
}