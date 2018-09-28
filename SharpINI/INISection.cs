using System.Collections.Generic;

namespace SharpINI
{
	public class INISection : Dictionary<string, string>
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
	}
}
