using SharpToys.Easyflect;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpToys.InstaHub;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public abstract class InstaMarkAttribute : Attribute
{
    public string Key { get; }

    public virtual Type TargetParentType { get; }


    protected InstaMarkAttribute(Type type, string key)
    {
        Key = key;
        TargetParentType = type;
    }


    public static Dictionary<InstaMarkAttribute, Type> FindAllInstaMarks()
    {
        // Initialize a dictionary to hold types with InstaMarkAttribute
        var instaMark_Type = new Dictionary<InstaMarkAttribute, Type>();

        EasyReflect.TraverseLoadedTypes((a, t) =>
        {
            if (!t.IsClass || !t.IsPublic || t.IsAbstract)
            {
                return;
            }

            var instaMarks = t.GetCustomAttributes<InstaMarkAttribute>();

            foreach (var mark in instaMarks)
            {
                instaMark_Type[mark] = t;
            }
        }, a => !a.IsDynamic);

        return instaMark_Type;
    }
}

public sealed class InstaMarkAttribute<T> : InstaMarkAttribute
{
    public InstaMarkAttribute(string key) 
        : base(typeof(T), key)
    {
    }
}

