using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SharpINI
{
#if SHARE_INTERNAL_METHODS
		public
#endif
	class StringView
	{
		public string realString;
		public int offset;
		public ParseOptions options;

		public int Length => Math.Max(this.realString.Length - offset, 0);
		public char this[int idx] => this.realString[this.offset + idx];

		public StringView(string realString, int offset = 0, ParseOptions options = null)
		{
			this.realString = realString;
			this.offset = offset;
			this.options = options ?? ParseOptions.Default;
		}

		public void TrimStart()
		{
			while(this.Length != 0 && char.IsWhiteSpace(this[0]))
				this.Move(1);
		}
		public void TrimWhitespace()
		{
			while(this.Length != 0 && this.options.spaceChars.Contains(this[0]))
				this.Move(1);
		}

		public void Move(int offset)
		{
			this.offset += offset;
		}

		public StringView SubString(int offset)
		{
			return new StringView(this.realString, this.offset + offset);
		}
		public string SubString(int offset, int length)
		{
			return this.realString.Substring(this.offset + offset, length);
		}

		public int IndexOf(char c)
		{
			return this.realString.IndexOf(c, this.offset);
		}

		public override string ToString()
		{
			return this.realString.Substring(this.offset);
		}
	}
}
