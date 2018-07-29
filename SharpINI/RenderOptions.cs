using System;
using System.Collections.Generic;
using System.Text;

namespace SharpINI
{
    public class RenderOptions
    {
		public static RenderOptions Default => new RenderOptions();


		public string lineBreak;
		public string space;
		public bool spaceAfterKey;
		public bool spaceBeforeValue;
		public int linesBetweenSections;
		public string initialSectionName;

		public RenderOptions(string lineBreak = "\n",
								string space = " ",
								bool spaceAfterKey = true,
								bool spaceBeforeValue = true,
								int linesBetweenSections = 1,
								string initialSectionName = "")
		{
			this.lineBreak = lineBreak;
			this.space = space;
			this.spaceAfterKey = spaceAfterKey;
			this.spaceBeforeValue = spaceBeforeValue;
			this.linesBetweenSections = linesBetweenSections;
			this.initialSectionName = initialSectionName;
		}
    }
}
