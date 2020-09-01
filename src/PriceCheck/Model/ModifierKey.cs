using System;

namespace PriceCheck
{
	public static class ModifierKey
	{
		public enum Enum : byte
		{
			VkShift = 0x10,
			VkTab = 0x09
		}

		public static readonly string[] Names =
		{
			"Shift",
			"Tab"
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