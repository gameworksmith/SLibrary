using System.Collections.Generic;

namespace SLibrary.EntitySystem
{
    public interface ISystem
    {
        // 执行命令
        void ExecuteComponent(IEnumerable<IComponentData> componentData);
        // 需要关注多种数据
        System.Func<IEnumerable<IComponentData>> FilterFunc { get; }
    }
}