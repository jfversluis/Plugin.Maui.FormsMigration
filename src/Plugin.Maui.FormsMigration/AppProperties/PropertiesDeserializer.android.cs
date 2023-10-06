using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Xml;

namespace Plugin.Maui.FormsMigration;

class PropertiesDeserializer
{
    public Task<IDictionary<string, object>> DeserializePropertiesAsync()
    {
        // Deserialize property dictionary to local storage
        return Task.Run(() =>
        {
			using IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
			if (!store.FileExists(Constants.propertyStoreFile))
			{
				return new Dictionary<string, object>(4);
			}

			using IsolatedStorageFileStream stream = store.OpenFile(Constants.propertyStoreFile, FileMode.Open, FileAccess.Read);
			using XmlDictionaryReader reader = XmlDictionaryReader.CreateBinaryReader(stream, XmlDictionaryReaderQuotas.Max);
			
            if (stream.Length == 0)
            {
				return new Dictionary<string, object>(4);
            }

			try
			{
				var dcs = new DataContractSerializer(typeof(Dictionary<string, object>));
				var readObject = dcs.ReadObject(reader) as IDictionary<string, object>;
				return readObject ?? new Dictionary<string, object>(4);
			}
			catch (Exception e)
			{
				Debug.WriteLine("Could not deserialize properties: " + e.Message);
				Console.WriteLine($"PropertyStore Exception while reading Application properties: {e}");
			}
			
			return new Dictionary<string, object>(4);
        });
    }
}