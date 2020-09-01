using System;
using ImGuiScene;

namespace PriceCheck.UIDev
{
	internal interface IPluginUIMock : IDisposable
	{
		void Initialize(SimpleImGuiScene scene);
	}
}