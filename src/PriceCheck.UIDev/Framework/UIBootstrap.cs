using System.Numerics;
using System.Runtime.InteropServices;
using ImGuiNET;
using ImGuiScene;
using static SDL2.SDL;

namespace PriceCheck.UIDev
{
	internal class UIBootstrap
	{
		public static unsafe void Initialize(IPluginUIMock pluginUI)
		{
			using (var scene = new SimpleImGuiScene(RendererFactory.RendererBackend.DirectX11, new WindowCreateInfo
			{
				Title = "UI Test",
				Fullscreen = true,
				TransparentColor = new float[] {0, 0, 0}
			}))
			{
				scene.Renderer.ClearColor = new Vector4(0, 0, 0, 0);
				scene.Window.OnSDLEvent += (ref SDL_Event sdlEvent) =>
				{
					if (sdlEvent.type == SDL_EventType.SDL_KEYDOWN &&
					    sdlEvent.key.keysym.scancode == SDL_Scancode.SDL_SCANCODE_ESCAPE) scene.ShouldQuit = true;
				};

				ImFontConfigPtr fontConfig = ImGuiNative.ImFontConfig_ImFontConfig();
				fontConfig.MergeMode = true;
				fontConfig.PixelSnapH = true;

				var fontPathJp = @"./resources/NotoSansCJKjp-Medium.otf";
				ImGui.GetIO().Fonts
					.AddFontFromFileTTF(fontPathJp, 17.0f, null, ImGui.GetIO().Fonts.GetGlyphRangesJapanese());

				var fontPathGame = @"./resources/gamesym.ttf";

				var rangeHandle = GCHandle.Alloc(new ushort[]
				{
					0xE020,
					0xE0DB,
					0
				}, GCHandleType.Pinned);


				ImGui.GetIO().Fonts
					.AddFontFromFileTTF(fontPathGame, 17.0f, fontConfig, rangeHandle.AddrOfPinnedObject());

				ImGui.GetIO().Fonts.Build();

				fontConfig.Destroy();
				rangeHandle.Free();


				ImGui.GetStyle().GrabRounding = 3f;
				ImGui.GetStyle().FrameRounding = 4f;
				ImGui.GetStyle().WindowRounding = 4f;
				ImGui.GetStyle().WindowBorderSize = 0f;
				ImGui.GetStyle().WindowMenuButtonPosition = ImGuiDir.Right;
				ImGui.GetStyle().ScrollbarSize = 16f;

				ImGui.GetStyle().Colors[(int) ImGuiCol.WindowBg] = new Vector4(0.06f, 0.06f, 0.06f, 0.87f);
				ImGui.GetStyle().Colors[(int) ImGuiCol.FrameBg] = new Vector4(0.29f, 0.29f, 0.29f, 0.54f);
				ImGui.GetStyle().Colors[(int) ImGuiCol.FrameBgHovered] = new Vector4(0.54f, 0.54f, 0.54f, 0.40f);
				ImGui.GetStyle().Colors[(int) ImGuiCol.FrameBgActive] = new Vector4(0.64f, 0.64f, 0.64f, 0.67f);
				ImGui.GetStyle().Colors[(int) ImGuiCol.TitleBgActive] = new Vector4(0.29f, 0.29f, 0.29f, 1.00f);
				ImGui.GetStyle().Colors[(int) ImGuiCol.CheckMark] = new Vector4(0.86f, 0.86f, 0.86f, 1.00f);
				ImGui.GetStyle().Colors[(int) ImGuiCol.SliderGrab] = new Vector4(0.54f, 0.54f, 0.54f, 1.00f);
				ImGui.GetStyle().Colors[(int) ImGuiCol.SliderGrabActive] = new Vector4(0.67f, 0.67f, 0.67f, 1.00f);
				ImGui.GetStyle().Colors[(int) ImGuiCol.Button] = new Vector4(0.71f, 0.71f, 0.71f, 0.40f);
				ImGui.GetStyle().Colors[(int) ImGuiCol.ButtonHovered] = new Vector4(0.47f, 0.47f, 0.47f, 1.00f);
				ImGui.GetStyle().Colors[(int) ImGuiCol.ButtonActive] = new Vector4(0.74f, 0.74f, 0.74f, 1.00f);
				ImGui.GetStyle().Colors[(int) ImGuiCol.Header] = new Vector4(0.59f, 0.59f, 0.59f, 0.31f);
				ImGui.GetStyle().Colors[(int) ImGuiCol.HeaderHovered] = new Vector4(0.50f, 0.50f, 0.50f, 0.80f);
				ImGui.GetStyle().Colors[(int) ImGuiCol.HeaderActive] = new Vector4(0.60f, 0.60f, 0.60f, 1.00f);
				ImGui.GetStyle().Colors[(int) ImGuiCol.ResizeGrip] = new Vector4(0.79f, 0.79f, 0.79f, 0.25f);
				ImGui.GetStyle().Colors[(int) ImGuiCol.ResizeGripHovered] = new Vector4(0.78f, 0.78f, 0.78f, 0.67f);
				ImGui.GetStyle().Colors[(int) ImGuiCol.ResizeGripActive] = new Vector4(0.88f, 0.88f, 0.88f, 0.95f);
				ImGui.GetStyle().Colors[(int) ImGuiCol.Tab] = new Vector4(0.23f, 0.23f, 0.23f, 0.86f);
				ImGui.GetStyle().Colors[(int) ImGuiCol.TabHovered] = new Vector4(0.71f, 0.71f, 0.71f, 0.80f);
				ImGui.GetStyle().Colors[(int) ImGuiCol.TabActive] = new Vector4(0.36f, 0.36f, 0.36f, 1.00f);

				pluginUI.Initialize(scene);

				scene.Run();

				pluginUI.Dispose();
			}
		}
	}
}