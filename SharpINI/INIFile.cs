using System.Collections.Generic;

namespace SharpINI
{
	public class INIFile : OrderedDictionary<string, IDictionary<string, string>>
	{
		public INIFile()
		{ }
		public INIFile(IDictionary<string, IDictionary<string, string>> dictionary) : base(dictionary)
		{ }

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
}
