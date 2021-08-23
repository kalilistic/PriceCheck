using System;

using Dalamud.DrunkenToad;

namespace PriceCheck
{
    /// <summary>
    /// Manage item hover events.
    /// </summary>
    public class HoveredItemManager
    {
        /// <summary>
        /// Previous id from item hover to allow for holding keybind after hover.
        /// </summary>
        public uint ItemId;

        /// <summary>
        /// Previous item quality from item hover to allow for holding keybind after hover.
        /// </summary>
        public bool ItemQuality;

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
                if (Convert.ToUInt32(itemId) == this.ItemId) return;
                if (itemId >= 1000000)
                {
                    this.ItemId = Convert.ToUInt32(itemId - 1000000);
                    this.ItemQuality = true;
                }
                else
                {
                    this.ItemId = Convert.ToUInt32(itemId);
                    this.ItemQuality = false;
                }

                if (!this.plugin.IsKeyBindPressed()) return;
                this.plugin.PriceService.ProcessItemAsync(this.ItemId, this.ItemQuality);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to price check.");
                this.plugin.ItemCancellationTokenSource = null;
            }
        }
    }
}
