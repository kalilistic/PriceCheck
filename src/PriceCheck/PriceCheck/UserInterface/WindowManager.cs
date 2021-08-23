using System;

using Dalamud.Interface;
using Dalamud.Interface.Windowing;

namespace PriceCheck
{
    /// <summary>
    /// Window manager to hold plugin windows and window system.
    /// </summary>
    public class WindowManager
    {
        /// <summary>
        /// Is main window open due to keybind being held.
        /// </summary>
        public bool IsOpenByKeybind;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowManager"/> class.
        /// </summary>
        /// <param name="priceCheckPlugin">PriceCheck plugin.</param>
        public WindowManager(PriceCheckPlugin priceCheckPlugin)
        {
            this.Plugin = priceCheckPlugin;

            // create windows
            this.MainWindow = new MainWindow(this.Plugin);
            this.ConfigWindow = new ConfigWindow(this.Plugin);

            // setup window system
            this.WindowSystem = new WindowSystem("PriceCheckWindowSystem");
            this.WindowSystem.AddWindow(this.MainWindow);
            this.WindowSystem.AddWindow(this.ConfigWindow);

            // add event listeners
            this.Plugin.PluginService.PluginInterface.UiBuilder.OnBuildUi += this.OnBuildUi;
            this.Plugin.PluginService.PluginInterface.UiBuilder.OnOpenConfigUi += this.OnOpenConfigUi;
        }

        /// <summary>
        /// Gets main PriceCheck window.
        /// </summary>
        public MainWindow? MainWindow { get; }

        /// <summary>
        /// Gets config PriceCheck window.
        /// </summary>
        public ConfigWindow? ConfigWindow { get; }

        private WindowSystem WindowSystem { get; }

        private PriceCheckPlugin Plugin { get; }

        /// <summary>
        /// Create a dummy scaled for use with tabs.
        /// </summary>
        public static void SpacerWithTabs()
        {
            ImGuiHelpers.ScaledDummy(1f);
        }

        /// <summary>
        /// Create a dummy scaled for use without tabs.
        /// </summary>
        public static void SpacerNoTabs()
        {
            ImGuiHelpers.ScaledDummy(28f);
        }

        /// <summary>
        /// Dispose plugin windows and commands.
        /// </summary>
        public void Dispose()
        {
            this.Plugin.PluginService.PluginInterface.UiBuilder.OnBuildUi -= this.OnBuildUi;
            this.Plugin.PluginService.PluginInterface.UiBuilder.OnOpenConfigUi -= this.OnOpenConfigUi;
            this.WindowSystem.RemoveAllWindows();
        }

        private void OnBuildUi()
        {
            // only show when logged in
            if (!this.Plugin.PluginService.ClientState.IsLoggedIn()) return;

            // check if keybind is pressed
            if (this.Plugin.IsKeyBindPressed())
            {
                if (this.Plugin.Configuration.ShowOverlay && this.Plugin.Configuration.ShowOverlayByKeybind && !this.MainWindow!.IsOpen)
                {
                    this.IsOpenByKeybind = true;
                    this.MainWindow.IsOpen = true;
                }

                if (this.Plugin.HoveredItemManager.ItemId != 0)
                {
                    this.Plugin.PriceService.ProcessItemAsync(this.Plugin.HoveredItemManager.ItemId, this.Plugin.HoveredItemManager.ItemQuality);
                }
            }
            else
            {
                this.IsOpenByKeybind = false;
            }

            // draw windows
            this.WindowSystem.Draw();
        }

        private void OnOpenConfigUi(object sender, EventArgs e)
        {
            this.ConfigWindow!.IsOpen ^= true;
        }
    }
}
