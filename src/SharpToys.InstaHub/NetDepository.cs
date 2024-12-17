using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace SharpToys.InstaHub;

public class NetDepository : IInstaDepository
{
    private IServiceProvider? _serviceProvider;


    public NetDepository()
    {

    }


    public Task<TInstance?> GetAsync<TInstance>(string key) where TInstance: class
    {
        Guard.IsNotNull(_serviceProvider);
        var instance = _serviceProvider.GetKeyedService<TInstance>(key);
        return Task.FromResult(instance);
    }

    public async Task<TInstance> GetRequiredAsync<TInstance>(string key) where TInstance : class
    {
        TInstance? instance = await GetAsync<TInstance>(key);
        Guard.IsNotNull(instance);
        return instance;
    }


    public void Build(Action<IServiceCollection> configure)
    {
        ServiceCollection serviceDescriptors = new();
        configure(serviceDescriptors);
        _serviceProvider = serviceDescriptors.BuildServiceProvider();
    }



    public static NetDepository ScanAndLoad(Action<IServiceCollection>? configureCommonDependency = null)
    {
        NetDepository depository = new();

        var instaMark_Type = InstaMarkAttribute.FindAllInstaMarks();

        void configure(IServiceCollection services)
        {
            configureCommonDependency?.Invoke(services);
            foreach (var pair in instaMark_Type)
            {
                InstaMarkAttribute instaMarkAttribute = pair.Key;
                services.AddKeyedTransient(instaMarkAttribute.TargetParentType, instaMarkAttribute.Key, pair.Value);
            }
        }

        depository.Build(configure);

        return depository;
    }
}

