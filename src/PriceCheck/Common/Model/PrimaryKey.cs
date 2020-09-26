using System;

namespace PriceCheck
{
	public static class PrimaryKey
	{
		public enum Enum : byte
		{
			VkNone,
			Vk0 = 0x30,
			Vk1,
			Vk2,
			Vk3,
			Vk4,
			Vk5,
			Vk6,
			Vk7,
			Vk8,
			Vk9,
			VkA = 0x41,
			VkB,
			VkC,
			VkD,
			VkE,
			VkF,
			VkG,
			VkH,
			VkI,
			VkJ,
			VkK,
			VkL,
			VkM,
			VkN,
			VkO,
			VkP,
			VkQ,
			VkR,
			VkS,
			VkT,
			VkU,
			VkV,
			VkW,
			VkX,
			VkY,
			VkZ
		}

		public static readonly string[] Names =
		{
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
		};

		public static int EnumToIndex(Enum value)
		{
			return Array.IndexOf(Names, value.ToString().Substring(2));
		}

		public static Enum IndexToEnum(int i)
		{
			return (Enum) System.Enum.Parse(typeof(Enum), $"Vk{Names[i]}");
		}
	}
}