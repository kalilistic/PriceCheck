using Dalamud.DrunkenToad;
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

            // setup window systems
            this.MainWindowSystem = new WindowSystem("PriceCheckMainWindowSystem");
            this.ConfigWindowSystem = new WindowSystem("PriceCheckConfigWindowSystem");

            // add windows
            this.MainWindowSystem.AddWindow(this.MainWindow);
            this.ConfigWindowSystem.AddWindow(this.ConfigWindow);

            // add event listeners
            PriceCheckPlugin.PluginInterface.UiBuilder.Draw += this.Draw;
            PriceCheckPlugin.PluginInterface.UiBuilder.OpenConfigUi += this.OpenConfigUi;
        }

        /// <summary>
        /// Gets main PriceCheck window.
        /// </summary>
        public MainWindow? MainWindow { get; }

        /// <summary>
        /// Gets config PriceCheck window.
        /// </summary>
        public ConfigWindow? ConfigWindow { get; }

        private WindowSystem MainWindowSystem { get; }

        private WindowSystem ConfigWindowSystem { get; }

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
            PriceCheckPlugin.PluginInterface.UiBuilder.Draw -= this.Draw;
            PriceCheckPlugin.PluginInterface.UiBuilder.OpenConfigUi -= this.OpenConfigUi;
            this.MainWindowSystem.RemoveAllWindows();
            this.ConfigWindowSystem.RemoveAllWindows();
        }

        private void Draw()
        {
            // only show when logged in
            if (!PriceCheckPlugin.ClientState.IsLoggedIn) return;

            // draw config if open
            this.ConfigWindowSystem.Draw();

            // run keybind post-click check to use item id set in hover manager
            if (this.Plugin.Configuration.KeybindEnabled && this.Plugin.Configuration.AllowKeybindAfterHover &&
                this.Plugin.IsKeyBindPressed())
            {
                // call price check if item is set from previous hover
                if (this.Plugin.HoveredItemManager.ItemId != 0)
                {
                    this.Plugin.PriceService.ProcessItemAsync(this.Plugin.HoveredItemManager.ItemId, this.Plugin.HoveredItemManager.ItemQuality);
                    this.Plugin.HoveredItemManager.ItemId = 0;
                }
            }

            // draw main window
            if (this.Plugin.Configuration.HideOverlayElapsed != 0 &&
                DateUtil.CurrentTime() - this.Plugin.PriceService.LastPriceCheck >
                this.Plugin.Configuration.HideOverlayElapsed)
            {
                if (!(this.Plugin.Configuration.ShowOverlayByKeybind && this.Plugin.IsKeyBindPressed()))
                {
                    return;
                }
            }

            this.MainWindowSystem.Draw();
        }

        private void OpenConfigUi()
        {
            this.ConfigWindow!.IsOpen ^= true;
        }
    }
}
