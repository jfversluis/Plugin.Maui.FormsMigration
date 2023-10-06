namespace Plugin.Maui.FormsMigration.Sample;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	// SecureStorage Example
	async void SecureStorage_Clicked(object sender, EventArgs e)
	{
		// Xamarin.Essentials SecureStorage stores its data in $(AppInfo.PackageName).xamarinessentials
		// MAUI SecureStorage stores its data in $(AppInfo.PackageName).microsoft.maui.essentials.preferences
		// MAUI also uses different encryption for SecureStorage than used in Xamarin.Essentials (I think)

		// $(AppInfo.PackageName) is referring to the Microsoft.Maui.ApplicationModel.AppInfo APIs and
		// will be replaced with your actual application ID. These should be the same for both your old app and new app.

		// The code below assumes that there is a secure value saved with the key "oauth_token". Replace this key
		// with any value(s) you have stored in your legacy Xamarin app to get them out.

		string oauthToken = await LegacySecureStorage.GetAsync("oauth_token");
		bool result = LegacySecureStorage.Remove("oauth_token");
		await SecureStorage.SetAsync("oauth_token", oauthToken);
	}

	// VersionTracking Example
	async void VersionTracking_Clicked(object sender, EventArgs e)
	{

	}

	// Properties Example
	async void AppProperties_Clicked(object sender, EventArgs e)
	{
		// Retrieve values from your legacy Xamarin application through the LegacyApplication class.

		// The code below assumes that there is a property value saved with the key "id". Replace this key
		// with any value(s) you have stored in your legacy Xamarin app to get them out.

		// For more information, see: https://learn.microsoft.com/dotnet/maui/migration/app-properties

		int id;
		if (LegacyApplication.Current.Properties.ContainsKey("id"))
		{
			id = (int)LegacyApplication.Current.Properties["id"];
			Preferences.Set("id", id);
		}
	}
}
