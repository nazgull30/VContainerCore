using System;
using System.Collections.Generic;
using VContainer.Godot;
using VContainer.Internal;

namespace VContainer.Factory
{
    public abstract class PlaceholderFactoryBase<TValue>
    {
        private IObjectResolver _resolver;

        [Inject]
        protected void Construct(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        protected object Create(IReadOnlyList<IInjectParameter> parameters)
        {
            try
            {
                var injector = InjectorCache.GetOrBuild(typeof(TValue));
                var instance = injector.CreateInstance(_resolver, parameters);
                return instance;
            }
            catch (Exception e)
            {
                throw new VContainerException(typeof(TValue),
                    $"Error during construction of via {GetType()}.Create method! \n {e}");
            }
        }
    }

    public class PlaceholderFactory<TValue> : PlaceholderFactoryBase<TValue>
    {
        public TValue Create()
        {
            return (TValue)Create(null);
        }
    }

    public class PlaceholderFactory<TParam1, TValue> : PlaceholderFactoryBase<TValue>
    {
        public TValue Create(TParam1 param1)
        {
            var parameters = new IInjectParameter[]
            {
                new TypedParameter(typeof(TParam1), param1)
            };
            return (TValue)Create(parameters);
        }
    }

    public class PlaceholderFactory<TParam1, TParam2, TValue> : PlaceholderFactoryBase<TValue>
    {
        public TValue Create(TParam1 param1, TParam2 param2)
        {
            var parameters = new IInjectParameter[]
            {
                new TypedParameter(typeof(TParam1), param1),
                new TypedParameter(typeof(TParam2), param2)
            };
            return (TValue)Create(parameters);
        }
    }

    public class PlaceholderFactory<TParam1, TParam2, TParam3, TValue> : PlaceholderFactoryBase<TValue>
    {
        public TValue Create(TParam1 param1, TParam2 param2, TParam3 param3)
        {
            var parameters = new IInjectParameter[]
            {
                new TypedParameter(typeof(TParam1), param1),
                new TypedParameter(typeof(TParam2), param2),
                new TypedParameter(typeof(TParam3), param3)
            };
            return (TValue)Create(parameters);
        }
    }

    public class PlaceholderFactory<TParam1, TParam2, TParam3, TParam4, TValue> : PlaceholderFactoryBase<TValue>
    {
        public TValue Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
        {
            var parameters = new IInjectParameter[]
            {
                new TypedParameter(typeof(TParam1), param1),
                new TypedParameter(typeof(TParam2), param2),
                new TypedParameter(typeof(TParam3), param3),
                new TypedParameter(typeof(TParam4), param4)
            };
            return (TValue)Create(parameters);
        }
    }
}
