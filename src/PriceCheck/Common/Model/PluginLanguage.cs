using System.Collections.Generic;
using System.Linq;

namespace PriceCheck
{
	public class PluginLanguage
	{
		public static readonly List<PluginLanguage> Languages = new List<PluginLanguage>();
		public static readonly List<string> LanguageNames = new List<string>();

		public static readonly PluginLanguage Default = new PluginLanguage(0, "Default", "default");
		public static readonly PluginLanguage English = new PluginLanguage(1, "English", "en");
		public static readonly PluginLanguage German = new PluginLanguage(2, "Deutsche", "de");
		public static readonly PluginLanguage French = new PluginLanguage(3, "Français", "fr");
		public static readonly PluginLanguage Japanese = new PluginLanguage(4, "日本人", "ja");
		public static readonly PluginLanguage Spanish = new PluginLanguage(5, "Español", "es");
		public static readonly PluginLanguage Italian = new PluginLanguage(6, "Italiano", "it");
		public static readonly PluginLanguage Norwegian = new PluginLanguage(7, "Norsk", "no");
		public static readonly PluginLanguage PortugueseBR = new PluginLanguage(8, "Português (BR)", "pt");
		public static readonly PluginLanguage Russian = new PluginLanguage(9, "Русский язык", "ru");

		public PluginLanguage()
		{
		}

		private PluginLanguage(int index, string name, string code)
		{
			Index = index;
			Name = name;
			Code = code;
			Languages.Add(this);
			LanguageNames.Add(name);
		}

		public int Index { get; set; }
		public string Name { get; set; }
		public string Code { get; set; }

		public static PluginLanguage GetLanguageByIndex(int index)
		{
			return Languages.FirstOrDefault(language => language.Index == index);
		}

		public override string ToString()
		{
			return Name;
		}
	}
}