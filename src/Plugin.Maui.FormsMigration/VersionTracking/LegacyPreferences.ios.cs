using System.Globalization;

namespace Plugin.Maui.FormsMigration.VersionTracking;

public static partial class LegacyPreferences
{
	static readonly object locker = new();

	static bool PlatformContainsKey(string key, string sharedName)
	{
		lock (locker)
		{
			return GetUserDefaults(sharedName)[key] != null;
		}
	}

	static void PlatformRemove(string key, string sharedName)
	{
		lock (locker)
		{
			using var userDefaults = GetUserDefaults(sharedName);
			if (userDefaults[key] != null)
			{
				userDefaults.RemoveObject(key);
			}
		}
	}

	static T PlatformGet<T>(string key, T defaultValue, string sharedName)
	{
		object value = null;

		lock (locker)
		{
			using var userDefaults = GetUserDefaults(sharedName);
			if (userDefaults[key] == null)
			{
				return defaultValue;
			}

			switch (defaultValue)
			{
				case int i:
					value = (int)(nint)userDefaults.IntForKey(key);
					break;
				case bool b:
					value = userDefaults.BoolForKey(key);
					break;
				case long l:
					var savedLong = userDefaults.StringForKey(key);
					value = Convert.ToInt64(savedLong, CultureInfo.InvariantCulture);
					break;
				case double d:
					value = userDefaults.DoubleForKey(key);
					break;
				case float f:
					value = userDefaults.FloatForKey(key);
					break;
				case string s:
					// the case when the string is not null
					value = userDefaults.StringForKey(key);
					break;
				default:
					// the case when the string is null
					if (typeof(T) == typeof(string))
					{
						value = userDefaults.StringForKey(key);
					}
					break;
			}
		}

		return (T)value;
	}

	static NSUserDefaults GetUserDefaults(string sharedName)
	{
		if (!string.IsNullOrWhiteSpace(sharedName))
		{
			return new NSUserDefaults(sharedName, NSUserDefaultsType.SuiteName);
		}
			
		return NSUserDefaults.StandardUserDefaults;
	}
}
