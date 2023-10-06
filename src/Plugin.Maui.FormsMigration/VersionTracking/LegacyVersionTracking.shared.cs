using Microsoft.Maui.ApplicationModel;

namespace Plugin.Maui.FormsMigration;

public static class LegacyVersionTracking
{
	const string versionsKey = "VersionTracking.Versions";
	const string buildsKey = "VersionTracking.Builds";

	static readonly string sharedName = LegacyPreferences.GetPrivatePreferencesSharedName("versiontracking");

	static Dictionary<string, List<string>> versionTrail = new();

	public static string VersionsKey => versionsKey;
	public static string BuildsKey => buildsKey;
	public static string SharedName => sharedName;

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

	public static void Track()
	{
	}

	public static bool IsFirstLaunchEver { get; private set; }

	public static bool IsFirstLaunchForCurrentVersion { get; private set; }

	public static bool IsFirstLaunchForCurrentBuild { get; private set; }

	public static string CurrentVersion => AppInfo.VersionString;

	public static string CurrentBuild => AppInfo.BuildString;

	public static string PreviousVersion => GetPrevious(versionsKey);

	public static string PreviousBuild => GetPrevious(buildsKey);

	public static string FirstInstalledVersion => versionTrail[versionsKey].FirstOrDefault();

	public static string FirstInstalledBuild => versionTrail[buildsKey].FirstOrDefault();

	public static IEnumerable<string> VersionHistory => versionTrail[versionsKey].ToArray();

	public static IEnumerable<string> BuildHistory => versionTrail[buildsKey].ToArray();

	public static bool IsFirstLaunchForVersion(string version)
		=> CurrentVersion == version && IsFirstLaunchForCurrentVersion;

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
