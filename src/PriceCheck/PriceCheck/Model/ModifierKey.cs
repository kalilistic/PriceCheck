using System;

namespace PriceCheck
{
    /// <summary>
    /// Modifier key for key binds.
    /// </summary>
    public static class ModifierKey
    {
        /// <summary>
        /// Modifier key names.
        /// </summary>
        public static readonly string[] Names =
        {
            "Shift",
            "Tab",
        };

        /// <summary>
        /// Modifier key enum.
        /// </summary>
        public enum Enum : byte
        {
            /// <summary>
            /// Shift key.
            /// </summary>
            VkShift = 0x10,

            /// <summary>
            /// Tab key.
            /// </summary>
            VkTab = 0x09,
        }

        /// <summary>
        /// Convert enum to index.
        /// </summary>
        /// <param name="value">modifier key enum.</param>
        /// <returns>modifier key index.</returns>
        public static int EnumToIndex(Enum value)
        {
            return Array.IndexOf(Names, value.ToString().Substring(2));
        }

        /// <summary>
        /// Convert index to enum.
        /// </summary>
        /// <param name="i">modifier key index.</param>
        /// <returns>modifier key enum.</returns>
        public static Enum IndexToEnum(int i)
        {
            return (Enum)System.Enum.Parse(typeof(Enum), $"Vk{Names[i]}");
        }
    }
}
