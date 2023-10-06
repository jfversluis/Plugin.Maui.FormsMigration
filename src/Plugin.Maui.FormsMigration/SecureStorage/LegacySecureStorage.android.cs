using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;

namespace Plugin.Maui.FormsMigration.SecureStorage;

public static class LegacySecureStorage
{
    internal static readonly string alias = $"{AppInfo.PackageName}.xamarinessentials";

    public static Task<string> GetAsync(string key)
    {
        ArgumentException.ThrowIfNullOrEmpty(nameof(key), nameof(key));

        string result = string.Empty;

        object locker = new();
        string? encVal = Preferences.Get(key, null, alias);

        if (!string.IsNullOrEmpty(encVal))
        {
            byte[] encData = Convert.FromBase64String(encVal);
            lock (locker)
            {
                AndroidKeyStore keyStore = new AndroidKeyStore(Platform.AppContext, alias, false);
                result = keyStore.Decrypt(encData);
            }
        }

        return Task.FromResult(result);
    }

    public static bool Remove(string key)
    {
        Preferences.Clear(alias);

        return true;
    }

    public static void RemoveAll()
    {
        Preferences.Clear(alias);
    }
}