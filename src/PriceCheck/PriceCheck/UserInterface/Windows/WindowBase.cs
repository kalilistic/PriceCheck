using ImGuiNET;

namespace PriceCheck
{
    /// <summary>
    /// Window base.
    /// </summary>
    public abstract class WindowBase
    {
        /// <summary>
        /// Gets or sets a value indicating whether gets isVisible.
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Gets UI scale.
        /// </summary>
        protected static float Scale => ImGui.GetIO().FontGlobalScale;

        /// <summary>
        /// Draw view.
        /// </summary>
        public abstract void DrawView();

        /// <summary>
        /// Toggle view.
        /// </summary>
        public void ToggleView()
        {
            this.IsVisible = !this.IsVisible;
        }

        /// <summary>
        /// Show view.
        /// </summary>
        public void ShowView()
        {
            this.IsVisible = true;
        }

        /// <summary>
        /// Hide view.
        /// </summary>
        public void HideView()
        {
            this.IsVisible = false;
        }
    }
}
