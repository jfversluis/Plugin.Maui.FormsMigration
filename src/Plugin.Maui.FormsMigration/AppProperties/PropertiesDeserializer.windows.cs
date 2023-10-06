using System.Diagnostics;
using System.Runtime.Serialization;
using Windows.Storage;

namespace Plugin.Maui.FormsMigration.Properties;

public class PropertiesDeserializer
{
    const string PropertyStoreFile = "PropertyStore.forms";

    public async Task<IDictionary<string, object>> DeserializePropertiesAsync()
    {
        try
        {
            StorageFile file = await ApplicationData.Current.RoamingFolder.GetFileAsync(PropertyStoreFile).DontSync();
            using (Stream stream = (await file.OpenReadAsync().DontSync()).AsStreamForRead())
            {
                if (stream.Length == 0)
                {
                    return new Dictionary<string, object>(4);
                }

                try
                {
                    var serializer = new DataContractSerializer(typeof(IDictionary<string, object>));
                    return (IDictionary<string, object>)serializer.ReadObject(stream);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Could not deserialize properties: " + e.Message);
                    Console.WriteLine($"PropertyStore Exception while reading Application properties: {e}");
                }
                return null;
            }
        }
        catch (FileNotFoundException)
        {
            return new Dictionary<string, object>(4);
        }
    }
}