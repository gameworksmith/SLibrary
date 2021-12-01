using System.Collections.Generic;

namespace SLibrary.EntitySystem
{
    public interface IWorld
    {
        List<IEntity> Entities { get; }
        List<ISystem> Systems { get; }
        List<IComponentData> Components { get; }
        
        void AddEntity(IEntity entity);
        void RemoveEntity(IEntity entity);

        void AddSystem(ISystem system);
        void RemoveSystem(ISystem system);

        void AddComponent(IComponentData componentData);
        void RemoveComponent(IComponentData componentData);

        int NextEntityId { get; }
        int NextComponentId { get; }

        /// <summary>
        /// 用于遍历数组进行System的更新
        /// </summary>
        void Update();

    }
}