using System;

using CheapLoc;
using Dalamud.DrunkenToad;
using Dalamud.Game.Gui.ContextMenus;

namespace PriceCheck
{
    /// <summary>
    /// Manage custom context menu on items.
    /// </summary>
    public class ContextMenuManager
    {
        private readonly PriceCheckPlugin plugin;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextMenuManager"/> class.
        /// </summary>
        /// <param name="plugin">plugin.</param>
        public ContextMenuManager(PriceCheckPlugin plugin)
        {
            this.plugin = plugin;
            PriceCheckPlugin.ContextMenu.ContextMenuOpened += this.OnContextMenuOpened;
        }

        /// <summary>`
        /// Dispose context menu manager.
        /// </summary>
        public void Dispose()
        {
            PriceCheckPlugin.ContextMenu.ContextMenuOpened -= this.OnContextMenuOpened;
        }

        private void OnContextMenuOpened(ContextMenuOpenedArgs args)
        {
            if (!this.plugin.Configuration.ShowContextMenu) return;
            if (args.InventoryItemContext == null) return;
            args.AddCustomItem(Loc.Localize("ContextMenuItem", "Check Marketboard Price"), this.Selected);
        }

        private void Selected(CustomContextMenuItemSelectedArgs args)
        {
            try
            {
                if (args.ContextMenuOpenedArgs.InventoryItemContext == null) return;
                this.plugin.PriceService.ProcessItemAsync(args.ContextMenuOpenedArgs.InventoryItemContext.Id, args.ContextMenuOpenedArgs.InventoryItemContext.IsHighQuality);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to price check item via context menu.");
            }
        }
    }
}
