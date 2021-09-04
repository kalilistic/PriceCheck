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
            PriceCheckPlugin.GameGui.HoveredItemChanged += this.HoveredItemChanged;
        }

        /// <summary>
        /// Dispose hover item events.
        /// </summary>
        public void Dispose()
        {
            PriceCheckPlugin.GameGui.HoveredItemChanged -= this.HoveredItemChanged;
        }

        private void HoveredItemChanged(object? sender, ulong itemId)
        {
            try
            {
                // cancel in-flight request
                if (this.plugin.ItemCancellationTokenSource != null)
                {
                    if (!this.plugin.ItemCancellationTokenSource.IsCancellationRequested)
                        this.plugin.ItemCancellationTokenSource.Cancel();
                    this.plugin.ItemCancellationTokenSource.Dispose();
                }

                // stop if invalid itemId
                if (itemId == 0) return;

                // capture itemId/quality
                uint realItemId;
                bool itemQuality;
                if (itemId >= 1000000)
                {
                    realItemId = Convert.ToUInt32(itemId - 1000000);
                    itemQuality = true;
                }
                else
                {
                    realItemId = Convert.ToUInt32(itemId);
                    itemQuality = false;
                }

                // if keybind without pre-click
                if (this.plugin.Configuration.KeybindEnabled && !this.plugin.Configuration.AllowKeybindAfterHover)
                {
                    // call immediately
                    if (!this.plugin.IsKeyBindPressed()) return;
                    this.plugin.PriceService.ProcessItemAsync(realItemId, itemQuality);
                    return;
                }

                // if keybind post-click
                if (this.plugin.Configuration.KeybindEnabled && this.plugin.Configuration.AllowKeybindAfterHover)
                {
                    if (this.plugin.IsKeyBindPressed())
                    {
                        // call immediately
                        this.plugin.PriceService.ProcessItemAsync(realItemId, itemQuality);
                    }
                    else
                    {
                        // save for next keybind press
                        this.ItemId = realItemId;
                        this.ItemQuality = itemQuality;
                    }

                    return;
                }

                // if no keybind
                if (!this.plugin.Configuration.KeybindEnabled)
                {
                    this.plugin.PriceService.ProcessItemAsync(realItemId, itemQuality);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to price check.");
                this.ItemId = 0;
                this.plugin.ItemCancellationTokenSource = null;
            }
        }
    }
}
