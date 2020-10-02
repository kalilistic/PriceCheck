using System;
using Dalamud.Configuration;

namespace PriceCheck
{
	[Serializable]
	public class PluginConfig : PriceCheckConfig, IPluginConfiguration
	{
		public int Version { get; set; } = 0;
	}
}