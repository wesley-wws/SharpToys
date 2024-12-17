using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpToys.InstaHub.Samples;

public interface IPlugin
{
}

public interface IPlugin2
{
}

[InstaMark<IPlugin>("KeyPlugin1")]
[InstaMark<IPlugin2>("KeyPlugin2")]
public class Plugin : IPlugin, IPlugin2
{

}

[InstaMark<IPlugin2>("KeyPlugin3")]
public class Plugin2 : IPlugin2
{
    public Plugin2(ICommonDependency commonDependency)
    {
        
    }
}


public interface ICommonDependency
{
}

public class CommonDependency : ICommonDependency
{

}
