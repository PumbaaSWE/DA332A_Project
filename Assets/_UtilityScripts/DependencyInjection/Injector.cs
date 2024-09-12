using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;



[AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property)]
sealed class InjectAttribute : Attribute
{

}

[AttributeUsage(AttributeTargets.Method)]
sealed class ProvideAttribute : Attribute
{
    public ProvideAttribute()
    {

    }
}
public interface IDependencyProvider { }

[DefaultExecutionOrder(-99999)]
public class Injector : Singleton<Injector>
{
    
    const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    readonly Dictionary<Type, object> providers = new Dictionary<Type, object>();

    protected override void Awake()
    {
        base.Awake();
        IEnumerable<IDependencyProvider> providers = FindMonoBehaviour().OfType<IDependencyProvider>();
        foreach (IDependencyProvider provider in providers)
        {
            RegisterProvider(provider);
        }

        IEnumerable<MonoBehaviour> injectables = FindMonoBehaviour().Where(IsInjectable);
        foreach (MonoBehaviour injectable in injectables)
        {
            Inject(injectable);
        }
    }

    private void Inject(object instance)
    {
        Type type = instance.GetType();
        IEnumerable<FieldInfo> injectableFields = type.GetFields(bindingFlags).Where(member => Attribute.IsDefined(member, typeof(InjectAttribute)));
        foreach (var injectableField in injectableFields)
        {
            Type fieldType = injectableField.FieldType;
            object resolvedInstance = Resolve(fieldType);
            if(resolvedInstance == null)
            {
                throw new Exception($"Failed to resolve field {fieldType.Name} for {type.Name}");
            }

            injectableField.SetValue(instance, resolvedInstance);
        }

        IEnumerable<MethodInfo> injectableMethods = type.GetMethods(bindingFlags).Where(member => Attribute.IsDefined(member, typeof(InjectAttribute)));
        foreach (var injectableMethod in injectableMethods)
        {
            IEnumerable<Type> requiredParams = injectableMethod.GetParameters().Select(param => param.ParameterType);
            object[] resolvedInstances = requiredParams.Select(Resolve).ToArray();
            if (resolvedInstances.Any(resolvedInstance => resolvedInstance == null))
            {
                throw new Exception($"Failed to inject {type.Name}.{injectableMethod.Name}");
            }
            injectableMethod.Invoke(instance, resolvedInstances);

        }

        IEnumerable<PropertyInfo> injectableProperties = type.GetProperties(bindingFlags).Where(member => Attribute.IsDefined(member, typeof(InjectAttribute)));
        foreach (var injectableProperty in injectableProperties)
        {
            Type propertyType = injectableProperty.PropertyType;
            object resolvedInstance = Resolve(propertyType);
            if (resolvedInstance == null)
            {
                throw new Exception($"Failed to resolve property {propertyType.Name} for {type.Name}");
            }

            MethodInfo setter = injectableProperty.GetSetMethod(true);

            if (setter != null)
            {
                // Just be aware that you're kind of being sneaky here.
                setter.Invoke(instance, new object[] { resolvedInstance });
            }
            else
            {
                throw new Exception($"{type.Name} property {propertyType.Name} for has no set method!");
            }
            //injectableProperty.SetValue(instance, resolvedInstance);
        }

    }

    object Resolve(Type type)
    {
        providers.TryGetValue(type, out object instance);
        return instance;
    }

    private static bool IsInjectable(MonoBehaviour script)
    {
        MemberInfo[] members = script.GetType().GetMembers(bindingFlags);
        return members.Any(member => Attribute.IsDefined(member, typeof(InjectAttribute)));
    }

    private void RegisterProvider(IDependencyProvider provider)
    {
        MethodInfo[] methods = provider.GetType().GetMethods(bindingFlags);
        foreach (MethodInfo method in methods)
        {
            if(!Attribute.IsDefined(method, typeof(ProvideAttribute)))
            {
                continue;
            }

            Type returnType = method.ReturnType;
            object providedInstance = method.Invoke(provider, null);
            if(providedInstance != null)
            {
                providers.Add(returnType, providedInstance);
                //Debug.Log($"Registered {returnType.Name} from {provider.GetType().Name}");
            }
            else
            {
                throw new Exception($"Provider {provider.GetType().Name} returned null for {returnType.Name}");
            }
        }
    }

    private static MonoBehaviour[] FindMonoBehaviour()
    {
        return FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.InstanceID);
    }
}
