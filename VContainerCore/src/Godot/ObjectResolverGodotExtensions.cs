using Godot;

namespace VContainer.Godot
{
    public static class ObjectResolverGodotExtensions
    {
        public static void InjectGameObject(this IObjectResolver resolver, Node gameObject)
        {
            void InjectGameObjectRecursive(Node current)
            {
                if (!current.GetScript().AsBool())
                    return;

                resolver.Inject(current);
                var children = current.GetChildren();
                foreach (var child in children)
                {
                    if (!child.GetScript().AsBool())
                        continue;
                    resolver.Inject(child);
                    InjectGameObjectRecursive(child);
                }
            }

            InjectGameObjectRecursive(gameObject);
        }

        public static T Instantiate<T>(this IObjectResolver resolver, PackedScene prefab) where T : Node
        {
            var instance = prefab.Instantiate<T>();
            InjectGodotEngineObject(resolver, instance);
            return instance;
        }

        public static T Instantiate<T>(this IObjectResolver resolver, PackedScene prefab, Node2D parent)
            where T : Node2D
        {
            var instance = prefab.Instantiate<T>();
            parent.AddChild(instance);
            InjectGodotEngineObject(resolver, instance);
            return instance;
        }

        public static T Instantiate<T>(
            this IObjectResolver resolver,
            PackedScene prefab,
            Vector2 position,
            float rotation)
            where T : Node2D
        {
            var instance = prefab.Instantiate<T>();
            instance.GlobalPosition = position;
            instance.GlobalRotation = rotation;
            InjectGodotEngineObject(resolver, instance);
            return instance;
        }

        public static T Instantiate<T>(
            this IObjectResolver resolver,
            PackedScene prefab,
            Vector2 position,
            float rotation,
            Node2D parent)
            where T : Node2D
        {
            var instance = prefab.Instantiate<T>();
            instance.GlobalPosition = position;
            instance.GlobalRotation = rotation;
            parent.AddChild(instance);
            InjectGodotEngineObject(resolver, instance);
            return instance;
        }

        static void InjectGodotEngineObject<T>(IObjectResolver resolver, T instance) where T : Node
        {
            if (instance is Node gameObject)
                resolver.InjectGameObject(gameObject);
            else
                resolver.Inject(instance);
        }
    }
}
