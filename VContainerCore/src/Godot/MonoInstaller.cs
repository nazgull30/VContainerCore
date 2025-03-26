using System;
using Godot;

namespace VContainer.Godot
{
    public partial class MonoInstaller : Node, IInstaller
    {
        public virtual void Install(IContainerBuilder builder)
        {
            throw new NotImplementedException();
        }
    }
}
