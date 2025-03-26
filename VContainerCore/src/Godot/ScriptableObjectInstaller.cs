using System;
using Godot;

namespace VContainer.Godot
{
    public partial class ScriptableObjectInstaller : Resource, IInstaller
    {
        public virtual void Install(IContainerBuilder builder)
        {
            throw new NotImplementedException();
        }
    }
}
