using System;
using Dalamud.Configuration;
using Dalamud.Plugin;

namespace PriceCheck
{
	[Serializable]
	public class PluginConfiguration : Configuration, IPluginConfiguration
	{
		[NonSerialized] private DalamudPluginInterface _pluginInterface;

		public void Initialize(DalamudPluginInterface pluginInterface)
		{
			_pluginInterface = pluginInterface;
		}

		public override void Save()
		{
			_pluginInterface.SavePluginConfig(this);
		}
	}
}