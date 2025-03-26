using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Godot;
using VContainer.Internal;

namespace VContainer
{
    public static class InjectorCache
    {
        static readonly ConcurrentDictionary<Type, IInjector> Injectors = new();

        public static IInjector GetOrBuild(Type type)
        {
            return Injectors.GetOrAdd(type, key =>
            {
                GD.Print($"WARNING!! {type}: ReflectionInjector created!!");
                return ReflectionInjector.Build(key);
            });
        }


        public static void SetInjectorsRepo(IInjectorRepo injectorRepo)
        {
            // Injectors.TryAdd(typeof(EntryPointDispatcher), new EntryPointDispatcherInjector());
            foreach (var (key, value) in injectorRepo.Injectors)
            {
                Injectors.TryAdd(key, value);
            }
        }
    }

    public interface IInjectorRepo
    {
        Dictionary<Type, IInjector> Injectors { get; }
    }

}
