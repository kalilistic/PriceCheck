namespace PriceCheck
{
    /// <summary>
    /// Window base.
    /// </summary>
    public interface IWindowBase
    {
        /// <summary>
        /// Gets UI scale.
        /// </summary>
        float Scale { get; }

        /// <summary>
        /// Gets or sets a value indicating whether gets isVisible.
        /// </summary>
        bool IsVisible { get; set; }

        /// <summary>
        /// Draw view.
        /// </summary>
        void DrawView();

        /// <summary>
        /// Toggle view.
        /// </summary>
        void ToggleView();

        /// <summary>
        /// Show view.
        /// </summary>
        void ShowView();

        /// <summary>
        /// Hide view.
        /// </summary>
        void HideView();
    }
}
