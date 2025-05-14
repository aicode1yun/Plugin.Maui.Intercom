namespace Plugin.Maui.Intercom;

public static class Intercom
{
    private static IIntercom? s_defaultImplementation;

    /// <summary>
    ///     Provides the default implementation for static usage of this API.
    /// </summary>
    public static IIntercom Default
    {
        get => s_defaultImplementation ??= new IntercomImplementation();
    }

    internal static void SetDefault(IIntercom? implementation)
    {
        s_defaultImplementation = implementation;
    }
}
