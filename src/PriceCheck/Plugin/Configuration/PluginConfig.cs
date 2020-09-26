using System;
using Dalamud.Configuration;

namespace PriceCheck
{
	[Serializable]
	public class PluginConfig : PriceCheckConfig, IPluginConfiguration
	{
	}
}