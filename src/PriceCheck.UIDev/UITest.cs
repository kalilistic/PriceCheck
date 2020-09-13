using ImGuiScene;
using PriceCheck.Mock;

namespace PriceCheck.UIDev
{
	internal class UITest : PluginUIBase, IPluginUIMock
	{
		public UITest(IPluginWrapper plugin, IPriceService priceService) : base(plugin, priceService)
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
			var plugin = new MockPluginWrapper();
			var priceService = new MockPriceService();
			UIBootstrap.Initialize(new UITest(plugin, priceService));
		}
	}
}