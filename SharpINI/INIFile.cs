using System.Collections.Generic;

namespace SharpINI
{
	public class INIFile : Dictionary<string, Dictionary<string, string>>
	{
		public INIFile()
		{ }
		public INIFile(IDictionary<string, Dictionary<string, string>> dictionary) : base(dictionary)
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
