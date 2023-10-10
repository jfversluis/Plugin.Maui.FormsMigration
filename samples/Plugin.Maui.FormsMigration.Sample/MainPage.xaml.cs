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
		// More information: https://learn.microsoft.com/dotnet/maui/migration/secure-storage
		
		// Xamarin.Essentials SecureStorage stores its data in $(AppInfo.PackageName).xamarinessentials
		// MAUI SecureStorage stores its data in $(AppInfo.PackageName).microsoft.maui.essentials.preferences
		// MAUI also uses different encryption for SecureStorage than used in Xamarin.Essentials.

		// $(AppInfo.PackageName) is referring to the Microsoft.Maui.ApplicationModel.AppInfo APIs and
		// will be replaced with your actual application ID. These should be the same for both your old app and new app.

		// The code below assumes that there is a secure value saved with the key "oauth_token". Replace this key
		// with any value(s) you have stored in your legacy Xamarin app to get them out.

		string oauthToken = await LegacySecureStorage.GetAsync("oauth_token");
		bool result = LegacySecureStorage.Remove("oauth_token");
		await SecureStorage.SetAsync("oauth_token", oauthToken);
	}

	// VersionTracking Example
	void VersionTracking_Clicked(object sender, EventArgs e)
	{
		// More information: https://learn.microsoft.com/dotnet/maui/migration/version-tracking

		// Retrieve values from your legacy Xamarin application through the LegacyVersionTracking class.

		// Read legacy version/build number information
		labelIsFirst.Text = LegacyVersionTracking.IsFirstLaunchEver.ToString();
		labelCurrentVersionIsFirst.Text = LegacyVersionTracking.IsFirstLaunchForCurrentVersion.ToString();
		labelCurrentBuildIsFirst.Text = LegacyVersionTracking.IsFirstLaunchForCurrentBuild.ToString();
		labelCurrentVersion.Text = LegacyVersionTracking.CurrentVersion.ToString();
		labelCurrentBuild.Text = LegacyVersionTracking.CurrentBuild.ToString();
		labelFirstInstalledVer.Text = LegacyVersionTracking.FirstInstalledVersion.ToString();
		labelFirstInstalledBuild.Text = LegacyVersionTracking.FirstInstalledBuild.ToString();
		labelVersionHistory.Text = string.Join(',', LegacyVersionTracking.VersionHistory);
		labelBuildHistory.Text = string.Join(',', LegacyVersionTracking.BuildHistory);

		// These two properties may be null if this is the first version
		labelPreviousVersion.Text = LegacyVersionTracking.PreviousVersion?.ToString() ?? "none";
		labelPreviousBuild.Text = LegacyVersionTracking.PreviousBuild?.ToString() ?? "none";

		// Transfer the full legacy Xamarin app version tracking information (version & build numbers)
		// to the .NET MAUI Version Tracking data store. Before doing so the *new* .NET MAUI Version Tracking
		// store will be cleared.
		LegacyVersionTracking.TransferHistory(true, true, true);
	}

	// Properties Example
	void AppProperties_Clicked(object sender, EventArgs e)
	{
		// More information: https://learn.microsoft.com/dotnet/maui/migration/app-properties

		// Retrieve values from your legacy Xamarin application through the LegacyApplication class.

		// The code below assumes that there is a property value saved with the key "id". Replace this key
		// with any value(s) you have stored in your legacy Xamarin app to get them out.

		int id;
		if (LegacyApplication.Current.Properties.ContainsKey("id"))
		{
			id = (int)LegacyApplication.Current.Properties["id"];
			Preferences.Set("id", id);
		}
	}
}
