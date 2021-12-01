using System.Collections.Generic;

namespace SLibrary.EntitySystem
{
    public interface IEntity
    {
        List<int> ComponentIds { get; }

        void AddComponent(int componentId);
        void RemoveComponent(int componentId);
    }
}