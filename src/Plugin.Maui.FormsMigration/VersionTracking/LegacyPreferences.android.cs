using System.Globalization;
using Android.Content;
using Android.Preferences;

namespace Plugin.Maui.FormsMigration;

public static partial class LegacyPreferences
{
	static readonly object locker = new();

	static bool PlatformContainsKey(string key, string sharedName)
	{
		lock (locker)
		{
			using var sharedPreferences = GetSharedPreferences(sharedName);
			return sharedPreferences.Contains(key);
		}
	}

	static void PlatformRemove(string key, string sharedName)
	{
		lock (locker)
		{
			using var sharedPreferences = GetSharedPreferences(sharedName);
			using var editor = sharedPreferences.Edit();
			editor.Remove(key).Apply();
		}
	}

	static T PlatformGet<T>(string key, T defaultValue, string sharedName)
	{
		lock (locker)
		{
			object value = null;
			using (var sharedPreferences = GetSharedPreferences(sharedName))
			{
				if (defaultValue == null)
				{
					value = sharedPreferences.GetString(key, null);
				}
				else
				{
					switch (defaultValue)
					{
						case int i:
							value = sharedPreferences.GetInt(key, i);
							break;
						case bool b:
							value = sharedPreferences.GetBoolean(key, b);
							break;
						case long l:
							value = sharedPreferences.GetLong(key, l);
							break;
						case double d:
							var savedDouble = sharedPreferences.GetString(key, null);
							if (string.IsNullOrWhiteSpace(savedDouble))
							{
								value = defaultValue;
							}
							else
							{
								if (!double.TryParse(savedDouble, NumberStyles.Number | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out var outDouble))
								{
									var maxString = Convert.ToString(double.MaxValue, CultureInfo.InvariantCulture);
									outDouble = savedDouble.Equals(maxString) ? double.MaxValue : double.MinValue;
								}

								value = outDouble;
							}
							break;
						case float f:
							value = sharedPreferences.GetFloat(key, f);
							break;
						case string s:
							// the case when the string is not null
							value = sharedPreferences.GetString(key, s);
							break;
					}
				}
			}

			return (T)value;
		}
	}

	static ISharedPreferences GetSharedPreferences(string sharedName)
	{
		var context = Platform.AppContext;

		return string.IsNullOrWhiteSpace(sharedName) ?
#pragma warning disable CS0618 // Type or member is obsolete
			PreferenceManager.GetDefaultSharedPreferences(context) :
#pragma warning restore CS0618 // Type or member is obsolete
				context.GetSharedPreferences(sharedName, FileCreationMode.Private);
	}
}
