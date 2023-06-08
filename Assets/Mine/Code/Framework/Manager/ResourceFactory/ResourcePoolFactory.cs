using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Mine.Code.Framework.Manager.ResourceFactory
{
    public class ResourcePoolFactory<T> : ResourceFactory<T> where T : Object
    {
        #region Constructor

        public ResourcePoolFactory(string path, bool isAddressable, bool isInject, int poolSize) : base(path, isAddressable, isInject) => AddPool(poolSize).Forget();

        #endregion

        #region Fields

        readonly List<T> pool = new();
        Transform container;

        #endregion

        #region Properties
    

        #endregion

        #region Constructor

        async UniTaskVoid AddPool(int poolSize)
        {
            container = new GameObject($"Pool[{path}]").transform;

            for (int i = 0; i < poolSize; i++)
            {
                Enqueue(await base.LoadAsync());
            }
        }

        #endregion

        #region Override Methods

        public override async UniTask<T> LoadAsync(Transform parent = null)
        {
            if (TryDequeue(out var instance))
            {
                if (instance is Component component)
                {
                    component.transform.SetParent(parent);
                    component.gameObject.SetActive(true);
                }
                else if (instance is GameObject gameObject)
                {
                    gameObject.transform.SetParent(parent);
                    gameObject.SetActive(true);
                }
                
                return instance;
            }
            else
            {
                instance = await base.LoadAsync(parent);

                if (instance == null) return instance;

                if (instance is Component component) component.gameObject.SetActive(true);
                else if (instance is GameObject gameObject) gameObject.SetActive(true);
                
                return instance;
            }
        }
    
        public override void Release(T instance) => Enqueue(instance);

        #endregion

        #region Private Methods

        void Enqueue(T instance)
        {
            pool.Add(instance);

            if (instance is Component component)
            {
                component.gameObject.SetActive(false);
                component.transform.SetParent(container);
                component.OnDestroyAsObservable().Subscribe(_ =>
                {
#if ADDRESSABLE_SUPPORT
                    if(isAddressable) Addressables.Release(instance);
#endif
                    pool.Remove(instance);
                });
            }
            else if (instance is GameObject gameObject)
            {
                gameObject.SetActive(false);
                gameObject.transform.SetParent(container);
                gameObject.OnDestroyAsObservable().Subscribe(_ =>
                {
#if ADDRESSABLE_SUPPORT
                    if(isAddressable) Addressables.Release(instance);
#endif
                    pool.Remove(instance);
                });
            }
        }

        T Dequeue()
        {
            var instance = pool.LastOrDefault();
            pool.RemoveAt(pool.Count - 1);
            return instance;
        }
        
        bool TryDequeue(out T instance)
        {
            var any = pool.Any();
            if (any) instance = Dequeue();
            else instance = null;
            return any;
        }

        #endregion
    }
}