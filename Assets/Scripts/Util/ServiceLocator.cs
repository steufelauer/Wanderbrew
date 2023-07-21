using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : IServiceLocator
{
    protected IDictionary<object, object> services;

    public ServiceLocator()
    {
        services = new Dictionary<object, object>();
        InitializeServices();
    }

    protected virtual void InitializeServices()
    {
      Debug.Log("ServiceLocator InitializeServices");
    }

    public T GetService<T>()
    {
        try
        {
            return (T)services[typeof(T)];
        }
        catch (KeyNotFoundException)
        {
            Debug.LogError("The requested service is not registered: " + typeof(T).ToString());
            return default;
        }
    }

    public void Add<T>(object service)
    {
        services.Add(typeof(T), service);
    }
}
