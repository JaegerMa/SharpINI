using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpINI
{
    public class ParseOptions
    {
		public static ParseOptions Default => new ParseOptions();


		public char[] lineStartCommentChars;
		public char[] spaceChars;
		public string[] lineBreaks;
		public bool trimSpaceBeforeValue;
		public bool trimSpaceAfterValue;
		public MultiKeyMode multiKeyMode;
		public MultiSectionMode multiSectionMode;
		public string initialSectionName;


		public ParseOptions(char[] lineStartCommentChars = null,
							char[] spaceChars = null,
							string[] lineBreaks = null,
							bool trimSpaceBeforeValue = true,
							bool trimSpaceAfterValue = false,
							MultiKeyMode multiKeyMode = MultiKeyMode.DISALLOW,
							MultiSectionMode multiSectionMode = MultiSectionMode.DISALLOW,
							string initialSectionName = "")
		{
			this.lineStartCommentChars = lineStartCommentChars ?? new char[] { ';', '#' };
			this.spaceChars = spaceChars ?? new char[] { ' ', '\t', '\xA0' /* NBSP */ };
			this.lineBreaks = lineBreaks ?? new string[] { "\r\n", "\n" };
			this.trimSpaceBeforeValue = trimSpaceBeforeValue;
			this.trimSpaceAfterValue = trimSpaceAfterValue;
			this.multiKeyMode = multiKeyMode;
			this.multiSectionMode = multiSectionMode;
			this.initialSectionName = initialSectionName;
		}

		public RenderOptions ToRenderOptions()
		{
			return new RenderOptions(lineBreak: this.lineBreaks.FirstOrDefault() ?? "\n",
										space: this.spaceChars.Select((c) => c.ToString()).FirstOrDefault() ?? " ",
										spaceAfterKey: this.trimSpaceBeforeValue,
										spaceBeforeValue: this.trimSpaceBeforeValue,
										initialSectionName: this.initialSectionName);
		}
	}

	public enum MultiKeyMode
	{
		IGNORE,
		OVERRIDE,
		DISALLOW,
	}
	public enum MultiSectionMode
	{
		IGNORE,
		REPLACE,
		MERGE,
		DISALLOW,
	}
}
