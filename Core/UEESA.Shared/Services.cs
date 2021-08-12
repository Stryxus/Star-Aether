public static class Services
{
    public static IServiceProvider Provider { get; private set; }

    public static void SetServiceProvider(IServiceProvider provider)
    {
        if (Provider != null) return;
        Provider = provider;
    }

    public static void Get<T>(out T service) => service = (T)Provider.GetService(typeof(T));
    public static T Get<T>() => (T)Provider.GetService(typeof(T));
}