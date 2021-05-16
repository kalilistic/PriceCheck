using ImGuiNET;

namespace PriceCheck
{
    /// <inheritdoc />
    public abstract class WindowBase : IWindowBase
    {
        /// <inheritdoc />
        public bool IsVisible { get; set; }

        /// <inheritdoc/>
        public float Scale => ImGui.GetIO().FontGlobalScale;

        /// <inheritdoc />
        public abstract void DrawView();

        /// <inheritdoc />
        public void ToggleView()
        {
            this.IsVisible = !this.IsVisible;
        }

        /// <inheritdoc />
        public void ShowView()
        {
            this.IsVisible = true;
        }

        /// <inheritdoc />
        public void HideView()
        {
            this.IsVisible = false;
        }
    }
}
