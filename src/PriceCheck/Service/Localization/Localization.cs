using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using CheapLoc;

namespace PriceCheck
{
	public class Localization : ILocalization
	{
		private readonly IPluginWrapper _plugin;

		public Localization(IPluginWrapper plugin)
		{
			_plugin = plugin;
			SetLanguage();
		}

		internal void SetLanguage()
		{
			var languageCode = GetLanguageCode();
			_plugin.LogInfo("Attempting to load lang {0}", languageCode);
			SetupLanguage(languageCode);
		}

		internal void SetupLanguage(string languageCode)
		{
			if (languageCode != PluginLanguage.English.Code)
			{
				var locData = LoadLocData(languageCode);
				Loc.Setup(locData);
			}
			else
			{
				Loc.SetupWithFallbacks();
			}
		}

		internal string GetLanguageCode()
		{
			var languageCode = PluginLanguage.GetLanguageByIndex(_plugin.GetConfig().PluginLanguage).Code.ToLower();
			if (languageCode == PluginLanguage.Default.Code)
			{
				languageCode = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLower();
				_plugin.LogInfo("PluginLanguage is default so using UICulture {0}", languageCode);
			}

			if (PluginLanguage.Languages.All(lang => languageCode != lang.Code))
			{
				_plugin.LogInfo("UICulture {0} is not supported so using fallback", languageCode);
				languageCode = PluginLanguage.English.Code;
			}

			return languageCode;
		}

		internal string LoadLocData(string languageCode)
		{
			var resourceFile = $"PriceCheck.Resource.{languageCode}.json";
			_plugin.LogInfo("Loading lang resource file {0}", resourceFile);
			var assembly = GetAssembly();
			var resourceStream =
				assembly.GetManifestResourceStream(resourceFile);
			if (resourceStream == null) return null;
			using (var reader = new StreamReader(resourceStream))
			{
				return reader.ReadToEnd();
			}
		}

		internal void ExportLocalizable()
		{
			Loc.ExportLocalizableForAssembly(GetAssembly());
		}

		private static Assembly GetAssembly()
		{
			return Assembly.GetExecutingAssembly();
		}
	}
}