// ReSharper disable UnusedMember.Global
// ReSharper disable DelegateSubtraction

using System;
using Dalamud.Plugin;

namespace PriceCheck
{
    public class Plugin : IDalamudPlugin
    {
        private PriceCheckPlugin _priceCheckPlugin;

        public string Name => "PriceCheck";

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            _priceCheckPlugin = new PriceCheckPlugin(Name, pluginInterface);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            _priceCheckPlugin.Dispose();
        }
    }
}