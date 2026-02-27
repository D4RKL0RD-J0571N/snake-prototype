using System.Collections.Generic;
using UnityEngine;

namespace SnakePrototype.Utils
{
    /// <summary>
    /// Generic object pool for efficient memory management
    /// </summary>
    /// <typeparam name="T">Type of object to pool</typeparam>
    public class ObjectPool<T> where T : class, new()
    {
        private readonly Queue<T> _pool = new Queue<T>();
        private readonly System.Func<T> _createFunc;
        private readonly System.Action<T> _resetAction;
        private readonly int _maxSize;

        public int CountAll { get; private set; }
        public int CountActive { get; private set; }
        public int CountInactive => _pool.Count;

        public ObjectPool(System.Func<T> createFunc = null, System.Action<T> resetAction = null, int maxSize = 100)
        {
            _createFunc = createFunc ?? (() => new T());
            _resetAction = resetAction;
            _maxSize = maxSize;
        }

        public T Get()
        {
            if (_pool.Count > 0)
            {
                T item = _pool.Dequeue();
                CountActive++;
                return item;
            }
            
            T newItem = _createFunc();
            CountAll++;
            CountActive++;
            return newItem;
        }

        public void Release(T item)
        {
            if (item == null) return;
            
            if (CountActive <= 0) return;
            
            CountActive--;
            _resetAction?.Invoke(item);
            _pool.Enqueue(item);
        }

        public void Clear()
        {
            _pool.Clear();
            CountAll = 0;
            CountActive = 0;
        }

        public void Warm(int count)
        {
            for (int i = 0; i < count; i++)
            {
                T item = _createFunc();
                _resetAction?.Invoke(item);
                _pool.Enqueue(item);
            }
            
            CountAll += count;
        }
    }

    /// <summary>
    /// MonoBehaviour version of ObjectPool for Unity components
    /// </summary>
    /// <typeparam name="T">Component type to pool</typeparam>
    public class MonoBehaviourPool<T> where T : MonoBehaviour
    {
        private readonly Queue<T> _pool = new Queue<T>();
        private readonly GameObject _prefab;
        private readonly Transform _parent;
        private readonly int _maxSize;

        public int CountAll { get; private set; }
        public int CountActive { get; private set; }
        public int CountInactive => _pool.Count;

        public MonoBehaviourPool(GameObject prefab, Transform parent = null, int maxSize = 100)
        {
            _prefab = prefab;
            _parent = parent;
            _maxSize = maxSize;
        }

        public T Get()
        {
            if (_pool.Count > 0)
            {
                T item = _pool.Dequeue();
                item.gameObject.SetActive(true);
                CountActive++;
                return item;
            }
            
            GameObject newObj = Object.Instantiate(_prefab, _parent);
            T component = newObj.GetComponent<T>();
            if (component == null)
            {
                component = newObj.AddComponent<T>();
            }
            
            CountAll++;
            CountActive++;
            return component;
        }

        public void Release(T item)
        {
            if (item == null) return;
            
            if (CountActive <= 0) return;
            
            CountActive--;
            item.gameObject.SetActive(false);
            
            if (_parent != null)
            {
                item.transform.SetParent(_parent);
            }
            
            _pool.Enqueue(item);
        }

        public void Clear()
        {
            while (_pool.Count > 0)
            {
                T item = _pool.Dequeue();
                if (item != null && item.gameObject != null)
                {
                    Object.DestroyImmediate(item.gameObject);
                }
            }
            
            CountAll = 0;
            CountActive = 0;
        }

        public void Warm(int count)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject newObj = Object.Instantiate(_prefab, _parent);
                newObj.SetActive(false);
                T component = newObj.GetComponent<T>();
                if (component == null)
                {
                    component = newObj.AddComponent<T>();
                }
                _pool.Enqueue(component);
            }
            
            CountAll += count;
        }
    }
}
