using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpINI
{
    public static class INIReader
	{
		public static INIFile ReadINI(string input, ParseOptions options = null)
		{
			options = options ?? ParseOptions.Default;

			foreach(var lineBreak in options.lineBreaks)
				input = input.Replace(lineBreak, "\n");

			var stringView = new StringView(input, options: options);
			return ReadINI(stringView, options);
		}
#if SHARE_INTERNAL_METHODS
		public
#endif
		static INIFile ReadINI(StringView input, ParseOptions options)
		{
			var sections = new INIFile();

			INISection currentSection = null;
			string currentSectionTitle = null;

			while(input.Length != 0)
			{
				if(options.lineStartCommentChars.Contains(input[0]))
				{
					ReadStringUntil(input, '\n', acceptEOF: true);
					continue;
				}

				input.TrimWhitespace();

				switch(input[0])
				{
					case '\n':
						input.Move(1);
						continue;
					case '[':
						var sectionTitle = ReadSectionTitle(input);
						AddSection(sectionTitle);
						break;
					default:
						var (key, value) = ReadKeyValue(input, options);
						AddKey(key, value);
						break;

				}
			}

			return sections;



			void NewSection(string name)
			{
				currentSection = new INISection();
				currentSectionTitle = name;

				sections[name] = currentSection;
			}
			void AddSection(string name)
			{
				if(!sections.ContainsKey(name))
				{
					NewSection(name);
					return;
				}

				switch(options.multiSectionMode)
				{
					case MultiSectionMode.DISALLOW:
						throw new Exception($"Section '{name}' was already declared");
					case MultiSectionMode.IGNORE:
						currentSection = null;
						currentSectionTitle = null;
						return;
					case MultiSectionMode.REPLACE:
						NewSection(name);
						break;
					case MultiSectionMode.MERGE:
						currentSection = sections[name];
						break;
				}
			}
			void AddKey(string key, string value)
			{
				if(currentSection == null)
				{
					if(sections.Count != 0)
						return;

					if(options.initialSectionName == null)
						throw new Exception("Keys outside of a section");
					AddSection(options.initialSectionName);
				}
					

				if(!currentSection.ContainsKey(key))
				{
					currentSection[key] = value;
					return;
				}

				switch(options.multiKeyMode)
				{
					case MultiKeyMode.DISALLOW:
						throw new Exception($"Key '{key} in section '{currentSectionTitle}'");
					case MultiKeyMode.IGNORE:
						return;
					case MultiKeyMode.OVERRIDE:
						currentSection[key] = value;
						break;
				}
			}
		}

#if SHARE_INTERNAL_METHODS
		public
#endif
		static string ReadSectionTitle(StringView input)
		{
#if SHARE_INTERNAL_METHODS
			input.TrimWhitespace();
			if(input.Length < 1 || input[0] != '[')
				throw new Exception("Not a section");
#endif

			input.Move(1);
			string title = ReadStringUntil(input, ']');
			input.TrimWhitespace();

			if(input.Length != 0)
			{
				if(input[0] != '\n')
					throw new Exception($"Unexpected char at offset {input.offset}. Expected line-break");

				input.Move(1);
			}

			return title;
		}

#if SHARE_INTERNAL_METHODS
		public
#endif
		static (string, string) ReadKeyValue(StringView input, ParseOptions options)
		{
#if SHARE_INTERNAL_METHODS
			input.TrimWhitespace();
#endif
			string key = ReadStringUntil(input, '=');
			key = key.TrimEnd(options.spaceChars);

			string value = ReadStringUntil(input, '\n', acceptEOF: true);
			if(options.trimSpaceBeforeValue)
				value = value.TrimStart(options.spaceChars);
			if(options.trimSpaceAfterValue)
				value = value.TrimEnd(options.spaceChars);


			return (key, value);
		}


#if SHARE_INTERNAL_METHODS
		public
#endif
		static string ReadStringUntil(StringView input, char endChar, bool acceptEOF = false)
		{
			StringBuilder sb = new StringBuilder();
			while(input.Length > 0)
			{
				char c = input[0];
				input.Move(1);

				if(c == endChar)
					return sb.ToString();
				if(c == '\n')
					throw new Exception($"Unexpected end of line. Expected char '{endChar}' at offset {input.offset}");

				sb.Append(c);
			}

			if(acceptEOF)
				return sb.ToString();

			throw new Exception($"Unexpected end of string. Waiting for char '{endChar}'");
		}
    }

	public class INIFile : Dictionary<string, Dictionary<string, string>>
	{
		public new INISection this[string key]
		{
			get
			{
				if(!this.ContainsKey(key))
					return null;

				return base[key] as INISection;
			}
			set => base[key] = value;
		}
	}
	public class INISection : Dictionary<string, string>
	{
		public new string this[string key]
		{
			get
			{
				if(!this.ContainsKey(key))
					return null;

				return base[key];
			}
			set => base[key] = value;
		}
	}
}
