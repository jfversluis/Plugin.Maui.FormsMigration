using Security;

namespace Plugin.Maui.FormsMigration.SecureStorage;

class KeyChain
{
	static SecRecord ExistingRecordForKey(string key, string service)
    {
        return new SecRecord(SecKind.GenericPassword)
        {
            Account = key,
            Service = service
        };
    }

    internal static string ValueForKey(string key, string service)
    {
		using var record = ExistingRecordForKey(key, service);
		using var match = SecKeyChain.QueryAsRecord(record, out var resultCode);

		if (resultCode == SecStatusCode.Success
            && !string.IsNullOrEmpty(match?.ValueData?.ToString()))
        {
			return NSString.FromData(match.ValueData, NSStringEncoding.UTF8)
                ?? string.Empty;
        }
		else
        {
			return string.Empty;
        }
	}

    internal static bool Remove(string key, string service)
    {
		using var record = ExistingRecordForKey(key, service);
		using var match = SecKeyChain.QueryAsRecord(record, out var resultCode);

		if (resultCode == SecStatusCode.Success)
		{
			RemoveRecord(record);
			return true;
		}
        
		return false;
    }

    internal static void RemoveAll(string service)
    {
		using var query = new SecRecord(SecKind.GenericPassword) { Service = service };
		SecKeyChain.Remove(query);
	}

	static bool RemoveRecord(SecRecord record)
    {
        var result = SecKeyChain.Remove(record);
        if (result != SecStatusCode.Success && result != SecStatusCode.ItemNotFound)
        {
            throw new Exception($"Error removing record: {result}");
        }

        return true;
    }
}