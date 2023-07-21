using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IServiceLocator
{
    T GetService<T>();
    void Add<T>(object service);
}
