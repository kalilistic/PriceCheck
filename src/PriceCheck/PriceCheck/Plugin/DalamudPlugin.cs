using Dalamud.Plugin;

namespace PriceCheck
{
    /// <summary>
    /// Base plugin to register with dalamud.
    /// </summary>
    public class DalamudPlugin : IDalamudPlugin
    {
        private PriceCheckPlugin priceCheckPlugin = null!;

        /// <inheritdoc/>
        public string Name => "PriceCheck";

        /// <inheritdoc/>
        public void Dispose()
        {
            this.priceCheckPlugin.Dispose();
        }

        /// <inheritdoc/>
        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.priceCheckPlugin = new PriceCheckPlugin(this.Name, pluginInterface);
        }
    }
}
