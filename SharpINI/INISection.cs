using System.Collections.Generic;

namespace SharpINI
{
	public class INISection : OrderedDictionary<string, string>
	{
		public INISection()
		{ }
		public INISection(IDictionary<string, string> dictionary) : base(dictionary)
		{ }

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



		public static INISection FromDictionary(IDictionary<string, string> dictionary)
		{
			return new INISection(dictionary);
		}
	}
}
