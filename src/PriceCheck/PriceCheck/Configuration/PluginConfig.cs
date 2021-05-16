using System;

using Dalamud.Configuration;

namespace PriceCheck
{
    /// <summary>
    /// Plugin configuration class used for dalamud.
    /// </summary>
    [Serializable]
    public class PluginConfig : PriceCheckConfig, IPluginConfiguration
    {
        /// <inheritdoc/>
        public int Version { get; set; } = 0;
    }
}
