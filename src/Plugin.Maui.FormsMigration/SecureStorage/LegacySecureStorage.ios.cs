using Microsoft.Maui.ApplicationModel;

namespace Plugin.Maui.FormsMigration.SecureStorage;

/// <summary>
/// IMPORTANT - Make sure you have an <c>Entitlements.plist</c> with the following:
//    <c><key>keychain-access-groups</key></c>
//    <c><string>$(AppIdentifierPrefix)$(CFBundleIdentifier)</string></c>
// and that the <c>Entitlements.plist</c> is set in the Custom Entitlements field for Bundle Signing
/// </summary>
public static class LegacySecureStorage
{
    internal static readonly string alias = $"{AppInfo.PackageName}.xamarinessentials";

    public static Task<string> GetAsync(string key)
    {
        ArgumentException.ThrowIfNullOrEmpty(nameof(key), nameof(key));

		string result = KeyChain.ValueForKey(key, alias);

		return Task.FromResult(result);
    }

    public static bool Remove(string key)
    {
		bool result = KeyChain.Remove(key, alias);

		return result;
    }

    public static void RemoveAll()
    {
		KeyChain.RemoveAll(alias);
    }
}