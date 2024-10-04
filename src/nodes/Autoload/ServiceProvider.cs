using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace HalfNibbleGame.Autoload;

public interface IServiceProvider
{
    void ProvidePersistent<T>(T obj);
    void ProvideInScene<T>(T obj);
    T Get<T>();
    bool TryGet<T>([NotNullWhen(true)] out T? obj);
}

public sealed class ServiceProvider : IServiceProvider
{
    private readonly Dictionary<Type, object> services = new();
    private readonly List<Type> sceneScopedServices = new();

    public void ProvidePersistent<T>(T obj)
    {
        provide(obj);
    }

    public void ProvideInScene<T>(T obj)
    {
        provide(obj);
        sceneScopedServices.Add(typeof(T));
    }

    private void provide<T>(T obj)
    {
        if (obj is null)
        {
            throw new ArgumentNullException();
        }
        services.Add(typeof(T), obj);
    }

    public T Get<T>()
    {
        if (!TryGet<T>(out var obj))
        {
            throw new KeyNotFoundException();
        }

        return obj;
    }

    public bool TryGet<T>([NotNullWhen(true)] out T? obj)
    {
        if (!services.TryGetValue(typeof(T), out var service))
        {
            obj = default;
            return false;
        }

        obj = (T) service;
        return true;
    }

    public void OnSceneChanging()
    {
        foreach (var type in sceneScopedServices)
        {
            services.Remove(type);
        }
        sceneScopedServices.Clear();
    }
}
