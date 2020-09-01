using ImGuiScene;
using PriceCheck.Mock;

namespace PriceCheck.UIDev
{
	internal class UITest : PluginUIBase, IPluginUIMock
	{
		public UITest(Configuration configuration, IPriceService priceService) : base(configuration, priceService)
		{
		}

		public void Initialize(SimpleImGuiScene scene)
		{
			scene.OnBuildUI += Draw;
			OverlayWindow.IsVisible = true;
			SettingsWindow.IsVisible = true;
		}

		public static void Main()
		{
			var config = new MockConfig();
			var priceService = new MockPriceService();
			UIBootstrap.Initialize(new UITest(config, priceService));
		}
	}
}