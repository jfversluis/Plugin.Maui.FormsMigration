namespace Plugin.Maui.FormsMigration;

/// <summary>
/// The LegacyApplication allows you to access the values saved through <c>Application.Current.Properties</c> in your legacy Xamarin app.
/// </summary>
/// <remarks>
/// <para>The <c>Properties</c> API is no longer available in .NET MAUI, use this API to retrieve values from your legacy Xamarin app and save them
/// through <see cref="Microsoft.Maui.Storage.Preferences"/>.</para>
/// <para>For more information, please refer to <a href="https://learn.microsoft.com/dotnet/maui/migration/app-properties">Microsoft Learn</a>.</para>
/// </remarks>
public class LegacyApplication
{
	readonly PropertiesDeserializer deserializer;
	Task<IDictionary<string, object>>? propertiesTask;

	static LegacyApplication? current;
	public static LegacyApplication? Current
	{
		get
		{
			current ??= (LegacyApplication?)Activator.CreateInstance(typeof(LegacyApplication));

			return current;
		}
	}

	public LegacyApplication()
	{
		deserializer = new PropertiesDeserializer();
	}

	public IDictionary<string, object> Properties
	{
		get
		{
			propertiesTask ??= GetPropertiesAsync();
			return propertiesTask.Result;
		}
	}

	async Task<IDictionary<string, object>> GetPropertiesAsync()
	{
		var properties = await deserializer.DeserializePropertiesAsync().ConfigureAwait(false);
		properties ??= new Dictionary<string, object>(4);

		return properties;
	}
}

