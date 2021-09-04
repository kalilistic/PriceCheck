using Dalamud.Game.Command;

namespace PriceCheck
{
    /// <summary>
    /// Manage plugin commands.
    /// </summary>
    public class PluginCommandManager
    {
        private readonly PriceCheckPlugin plugin;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginCommandManager"/> class.
        /// </summary>
        /// <param name="plugin">plugin.</param>
        public PluginCommandManager(PriceCheckPlugin plugin)
        {
            this.plugin = plugin;
            PriceCheckPlugin.CommandManager.AddHandler("/pcheck", new CommandInfo(this.TogglePriceCheck)
            {
                HelpMessage = "Show price check.",
                ShowInHelp = true,
            });
            PriceCheckPlugin.CommandManager.AddHandler("/pricecheck", new CommandInfo(this.TogglePriceCheck)
            {
                ShowInHelp = false,
            });
            PriceCheckPlugin.CommandManager.AddHandler("/pcheckconfig", new CommandInfo(this.ToggleConfig)
            {
                HelpMessage = "Show price check config.",
                ShowInHelp = true,
            });
            PriceCheckPlugin.CommandManager.AddHandler("/pricecheckconfig", new CommandInfo(this.ToggleConfig)
            {
                ShowInHelp = false,
            });
        }

        /// <summary>
        /// Dispose command manager.
        /// </summary>
        public static void Dispose()
        {
            PriceCheckPlugin.CommandManager.RemoveHandler("/pcheck");
            PriceCheckPlugin.CommandManager.RemoveHandler("/pricecheck");
            PriceCheckPlugin.CommandManager.RemoveHandler("/pcheckconfig");
            PriceCheckPlugin.CommandManager.RemoveHandler("/pricecheckconfig");
        }

        private void ToggleConfig(string command, string args)
        {
            this.plugin.WindowManager.ConfigWindow!.Toggle();
        }

        private void TogglePriceCheck(string command, string args)
        {
            this.plugin.Configuration.ShowOverlay = !this.plugin.Configuration.ShowOverlay;
            this.plugin.WindowManager.MainWindow!.Toggle();
        }
    }
}
