using Microsoft.Maui.ApplicationModel;

namespace Plugin.Maui.FormsMigration;

/// <summary>
/// The LegacySecureStorage class allows access to the secure values saved with Xamarin.Essentials.
/// </summary>
/// <remarks>
/// <para>IMPORTANT - Make sure you have an <c>Entitlements.plist</c> with the following:</para>
///    <para><c>&lt;key&gt;keychain-access-groups&lt;/key&gt;</c></para>
///    <para><c>&lt;string&gt;$(AppIdentifierPrefix)$(CFBundleIdentifier)&lt;/string&gt;</c></para>
/// <para>and that the <c>Entitlements.plist</c> is set in the Custom Entitlements field for Bundle Signing.</para>
/// <para>This functionality should only be used in migration scenarios from Xamarin to .NET MAUI.
/// Existing values from the legacy application can be read through this functionality and should be (re)saved with <see cref="Microsoft.Maui.Storage.SecureStorage"/>.</para>
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

		string result = KeyChain.ValueForKey(key, alias);

		return Task.FromResult(result);
    }

	/// <summary>
	/// Removes a key and its associated value if it exists from the Xamarin.Essentials (legacy) SecureStorage store.
	/// </summary>
	/// <param name="key">The key to remove.</param>
	public static bool Remove(string key)
    {
		bool result = KeyChain.Remove(key, alias);

		return result;
    }

	/// <summary>
	/// Removes all of the stored encrypted key/value pairs from the Xamarin.Essentials (legacy) SecureStorage store.
	/// </summary>
	public static void RemoveAll()
    {
		KeyChain.RemoveAll(alias);
    }
}