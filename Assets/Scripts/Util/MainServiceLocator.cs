using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainServiceLocator : ServiceLocator
{


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize() => ServiceLocatorProvider.Register(new MainServiceLocator());

    protected override void InitializeServices()
    {
      Debug.Log("MainServiceLocator InitializeServices");
      Add<IUIProviderService>(new UIProviderService());
      Add<IGameStateService>(new GameStateService());
    }

}
