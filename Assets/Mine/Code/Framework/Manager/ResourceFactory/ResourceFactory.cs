#if ADDRESSABLE_SUPPORT
using UnityEngine.AddressableAssets;
#endif
#if VCONTAINER_SUPPORT
using VContainer;
using VContainer.Unity;
#endif
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Mine.Code.Framework.Manager.ResourceFactory
{
    public class ResourceFactory<T> where T : Object
    {
        #region Inner Classes

        public class ResourceFactoryBuilder
        {
            bool isInject;
            bool isAddressable;

#if VCONTAINER_SUPPORT
            public ResourceFactoryBuilder SetInject()
            {
                isInject = true;

                return this;
            }
#endif

#if ADDRESSABLE_SUPPORT
            public ResourceFactoryBuilder SetAddressable()
            {
                isAddressable = true;

                return this;
            }
#endif

            public ResourceFactory<T> Build<T>(string path) where T : Object
            {
                var resourceFactory = new ResourceFactory<T>(
                    path: path,
                    isAddressable: isAddressable,
                    isInject: isInject
                );

                return resourceFactory;
            }

            public ResourceFactory<T> BuildPool<T>(string path, int poolSize) where T : Component
            {
                var resourceFactory = new ResourcePoolFactory<T>(
                    path: path,
                    poolSize: poolSize,
                    isAddressable: isAddressable,
                    isInject: isInject
                );

                return resourceFactory;
            }
        }

        #endregion

        #region Fields

#if VCONTAINER_SUPPORT
        [Inject] LifetimeScope context;
#endif 
        readonly bool isAddressable;
        readonly bool isInject;
        
        protected readonly string path;

        #endregion

        #region Constructor

        protected ResourceFactory(string path, bool isAddressable, bool isInject)
        {
            this.path = path;
            this.isAddressable = isAddressable;
            this.isInject = isInject;
        }

        #endregion

        #region Static Methods

        public static ResourceFactoryBuilder Builder()
        {
            return new ResourceFactoryBuilder();
        }

        #endregion

        #region Virtual Methods

        public virtual async UniTask<T> LoadAsync(Transform parent = null)
        {
            T resource = null;
            
            if (isAddressable)
            {
#if ADDRESSABLE_SUPPORT
                resource = await Addressables.LoadAssetAsync<T>(path);
#endif
            }
            else resource = await Resources.LoadAsync<T>(path) as T;

            if(!resource) return null;
            
            if (typeof(T).IsAssignableFrom(typeof(Component)) || typeof(T).IsAssignableFrom(typeof(GameObject)))
            {
#if VCONTAINER_SUPPORT
                if(isInject) return context.Container.Instantiate(resource);
#endif
                return Object.Instantiate(resource, parent);
            }

            return resource;
        }

        public virtual void Release(T instance)
        {
#if ADDRESSABLE_SUPPORT
            if(isAddressable) Addressables.Release(instance);
#endif
            Object.Destroy(instance);
        }

        #endregion
    }
}