using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Mine.Code.Framework.Manager.ResourceFactory
{
    public class ResourcePoolFactory<T> : ResourceFactory<T> where T : Component
    {
        #region Constructor

        public ResourcePoolFactory(string path, bool isAddressable, bool isInject, int poolSize) : base(path, isAddressable, isInject) => AddPool(poolSize).Forget();

        #endregion

        #region Fields

        readonly Queue<T> pool = new();
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
                pool.Enqueue(await base.LoadAsync());
            }
        }

        #endregion

        #region Override Methods

        public override async UniTask<T> LoadAsync(Transform parent = null)
        {
            if (pool.TryDequeue(out var instance))
            {
                instance.transform.SetParent(parent);
                instance.gameObject.SetActive(true);
                return instance;
            }
            
            instance = await base.LoadAsync(parent);

            if (instance == null) return instance;
        
            instance.gameObject.SetActive(true);
            return instance;
        }
    
        public override void Release(T instance)
        {
            instance.gameObject.SetActive(false);
            instance.transform.SetParent(container);
            pool.Enqueue(instance);
        }

        #endregion
    }
}