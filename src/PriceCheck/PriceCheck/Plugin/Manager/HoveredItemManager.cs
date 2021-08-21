using System;
using System.Threading;
using System.Threading.Tasks;

using Dalamud.DrunkenToad;

namespace PriceCheck
{
    /// <summary>
    /// Manage item hover events.
    /// </summary>
    public class HoveredItemManager
    {
        private readonly PriceCheckPlugin plugin;

        /// <summary>
        /// Initializes a new instance of the <see cref="HoveredItemManager"/> class.
        /// </summary>
        /// <param name="plugin">plugin.</param>
        public HoveredItemManager(PriceCheckPlugin plugin)
        {
            this.plugin = plugin;
            this.plugin.PluginService.PluginInterface.Framework.Gui.HoveredItemChanged += this.HoveredItemChanged;
        }

        /// <summary>
        /// Dispose hover item events.
        /// </summary>
        public void Dispose()
        {
            this.plugin.PluginService.PluginInterface.Framework.Gui.HoveredItemChanged -= this.HoveredItemChanged;
        }

        private void HoveredItemChanged(object sender, ulong itemId)
        {
            try
            {
                if (!this.plugin.ShouldPriceCheck()) return;
                if (!this.plugin.IsKeyBindPressed()) return;
                if (this.plugin.ItemCancellationTokenSource != null)
                {
                    if (!this.plugin.ItemCancellationTokenSource.IsCancellationRequested)
                        this.plugin.ItemCancellationTokenSource.Cancel();
                    this.plugin.ItemCancellationTokenSource.Dispose();
                }

                if (itemId == 0)
                {
                    this.plugin.ItemCancellationTokenSource = null;
                    return;
                }

                this.plugin.ItemCancellationTokenSource = new CancellationTokenSource(this.plugin.Configuration.RequestTimeout * 2);
                Task.Run(async () =>
                {
                    await Task.Delay(this.plugin.Configuration.HoverDelay * 1000, this.plugin.ItemCancellationTokenSource!.Token)
                              .ConfigureAwait(false);
                    if (itemId >= 1000000)
                    {
                        this.plugin.PriceService.ProcessItem(Convert.ToUInt32(itemId - 1000000), true);
                    }
                    else
                    {
                        this.plugin.PriceService.ProcessItem(Convert.ToUInt32(itemId), false);
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to price check.");
                this.plugin.ItemCancellationTokenSource = null;
            }
        }
    }
}
