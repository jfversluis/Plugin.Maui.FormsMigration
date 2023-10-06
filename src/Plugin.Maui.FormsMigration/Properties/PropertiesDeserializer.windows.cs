using System.Diagnostics;
using System.Runtime.Serialization;
using Windows.Storage;

namespace Plugin.Maui.FormsMigration.Properties;

public static class PropertiesDeserializer
{
    const string propertyStoreFile = "PropertyStore.forms";

    public static async Task<IDictionary<string, object>> DeserializePropertiesAsync()
    {
        try
        {
            StorageFile file = await ApplicationData.Current.RoamingFolder.GetFileAsync(propertyStoreFile).DontSync();
			using Stream stream = (await file.OpenReadAsync().DontSync()).AsStreamForRead();

			if (stream.Length == 0)
			{
				return new Dictionary<string, object>(4);
			}

			try
			{
				var serializer = new DataContractSerializer(typeof(IDictionary<string, object>));
				var readObject = serializer.ReadObject(stream) as IDictionary<string, object>;

				return readObject ?? new Dictionary<string, object>(4);
			}
			catch (Exception e)
			{
				Debug.WriteLine("Could not deserialize properties: " + e.Message);
				Console.WriteLine($"PropertyStore Exception while reading Application properties: {e}");
			}
			return new Dictionary<string, object>(4);
		}
        catch (FileNotFoundException)
        {
            return new Dictionary<string, object>(4);
        }
    }
}