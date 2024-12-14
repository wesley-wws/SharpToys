using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace SharpToys.InstaHub;

public class InstanceHub
{
    private ServiceCollection _services;

    public InstanceHub()
    {
        _services = new ServiceCollection();
    }

    //public T GetInstance<T>(string key)
    //{

    //}
}


public class NetDependencyInjectionSource : IInstanceSource
{
    private readonly IServiceCollection _services;

    private IServiceProvider _serviceProvider;


    public NetDependencyInjectionSource()
    {
        _services = new ServiceCollection();
    }


    public Task<TInstance> GetAsync<TInstance>(string key)
    {
        var instance = _serviceProvider.GetKeyedService<TInstance>(key);
        Guard.IsNotNull(instance);
        return Task.FromResult(instance);
    }

    public void Build()
    {
        _serviceProvider = _services.BuildServiceProvider();
    }
}

public interface IInstanceSource
{
    Task<TInstance> GetAsync<TInstance>(string key);

    //Task<IReadOnlyDictionary<string, TInstance>> GetAsync();
}
