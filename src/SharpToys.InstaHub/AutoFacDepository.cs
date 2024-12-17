using Autofac;
using Autofac.Core;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpToys.InstaHub;

public class AutoFacDepository : IInstaDepository
{
    private IContainer? _container;

    public Task<TInstance?> GetAsync<TInstance>(string key) where TInstance : class
    {
        Guard.IsNotNull(_container);
        if (_container.TryResolveNamed(key, out TInstance? instance))
        {
            return Task.FromResult<TInstance?>(instance);
        }
        return Task.FromResult<TInstance?>(null);
    }

    public Task<TInstance> GetRequiredAsync<TInstance>(string key) where TInstance : class
    {
        Guard.IsNotNull(_container);
        var instance = _container.ResolveNamed<TInstance>(key);
        return Task.FromResult(instance);
    }


    public void Build(Action<ContainerBuilder> configure)
    {
        var builder = new ContainerBuilder();
        configure(builder);
        _container = builder.Build();
    }


    public static AutoFacDepository ScanAndLoad(Action<ContainerBuilder>? configureCommonDependency = null)
    {
        AutoFacDepository depository = new();

        var instaMark_Type = InstaMarkAttribute.FindAllInstaMarks();

        void Configure(ContainerBuilder builder)
        {
            configureCommonDependency?.Invoke(builder);

            foreach (var pair in instaMark_Type)
            {
                InstaMarkAttribute instaMarkAttribute = pair.Key;
                builder.RegisterType(pair.Value).Named(instaMarkAttribute.Key, instaMarkAttribute.TargetParentType);
            }
        }

        depository.Build(Configure);
        return depository;
    }
}
