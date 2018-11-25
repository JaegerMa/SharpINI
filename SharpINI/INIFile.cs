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


		public INIFile Clone()
		{
			return INIFile.FromDictionary(this, deepClone: true);
		}



		public static INIFile FromDictionary(IDictionary<string, IDictionary<string, string>> dictionary, bool deepClone = true)
		{
			IEnumerable<(string, IDictionary<string, string>)> entries;
			if(deepClone)
				entries = GetClonedEntries();
			else
				entries = GetEntries();


			var iniFile = new INIFile();

			//We need to access the original index-setter
			//from IDictionary as this allows IDictionary<string, string> to be set
			//while the one from INIFile only accepts INISections
			IDictionary<string, IDictionary<string, string>> iniDictionary = iniFile;
			foreach(var (key, value) in entries)
				iniDictionary[key] = value;


			return iniFile;



			IEnumerable<(string, IDictionary<string, string>)> GetEntries()
			{
				foreach(var entry in dictionary)
					yield return (entry.Key, entry.Value);
			}
			IEnumerable<(string, IDictionary<string, string>)> GetClonedEntries()
			{
				foreach(var entry in dictionary)
				{
					var section = new INISection(entry.Value);

					yield return (entry.Key, section);
				}
			}
		}
	}
}
