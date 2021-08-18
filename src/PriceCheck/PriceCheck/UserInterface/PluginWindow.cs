using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace PriceCheck
{
    /// <summary>
    /// Plugin window which extends window with PlayerTrack.
    /// </summary>
    public abstract class PluginWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginWindow"/> class.
        /// </summary>
        /// <param name="plugin">PriceCheck plugin.</param>
        /// <param name="windowName">Name of the window.</param>
        /// <param name="flags">ImGui flags.</param>
        protected PluginWindow(PriceCheckPlugin plugin, string windowName, ImGuiWindowFlags flags = ImGuiWindowFlags.None)
            : base(windowName, flags)
        {
            this.Plugin = plugin;
        }

        /// <summary>
        /// Gets PlayerTrack for window.
        /// </summary>
        protected PriceCheckPlugin Plugin { get; }

        /// <inheritdoc/>
        public abstract override void Draw();
    }
}
