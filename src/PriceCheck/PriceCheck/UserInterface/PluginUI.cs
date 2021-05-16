namespace PriceCheck
{
    /// <inheritdoc />
    public class PluginUI : PluginUIBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginUI"/> class.
        /// </summary>
        /// <param name="plugin">price check plugin.</param>
        public PluginUI(IPriceCheckPlugin plugin)
            : base(plugin)
        {
        }
    }
}
