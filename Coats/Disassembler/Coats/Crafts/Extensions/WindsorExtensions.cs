namespace Coats.Crafts.Extensions
{
    using Castle.MicroKernel;
    using Castle.MicroKernel.ComponentActivator;
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public static class WindsorExtensions
    {
        public static void InjectProperties(this IKernel kernel, object target)
        {
            Type type = target.GetType();
            foreach (PropertyInfo info in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (info.CanWrite && kernel.HasComponent(info.PropertyType))
                {
                    object obj2 = kernel.Resolve(info.PropertyType);
                    try
                    {
                        info.SetValue(target, obj2, null);
                    }
                    catch (Exception exception)
                    {
                        throw new ComponentActivatorException(string.Format("Error setting property {0} on type {1}, See inner exception for more information.", info.Name, type.FullName), exception, null);
                    }
                }
            }
        }
    }
}

