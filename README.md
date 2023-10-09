![](nuget.png)
# Plugin.Maui.FormsMigration

`Plugin.Maui.FormsMigration` provides helpers to make your transition from Xamarin.Forms to .NET MAUI easier.

## Install Plugin

[![NuGet](https://img.shields.io/nuget/v/Plugin.Maui.FormsMigration.svg?label=NuGet)](https://www.nuget.org/packages/Plugin.Maui.FormsMigration/)

Available on [NuGet](http://www.nuget.org/packages/Plugin.Maui.FormsMigration).

Install with the dotnet CLI: `dotnet add package Plugin.Maui.FormsMigration`, or through the NuGet Package Manager in Visual Studio.

### Supported Platforms

The supported platforms for this library are Android, iOS and Windows.

## API Usage

This library consists of three APIs that you can use for different scenarios:

* [App Properties](#app-properties)
* [SecureStorage](#securestorage)
* [VersionTracking](#versiontracking)

Each of these APIs will have their own way of using it. You can find the specifics below, or have a look at the [sample application](/samples/Plugin.Maui.FormsMigration.Sample/) that is included in this repository.

> [!NOTE]
> These APIs are meant for transition scenarios only. Use these APIs to retrieve data that is previously stored by your legacy Xamarin application and save them in a place where .NET MAUI can access that data from that point on. This is also the reason that these APIs are read-only.

### App Properties

With Xamarin and Xamarin.Forms you had the possibility to save simple types through the `Properties` API like so: `Application.Current.Properties ["id"] = someClass.ID;`. In .NET MAUI, this is no longer possible. 

To help you transition from Xamarin to .NET MAUI, this library offers the `LegacyApplication` API to still be able to access previously saved properties. From there, save them through a newer API that is available in .NET MAUI. An example could be to use the [Preferences API](https://learn.microsoft.com/dotnet/maui/platform-integration/storage/preferences). See a simple example below.

```csharp
int id;
if (LegacyApplication.Current.Properties.ContainsKey("id"))
{
    id = (int)LegacyApplication.Current.Properties["id"];
    Preferences.Set("id", id);
}
```

> [!NOTE]
> For this API to work properly, you will have to make sure that the application identifier (or bundle identifier) is the same between your legacy Xamarin app and .NET MAUI app. This is needed so that the app gets installed in the same container on the operating system, which is needed to be able to read the property values that were stored previously.

Also see this [Microsoft Learn Docs page](https://learn.microsoft.com/dotnet/maui/migration/app-properties) for more information.

### SecureStorage

The [SecureStorage API](https://learn.microsoft.com/dotnet/maui/platform-integration/storage/secure-storage) is still available in .NET MAUI the same as it is in Xamarin and Xamarin.Forms. However, the name of the store where values were stored and some other details have changed which causes the .NET MAUI version of your app to not being able to retrieve the previously saved secure storage information.

With the `LegacySecureStorage` you can retrieve the previously saved information in the secure storage of your legacy Xamarin application. From there you should resave them through the .NET MAUI secure storage API. An example of how to use this API can be found below.

```csharp
// The code below assumes that there is a secure value saved with the key "oauth_token". Replace this key
// with any value(s) you have stored in your legacy Xamarin app to get them out.

string oauthToken = await LegacySecureStorage.GetAsync("oauth_token");
bool result = LegacySecureStorage.Remove("oauth_token");
await SecureStorage.SetAsync("oauth_token", oauthToken);
```

> [!NOTE]
> For this API to work properly, you will have to make sure that the application identifier (or bundle identifier) is the same between your legacy Xamarin app and .NET MAUI app. This is needed so that the app gets installed in the same container on the operating system, which is needed to be able to read the secure store values that were stored previously.

> [!WARNING]
> For iOS, make sure you have a `Entitlements.plist` with the following entry:
> `<key>keychain-access-groups</key>`
> `<string>$(AppIdentifierPrefix)$(CFBundleIdentifier)</string>`
> `$(AppIdentifierPrefix)$(CFBundleIdentifier)` can stay in place and will be replaced at build time or you can replace it with a hardcoded value.
> Additionally make sure that the `Entitlements.plist` file is set in the Custom Entitlements field for Bundle Signing. For more information, refer to the [Microsoft Learn Docs page about entitlements](https://learn.microsoft.com/dotnet/maui/ios/entitlements).

Also see this [Microsoft Learn Docs page](https://learn.microsoft.com/dotnet/maui/migration/secure-storage) for more information.

### VersionTracking

The [VersionTracking API](https://learn.microsoft.com/dotnet/maui/platform-integration/appmodel/version-tracking) is still available in .NET MAUI the same as it is in Xamarin and Xamarin.Forms. However, the name of the store where values were stored and some other details have changed which causes the .NET MAUI version of your app to not being able to retrieve the previously saved version tracking information.

> [!NOTE]
> For this API to work properly, you will have to make sure that the application identifier (or bundle identifier) is the same between your legacy Xamarin app and .NET MAUI app. This is needed so that the app gets installed in the same container on the operating system, which is needed to be able to read the version information that was stored previously.

> [!WARNING]
> Make sure that the app version of your .NET MAUI app is **higher** than your legacy Xamarin application. Failing to do so might have unexpected results.

# Acknowledgements

This project could not have came to be without these projects and people, thank you! <3

The code in the plugin is mostly based on the original code in [Xamarin.Forms](https://github.com/xamarin/Xamarin.Forms) and [Xamarin.Essentials](https://github.com/xamarin/Essentials/). It has been adapted for the Microsoft Learn Docs by [David Britch](https://github.com/davidbritch).