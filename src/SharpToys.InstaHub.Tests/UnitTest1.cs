using Autofac;
using Microsoft.Extensions.DependencyInjection;
using SharpToys.InstaHub.Samples;

namespace SharpToys.InstaHub.Tests;

public class UnitTest1
{
    [Fact]
    public async void Test1()
    {
        NetDepository depository = NetDepository.ScanAndLoad(services =>
        {
            services.AddTransient<ICommonDependency,CommonDependency>();
        });

        var service = await depository.GetAsync<IPlugin>("afdsaf");

        var service1 = await depository.GetAsync<IPlugin>("KeyPlugin1");
        var service2 = await depository.GetAsync<IPlugin2>("KeyPlugin2");
        var service3 = await depository.GetAsync<IPlugin2>("KeyPlugin3");
    }

    [Fact]
    public async void Test2()
    {
        var depository = AutoFacDepository.ScanAndLoad(builder =>
        {
            builder.RegisterType<CommonDependency>().As<ICommonDependency>();
        });

        var service = await depository.GetAsync<IPlugin>("afdsaf");

        var service1 = await depository.GetAsync<IPlugin>("KeyPlugin1");
        var service2 = await depository.GetAsync<IPlugin2>("KeyPlugin2");
        var service3 = await depository.GetAsync<IPlugin2>("KeyPlugin3");
    }
}