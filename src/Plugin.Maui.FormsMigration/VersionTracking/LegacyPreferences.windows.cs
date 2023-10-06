using Windows.Storage;

namespace Plugin.Maui.FormsMigration.VersionTracking;

static partial class LegacyPreferences
{
	static readonly object locker = new();

	static bool PlatformContainsKey(string key, string sharedName)
	{
		lock (locker)
		{
			var appDataContainer = GetApplicationDataContainer(sharedName);
			return appDataContainer.Values.ContainsKey(key);
		}
	}

	static void PlatformRemove(string key, string sharedName)
	{
		lock (locker)
		{
			var appDataContainer = GetApplicationDataContainer(sharedName);
			if (appDataContainer.Values.ContainsKey(key))
			{
				appDataContainer.Values.Remove(key);
			}
		}
	}

	static T PlatformGet<T>(string key, T defaultValue, string sharedName)
	{
		lock (locker)
		{
			var appDataContainer = GetApplicationDataContainer(sharedName);
			if (appDataContainer.Values.ContainsKey(key))
			{
				var tempValue = appDataContainer.Values[key];
				if (tempValue != null)
				{
					return (T)tempValue;
				}
			}
		}

		return defaultValue;
	}

	static ApplicationDataContainer GetApplicationDataContainer(string sharedName)
	{
		var localSettings = ApplicationData.Current.LocalSettings;
		if (string.IsNullOrWhiteSpace(sharedName))
		{
			return localSettings;
		}

		if (!localSettings.Containers.ContainsKey(sharedName))
		{
			localSettings.CreateContainer(sharedName, ApplicationDataCreateDisposition.Always);
		}

		return localSettings.Containers[sharedName];
	}
}
