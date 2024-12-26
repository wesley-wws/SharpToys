using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SharpToys.Easyflect;

public static class EasyReflect
{
    public static void TraverseLoadedTypes(Action<Assembly, Type> action, Predicate<Assembly>? assemblyFilter = null)
    {
        // Get all currently loaded assemblies in the application domain
        IEnumerable<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies();
        if (assemblyFilter != null)
        {
            assemblies = assemblies.Where(t => assemblyFilter(t));
        }

        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();
            //try
            //{
            //    types = assembly.GetTypes();
            //}
            //catch (ReflectionTypeLoadException ex)
            //{
            //    types = ex.Types.Where(t => t != null).ToArray();
            //}

            foreach (var type in types)
            {
                action(assembly, type);
            }
        }
    }
}
