
using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocatorProvider
{
    //private static readonly Dictionary<string, IServiceLocator> locators = new Dictionary<string, IServiceLocator>();
    private static IServiceLocator locator;

    //public static void Register(string id, IServiceLocator serviceLocator) => locators[id] = serviceLocator;
    public static void Register(IServiceLocator serviceLocator) => locator = serviceLocator;

    public static IServiceLocator GetServiceLocator()
    {
        Debug.Log("GetServiceLocator, found locator: "+locator);
        return locator;
    }
}
