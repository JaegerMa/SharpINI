using System;
using System.Collections.Generic;
using System.Text;

namespace SharpINI
{
    public class INIWriter
    {
		public static string WriteINI(Dictionary<string, Dictionary<string, string>> sections, RenderOptions options = null)
		{
			options = options ?? RenderOptions.Default;

			var sb = new StringBuilder();
			WriteINI(sections, sb, options);

			return sb.ToString();
		}
		public static void WriteINI(Dictionary<string, Dictionary<string, string>> sections, StringBuilder sb, RenderOptions options = null)
		{
			bool firstSectionWritten = false;

			if(options.initialSectionName != null && sections.ContainsKey(options.initialSectionName))
			{
				foreach(var entry in sections[options.initialSectionName])
					WriteKeyValue(entry.Key, entry.Value, sb, options);

				firstSectionWritten = true;
			}


			foreach(var entry in sections)
			{
				if(firstSectionWritten)
					WriteInterSectionLineBreaks(sb, options);

				WriteSection(entry.Key, entry.Value, sb, options);
				firstSectionWritten = true;
			}
		}
		public static void WriteSection(string name, Dictionary<string, string> childs, StringBuilder sb, RenderOptions options = null)
		{
			options = options ?? RenderOptions.Default;

			if(name == null)
				return;

			sb.Append("[");
			sb.Append(name.Replace("]", ""));
			sb.Append("]");
			sb.Append(options.lineBreak);

			foreach(var entry in childs)
				WriteKeyValue(entry.Key, entry.Value, sb, options);
		}
		public static void WriteKeyValue(string name, string value, StringBuilder sb, RenderOptions options = null)
		{
			options = options ?? RenderOptions.Default;

			if(name == null)
				return;

			sb.Append(name.Replace("=", ""));

			if(options.spaceAfterKey)
				sb.Append(options.space);
			sb.Append("=");
			if(options.spaceBeforeValue)
				sb.Append(options.space);

			sb.Append(value);

			sb.Append(options.lineBreak);
		}


		static void WriteInterSectionLineBreaks(StringBuilder sb, RenderOptions options = null)
		{
			options = options ?? RenderOptions.Default;

			for(var i = 0; i < options.linesBetweenSections; ++i)
				sb.Append(options.lineBreak);
		}
	}
}
