using Dalamud.Game.Command;

namespace PriceCheck
{
    /// <summary>
    /// Manage plugin commands.
    /// </summary>
    public class CommandManager
    {
        private readonly PriceCheckPlugin plugin;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandManager"/> class.
        /// </summary>
        /// <param name="plugin">plugin.</param>
        public CommandManager(PriceCheckPlugin plugin)
        {
            this.plugin = plugin;
            this.plugin.PluginService.PluginInterface.CommandManager.AddHandler("/pcheck", new CommandInfo(this.TogglePriceCheck)
            {
                HelpMessage = "Show price check.",
                ShowInHelp = true,
            });
            this.plugin.PluginService.PluginInterface.CommandManager.AddHandler("/pricecheck", new CommandInfo(this.TogglePriceCheck)
            {
                ShowInHelp = false,
            });
            this.plugin.PluginService.PluginInterface.CommandManager.AddHandler("/pcheckconfig", new CommandInfo(this.ToggleConfig)
            {
                HelpMessage = "Show price check config.",
                ShowInHelp = true,
            });
            this.plugin.PluginService.PluginInterface.CommandManager.AddHandler("/pricecheckconfig", new CommandInfo(this.ToggleConfig)
            {
                ShowInHelp = false,
            });
        }

        /// <summary>
        /// Dispose command manager.
        /// </summary>
        public void Dispose()
        {
            this.plugin.PluginService.PluginInterface.CommandManager.RemoveHandler("/pcheck");
            this.plugin.PluginService.PluginInterface.CommandManager.RemoveHandler("/pricecheck");
            this.plugin.PluginService.PluginInterface.CommandManager.RemoveHandler("/pcheckconfig");
            this.plugin.PluginService.PluginInterface.CommandManager.RemoveHandler("/pricecheckconfig");
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
