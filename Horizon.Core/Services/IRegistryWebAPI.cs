using System;

namespace Horizon.Core.Services
{
    /// <summary>
    /// 注册webapi实例
    /// </summary>
    public interface IRegistryWebAPI
    {
        Uri Uri { get; }
    }
}