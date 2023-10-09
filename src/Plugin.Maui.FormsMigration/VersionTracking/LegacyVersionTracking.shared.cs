using Microsoft.Maui.ApplicationModel;

namespace Plugin.Maui.FormsMigration;

public static class LegacyVersionTracking
{
	const string versionsKey = "VersionTracking.Versions";
	const string buildsKey = "VersionTracking.Builds";

	static readonly string sharedName = LegacyPreferences.GetPrivatePreferencesSharedName("versiontracking");

	static Dictionary<string, List<string>> versionTrail = new();

	static string VersionsKey => versionsKey;
	static string BuildsKey => buildsKey;
	static string SharedName => sharedName;

	static LegacyVersionTracking()
	{
		InitVersionTracking();
	}

	/// <summary>
	/// Initialize VersionTracking module, load data and track current version.
	/// </summary>
	/// <remarks>
	/// For internal use. Usually only called once in production code.
	/// </remarks>
	internal static void InitVersionTracking()
	{
		IsFirstLaunchEver = !LegacyPreferences.ContainsKey(versionsKey, sharedName)
			|| !LegacyPreferences.ContainsKey(buildsKey, sharedName);
		if (IsFirstLaunchEver)
		{
			versionTrail = new Dictionary<string, List<string>>
				{
					{ versionsKey, new List<string>() },
					{ buildsKey, new List<string>() }
				};
		}
		else
		{
			versionTrail = new Dictionary<string, List<string>>
				{
					{ versionsKey, ReadHistory(versionsKey).ToList() },
					{ buildsKey, ReadHistory(buildsKey).ToList() }
				};
		}

		IsFirstLaunchForCurrentVersion = !versionTrail[versionsKey].Contains(CurrentVersion)
			|| CurrentVersion != LastInstalledVersion;

		if (IsFirstLaunchForCurrentVersion)
		{
			// Avoid duplicates and move current version to end of list if already present
			versionTrail[versionsKey].RemoveAll(v => v == CurrentVersion);
			versionTrail[versionsKey].Add(CurrentVersion);
		}

		IsFirstLaunchForCurrentBuild = !versionTrail[buildsKey].Contains(CurrentBuild)
			|| CurrentBuild != LastInstalledBuild;

		if (IsFirstLaunchForCurrentBuild)
		{
			// Avoid duplicates and move current build to end of list if already present
			versionTrail[buildsKey].RemoveAll(b => b == CurrentBuild);
			versionTrail[buildsKey].Add(CurrentBuild);
		}
	}

	/// <summary>
	/// Gets a value indicating whether this is the first time this app has ever been launched on this device.
	/// </summary>
	public static bool IsFirstLaunchEver { get; private set; }

	/// <summary>
	/// Gets a value indicating if this is the first launch of the legacy Xamarin app for the current version number.
	/// </summary>
	public static bool IsFirstLaunchForCurrentVersion { get; private set; }

	/// <summary>
	/// Gets a value indicating if this is the first launch of the legacy Xamarin app for the current version number.
	/// </summary>
	public static bool IsFirstLaunchForCurrentBuild { get; private set; }

	/// <summary>
	/// Gets the current version number of the legacy Xamarin app.
	/// </summary>
	public static string CurrentVersion => AppInfo.VersionString;

	/// <summary>
	/// Gets the current build of the legacy Xamarin app.
	/// </summary>
	public static string CurrentBuild => AppInfo.BuildString;

	/// <summary>
	/// Gets the version number for the previously run version of the legacy Xamarin app.
	/// </summary>
	public static string PreviousVersion => GetPrevious(versionsKey);

	/// <summary>
	/// Gets the build number for the previously run version of the legacy Xamarin app.
	/// </summary>
	public static string PreviousBuild => GetPrevious(buildsKey);

	/// <summary>
	/// Gets the version number of the first version of the app legacy Xamarin that was installed on this device.
	/// </summary>
	public static string FirstInstalledVersion => versionTrail[versionsKey].FirstOrDefault();

	/// <summary>
	/// Gets the build number of first version of the legacy Xamarin app that was installed on this device.
	/// </summary>
	public static string FirstInstalledBuild => versionTrail[buildsKey].FirstOrDefault();

	/// <summary>
	/// Gets the collection of version numbers of the legacy Xamarin app that ran on this device.
	/// </summary>
	public static IEnumerable<string> VersionHistory => versionTrail[versionsKey].ToArray();

	/// <summary>
	/// Gets the collection of build numbers of the legacy Xamarin app that ran on this device.
	/// </summary>
	public static IEnumerable<string> BuildHistory => versionTrail[buildsKey].ToArray();

	/// <summary>
	/// Determines if this is the first launch of the app for a specified version number.
	/// </summary>
	/// <param name="version">The version number.</param>
	/// <returns><see langword="true"/> if this is the first launch of the app for the specified version number; otherwise <see langword="false"/>.</returns>
	public static bool IsFirstLaunchForVersion(string version)
		=> CurrentVersion == version && IsFirstLaunchForCurrentVersion;

	/// <summary>
	/// Determines if this is the first launch of the app for a specified build number.
	/// </summary>
	/// <param name="build">The build number.</param>
	/// <returns><see langword="true"/> if this is the first launch of the app for the specified build number; otherwise <see langword="false"/>.</returns>
	public static bool IsFirstLaunchForBuild(string build)
		=> CurrentBuild == build && IsFirstLaunchForCurrentBuild;

	static string[] ReadHistory(string key)
		=> LegacyPreferences.Get(key, null, sharedName)?.Split(new[] { '|' },
			StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();

	static string GetPrevious(string key)
	{
		var trail = versionTrail[key];
		return (trail.Count >= 2) ? trail[trail.Count - 2] : null;
	}

	static string LastInstalledVersion => versionTrail[versionsKey].LastOrDefault();

	static string LastInstalledBuild => versionTrail[buildsKey].LastOrDefault();
}
