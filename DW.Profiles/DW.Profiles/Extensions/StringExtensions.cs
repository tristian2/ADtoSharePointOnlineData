using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace DW.Profiles.Extensions
{
	public static class StringExtensions
	{
		public static SecureString ToSecureString(this string Source)
		{
			if (string.IsNullOrWhiteSpace(Source))
				return null;
			else
			{
				SecureString Result = new SecureString();

				foreach (char c in Source.ToCharArray())
					Result.AppendChar(c);

				return Result;
			}
		}
	}
}
