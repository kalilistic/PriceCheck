using System;
using System.Diagnostics;
using System.Numerics;
using CheapLoc;
using ImGuiNET;

namespace PriceCheck
{
	public class MainWindow : WindowBase
	{
		private readonly IPluginWrapper _plugin;

		public MainWindow(IPluginWrapper plugin)
		{
			_plugin = plugin;
		}

		public event EventHandler<bool> OverlayVisibilityUpdated;
		public event EventHandler<bool> SettingsVisibilityUpdated;

		public void DrawWindow()
		{
			if (!IsVisible) return;
			ImGui.SetNextWindowSizeConstraints(new Vector2(0, 0), new Vector2(float.MaxValue, float.MaxValue));
			if (ImGui.Begin(Loc.Localize("MainWindow", "PriceCheck") + "###PriceCheck_Main_Window",
				ref IsVisible,
				ImGuiWindowFlags.AlwaysAutoResize))
			{
				if (ImGui.Button(Loc.Localize("ToggleOverlay", "Overlay") + "###PriceCheck_Overlay_Button",
					new Vector2(100, 30)))
				{
					_plugin.GetConfig().ShowOverlay = !_plugin.GetConfig().ShowOverlay;
					OverlayVisibilityUpdated?.Invoke(this, _plugin.GetConfig().ShowOverlay);
					_plugin.GetConfig().Save();
				}

				if (ImGui.Button(Loc.Localize("ToggleSettings", "Settings") + "###PriceCheck_Settings_Button",
					new Vector2(100, 30)))
					SettingsVisibilityUpdated?.Invoke(this, true);

				if (ImGui.Button(Loc.Localize("OpenTranslate", "Translate") + "###PriceCheck_Translate_Button",
					new Vector2(100, 30)))
					Process.Start("https://crowdin.com/project/pricecheck");

				if (ImGui.Button(Loc.Localize("OpenGithub", "Github") + "###PriceCheck_Github_Button",
					new Vector2(100, 30)))
					Process.Start("https://github.com/kalilistic/PriceCheck");

				if (ImGui.Button(Loc.Localize("PrintHelp", "Help") + "###PriceCheck_Help_Button",
					new Vector2(100, 30)))
					_plugin.PrintHelpMessage();
			}

			ImGui.End();
		}
	}
}