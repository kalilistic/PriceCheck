using System;

namespace PriceCheck
{
    /// <summary>
    /// Gets primary key.
    /// </summary>
    public static class PrimaryKey
    {
        /// <summary>
        /// Get primary key names.
        /// </summary>
        public static readonly string[] Names =
        [
            "None",
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "A",
            "B",
            "C",
            "D",
            "E",
            "F",
            "G",
            "H",
            "I",
            "J",
            "K",
            "L",
            "M",
            "N",
            "O",
            "P",
            "Q",
            "R",
            "S",
            "T",
            "U",
            "V",
            "W",
            "X",
            "Y",
            "Z"
        ];

        /// <summary>
        /// primary key enum.
        /// </summary>
        public enum Enum : byte
        {
            /// <summary>
            /// None.
            /// </summary>
            VkNone,

            /// <summary>
            /// Zero.
            /// </summary>
            Vk0 = 0x30,

            /// <summary>
            /// One.
            /// </summary>
            Vk1,

            /// <summary>
            /// Two.
            /// </summary>
            Vk2,

            /// <summary>
            /// Three.
            /// </summary>
            Vk3,

            /// <summary>
            /// Four.
            /// </summary>
            Vk4,

            /// <summary>
            /// Five.
            /// </summary>
            Vk5,

            /// <summary>
            /// Six.
            /// </summary>
            Vk6,

            /// <summary>
            /// Seven.
            /// </summary>
            Vk7,

            /// <summary>
            /// Eight.
            /// </summary>
            Vk8,

            /// <summary>
            /// Nine.
            /// </summary>
            Vk9,

            /// <summary>
            /// A.
            /// </summary>
            VkA = 0x41,

            /// <summary>
            /// B.
            /// </summary>
            VkB,

            /// <summary>
            /// C.
            /// </summary>
            VkC,

            /// <summary>
            /// D.
            /// </summary>
            VkD,

            /// <summary>
            /// E.
            /// </summary>
            VkE,

            /// <summary>
            /// F.
            /// </summary>
            VkF,

            /// <summary>
            /// G.
            /// </summary>
            VkG,

            /// <summary>
            /// H.
            /// </summary>
            VkH,

            /// <summary>
            /// I.
            /// </summary>
            VkI,

            /// <summary>
            /// J.
            /// </summary>
            VkJ,

            /// <summary>
            /// K.
            /// </summary>
            VkK,

            /// <summary>
            /// L.
            /// </summary>
            VkL,

            /// <summary>
            /// M.
            /// </summary>
            VkM,

            /// <summary>
            /// N.
            /// </summary>
            VkN,

            /// <summary>
            /// O.
            /// </summary>
            VkO,

            /// <summary>
            /// P.
            /// </summary>
            VkP,

            /// <summary>
            /// Q.
            /// </summary>
            VkQ,

            /// <summary>
            /// R.
            /// </summary>
            VkR,

            /// <summary>
            /// S.
            /// </summary>
            VkS,

            /// <summary>
            /// T.
            /// </summary>
            VkT,

            /// <summary>
            /// U.
            /// </summary>
            VkU,

            /// <summary>
            /// V.
            /// </summary>
            VkV,

            /// <summary>
            /// W.
            /// </summary>
            VkW,

            /// <summary>
            /// X.
            /// </summary>
            VkX,

            /// <summary>
            /// Y.
            /// </summary>
            VkY,

            /// <summary>
            /// Z.
            /// </summary>
            VkZ,
        }

        /// <summary>
        /// Convert enum to index.
        /// </summary>
        /// <param name="value">enum.</param>
        /// <returns>index.</returns>
        public static int EnumToIndex(Enum value)
        {
            return Array.IndexOf(Names, value.ToString().Substring(2));
        }

        /// <summary>
        /// Convert index to enum.
        /// </summary>
        /// <param name="i">index.</param>
        /// <returns>enum.</returns>
        public static Enum IndexToEnum(int i)
        {
            return (Enum)System.Enum.Parse(typeof(Enum), $"Vk{Names[i]}");
        }
    }
}
