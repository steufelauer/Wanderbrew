
public interface IUIProviderService
{
    void AddProvider<T>(object provider) where T : IUIProvider;
    T GetProvider<T>();
}

public class UIProviderService : ServiceLocator, IUIProviderService
{
    public void AddProvider<T>(object provider) where T : IUIProvider
    {
        Add<T>(provider);
    }

    public T GetProvider<T>() => GetService<T>();
}
