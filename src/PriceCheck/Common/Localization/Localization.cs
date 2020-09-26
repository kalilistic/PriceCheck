using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using CheapLoc;

namespace PriceCheck
{
	public class Localization : ILocalization
	{
		private readonly IPluginBase _plugin;

		public Localization(IPluginBase plugin)
		{
			_plugin = plugin;
		}

		internal void SetLanguage(int pluginLanguageIndex)
		{
			var language = PluginLanguage.GetLanguageByIndex(pluginLanguageIndex);
			SetLanguage(language);
		}

		internal void SetLanguage(PluginLanguage language)
		{
			var languageCode = GetLanguageCode(language);
			_plugin.LogInfo("Attempting to load lang {0}", languageCode);
			if (languageCode != PluginLanguage.English.Code)
			{
				var locData = LoadLocData(languageCode);
				Loc.Setup(locData);
			}
			else
			{
				Loc.SetupWithFallbacks();
			}

			Result.UpdateLanguage();
		}

		internal string GetLanguageCode(PluginLanguage language)
		{
			var languageCode = language.Code.ToLower();
			if (languageCode == PluginLanguage.Default.Code)
			{
				languageCode = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLower();
				_plugin.LogInfo("PluginLanguage is default so using UICulture {0}", languageCode);
			}

			if (PluginLanguage.Languages.Any(lang => languageCode == lang.Code)) return languageCode;
			_plugin.LogInfo("UICulture {0} is not supported so using fallback", languageCode);
			languageCode = PluginLanguage.English.Code;
			return languageCode;
		}

		internal string LoadLocData(string languageCode)
		{
			var resourceFile = $"{_plugin.PluginName}.Resource.{languageCode}.json";
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