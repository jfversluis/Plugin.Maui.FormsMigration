using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Microsoft.Maui.ApplicationModel;
using Windows.Security.Cryptography.DataProtection;
using Windows.Storage;

namespace Plugin.Maui.FormsMigration.SecureStorage;

/// <summary>
/// The LegacySecureStorage class allows access to the secure values saved with Xamarin.Essentials.
/// </summary>
/// <remarks>
/// This functionality should only be used in migration scenarios from Xamarin to .NET MAUI.
/// Existing values from the legacy application can be read through this functionality and should be (re)saved with <see cref="Microsoft.Maui.Storage.SecureStorage"/>.
/// </remarks>
public partial class LegacySecureStorage
{
	internal static readonly string alias = $"{AppInfo.PackageName}.xamarinessentials";

	/// <summary>
	/// Gets and decrypts the value for a given key from the Xamarin.Essentials (legacy) SecureStorage store.
	/// </summary>
	/// <param name="key">The key to retrieve the value for.</param>
	/// <returns>The decrypted string value or <see cref="string.Empty"/> if a value was not found.</returns>
	public static async Task<string> GetAsync(string key)
	{
		var settings = GetSettings(alias);

		var encBytes = settings.Values[key] as byte[];

		if (encBytes == null)
		{
			return string.Empty;
		}

		var provider = new DataProtectionProvider();

		var buffer = await provider.UnprotectAsync(encBytes.AsBuffer());

		return Encoding.UTF8.GetString(buffer.ToArray());
	}

	/// <summary>
	/// Removes a key and its associated value if it exists from the Xamarin.Essentials (legacy) SecureStorage store.
	/// </summary>
	/// <param name="key">The key to remove.</param>
	public static bool Remove(string key)
	{
		var settings = GetSettings(alias);

		if (settings.Values.ContainsKey(key))
		{
			settings.Values.Remove(key);
			return true;
		}

		return false;
	}

	/// <summary>
	/// Removes all of the stored encrypted key/value pairs from the Xamarin.Essentials (legacy) SecureStorage store.
	/// </summary>
	public static void RemoveAll()
	{
		var settings = GetSettings(alias);

		settings.Values.Clear();
	}

	static ApplicationDataContainer GetSettings(string name)
	{
		var localSettings = ApplicationData.Current.LocalSettings;

		if (!localSettings.Containers.ContainsKey(name))
		{
			localSettings.CreateContainer(name, ApplicationDataCreateDisposition.Always);
		}

		return localSettings.Containers[name];
	}
}
