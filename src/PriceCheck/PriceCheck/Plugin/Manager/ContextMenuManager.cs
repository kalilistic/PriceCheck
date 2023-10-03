using System;

using CheapLoc;
using Dalamud.ContextMenu;
using Dalamud.DrunkenToad;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Logging;

namespace PriceCheck
{
    /// <summary>
    /// Manage custom context menu on items.
    /// </summary>
    public class ContextMenuManager
    {
        private readonly PriceCheckPlugin plugin;
        private readonly InventoryContextMenuItem inventoryContextMenuItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextMenuManager"/> class.
        /// </summary>
        /// <param name="plugin">plugin.</param>
        public ContextMenuManager(PriceCheckPlugin plugin)
        {
            this.plugin = plugin;
            this.inventoryContextMenuItem = new InventoryContextMenuItem(
                new SeString(new TextPayload(Loc.Localize("ContextMenuItem", "Check Marketboard Price"))), this.Selected);
            this.plugin.ContextMenu.OnOpenInventoryContextMenu += this.OnContextMenuOpened;
        }

        /// <summary>`
        /// Dispose context menu manager.
        /// </summary>
        public void Dispose()
        {
            this.plugin.ContextMenu.OnOpenInventoryContextMenu -= this.OnContextMenuOpened;
        }

        private void OnContextMenuOpened(InventoryContextMenuOpenArgs args)
        {
            if (!this.plugin.Configuration.ShowContextMenu) return;
            if (args.ItemId == 0) return;
            args.AddCustomItem(this.inventoryContextMenuItem);
        }

        private void Selected(InventoryContextMenuItemSelectedArgs args)
        {
            try
            {
                if (args.ItemId == 0) return;
                this.plugin.PriceService.ProcessItemAsync(args.ItemId, args.ItemHq);
            }
            catch (Exception ex)
            {
                PluginLog.LogError(ex, "Failed to price check item via context menu.");
            }
        }
    }
}
