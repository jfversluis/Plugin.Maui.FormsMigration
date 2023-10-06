using Microsoft.Maui.ApplicationModel;

namespace Plugin.Maui.FormsMigration.VersionTracking;

static partial class LegacyPreferences
{
	internal static string GetPrivatePreferencesSharedName(string feature) => $"{AppInfo.PackageName}.xamarinessentials.{feature}";

	internal static bool ContainsKey(string key, string sharedName) => PlatformContainsKey(key, sharedName);
	internal static void Remove(string key, string sharedName) => PlatformRemove(key, sharedName);
	internal static string Get(string key, string defaultValue, string sharedName) => PlatformGet<string>(key, defaultValue, sharedName);
}
