using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CheapLoc;
using Dalamud.DrunkenToad;
using XivCommon.Functions.ContextMenu;
using XivCommon.Functions.ContextMenu.Inventory;

namespace PriceCheck
{
    /// <summary>
    /// Manage custom context menu on items.
    /// </summary>
    public class ContextMenuManager
    {
        private readonly PriceCheckPlugin plugin;
        private readonly InventoryContextMenuItem contextMenuItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextMenuManager"/> class.
        /// </summary>
        /// <param name="plugin">plugin.</param>
        public ContextMenuManager(PriceCheckPlugin plugin)
        {
            this.plugin = plugin;
            this.contextMenuItem = new InventoryContextMenuItem(
                                       Loc.Localize("ContextMenuItem", "Check Marketboard Price"), this.ContextItemChanged);
            this.plugin.XivCommon.Functions.ContextMenu.OpenInventoryContextMenu += this.OnOpenInventoryContextMenu;
        }

        /// <summary>
        /// Get index of menu item.
        /// </summary>
        /// <param name="configActionIds">list of defined action ids.</param>
        /// <param name="currentActionIds">list of found action ids in current context.</param>
        /// <returns>action index.</returns>
        public static int? GetActionIndex(ICollection<byte> configActionIds, IReadOnlyList<byte> currentActionIds)
        {
            for (var i = 0; i < currentActionIds.Count; i++)
            {
                if (configActionIds.Contains(currentActionIds[i]))
                {
                    return i;
                }
            }

            return null;
        }

        /// <summary>
        /// Dispose context menu manager.
        /// </summary>
        public void Dispose()
        {
            this.plugin.XivCommon.Functions.ContextMenu.OpenInventoryContextMenu -= this.OnOpenInventoryContextMenu;
        }

        private void OnOpenInventoryContextMenu(InventoryContextMenuOpenArgs args)
        {
            if (!this.plugin.Configuration.ShowContextMenu) return;

            // setup
            var index = 0;
            var actionIds = args.Items.Select(baseContextMenuItem => ((NativeContextMenuItem)baseContextMenuItem).InternalAction).ToList();

            // default
            if (this.plugin.Configuration.ShowContextAboveThis.Count == 0 &&
                this.plugin.Configuration.ShowContextBelowThis.Count == 0)
            {
                args.Items.Add(this.contextMenuItem);
                return;
            }

            // get show above index
            var relativeAboveIndex = GetActionIndex(this.plugin.Configuration.ShowContextAboveThis, actionIds);
            if (relativeAboveIndex != null)
            {
                index = (int)relativeAboveIndex;
                actionIds.RemoveRange(index, actionIds.Count - index);
            }

            // get show below index
            var relativeBelowIndex = GetActionIndex(this.plugin.Configuration.ShowContextBelowThis, actionIds);
            if (relativeBelowIndex != null)
            {
                index = (int)relativeBelowIndex + 1;
            }

            // default to bottom if nothing found
            if (relativeAboveIndex == null && relativeBelowIndex == null)
            {
                index = args.Items.Count;
            }

            // insert price check menu item
            args.Items.Insert(index, this.contextMenuItem);
        }

        private void ContextItemChanged(InventoryContextMenuItemSelectedArgs args)
        {
            try
            {
                if (!this.plugin.ShouldPriceCheck()) return;
                if (this.plugin.ItemCancellationTokenSource != null)
                {
                    if (!this.plugin.ItemCancellationTokenSource.IsCancellationRequested)
                        this.plugin.ItemCancellationTokenSource.Cancel();
                    this.plugin.ItemCancellationTokenSource.Dispose();
                }

                if (args.ItemId == 0)
                {
                    this.plugin.ItemCancellationTokenSource = null;
                    return;
                }

                this.plugin.ItemCancellationTokenSource = new CancellationTokenSource(this.plugin.Configuration.RequestTimeout * 2);
                Task.Run(async () =>
                {
                    await Task.Delay(this.plugin.Configuration.HoverDelay * 1000, this.plugin.ItemCancellationTokenSource!.Token)
                              .ConfigureAwait(false);
                    this.plugin.PriceService.ProcessItem(args.ItemId, args.ItemHq);
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to price check item via context menu.");
            }
        }
    }
}
