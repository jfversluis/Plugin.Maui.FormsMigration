using Plugin.Maui.FormsMigration;

namespace Plugin.Maui.FormsMigration.Sample;

public partial class MainPage : ContentPage
{
	readonly IFeature feature;

	public MainPage(IFeature feature)
	{
		InitializeComponent();
		
		this.feature = feature;
	}
}
