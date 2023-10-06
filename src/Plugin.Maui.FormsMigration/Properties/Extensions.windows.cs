using System.Runtime.CompilerServices;
using Windows.Foundation;

namespace Plugin.Maui.FormsMigration.Properties;

static class Extensions
{
	public static ConfiguredTaskAwaitable<T> DontSync<T>(this IAsyncOperation<T> self)
	{
		return self.AsTask().ConfigureAwait(false);
	}
}
