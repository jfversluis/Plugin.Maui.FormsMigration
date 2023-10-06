using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;

namespace Plugin.Maui.FormsMigration;

/// <summary>
/// The LegacySecureStorage class allows access to the secure values saved with Xamarin.Essentials.
/// </summary>
/// <remarks>
/// This functionality should only be used in migration scenarios from Xamarin to .NET MAUI.
/// Existing values from the legacy application can be read through this functionality and should be (re)saved with <see cref="Microsoft.Maui.Storage.SecureStorage"/>.
/// </remarks>
public static class LegacySecureStorage
{
    internal static readonly string alias = $"{AppInfo.PackageName}.xamarinessentials";

	/// <summary>
	/// Gets and decrypts the value for a given key from the Xamarin.Essentials (legacy) SecureStorage store.
	/// </summary>
	/// <param name="key">The key to retrieve the value for.</param>
	/// <returns>The decrypted string value or <see cref="string.Empty"/> if a value was not found.</returns>
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
                AndroidKeyStore keyStore = new(Platform.AppContext, alias, false);
                result = keyStore.Decrypt(encData);
            }
        }

        return Task.FromResult(result);
    }

	/// <summary>
	/// Removes a key and its associated value if it exists from the Xamarin.Essentials (legacy) SecureStorage store.
	/// </summary>
	/// <param name="key">The key to remove.</param>
	public static bool Remove(string key)
    {
        Preferences.Clear(alias);

        return true;
    }

	/// <summary>
	/// Removes all of the stored encrypted key/value pairs from the Xamarin.Essentials (legacy) SecureStorage store.
	/// </summary>
	public static void RemoveAll()
    {
        Preferences.Clear(alias);
    }
}