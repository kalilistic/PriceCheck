using ImGuiScene;
using PriceCheck.Mock;

namespace PriceCheck.UIDev
{
	internal class UITest : PluginUIBase, IPluginUIMock
	{
		public UITest(IPriceCheckPlugin priceCheckPlugin) : base(priceCheckPlugin)
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
			var plugin = new MockPriceCheckPlugin();
			UIBootstrap.Initialize(new UITest(plugin));
		}
	}
}