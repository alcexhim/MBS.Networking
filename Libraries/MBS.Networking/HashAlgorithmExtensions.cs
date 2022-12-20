using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MBS.Networking
{
	public static class HashAlgorithmExtensions
	{
		public static string HashString(this System.Security.Cryptography.HashAlgorithm algo, string value, System.Text.Encoding encoding = null)
		{
			if (encoding == null) encoding = System.Text.Encoding.Default;

			algo.Initialize();
			byte[] hashBytes = algo.ComputeHash(encoding.GetBytes(value));

			StringBuilder sbHash = new StringBuilder();
			for (int i = 0; i < hashBytes.Length; i++)
			{
				sbHash.Append(hashBytes[i].ToString("x").ToLower().PadLeft(2, '0'));
			}
			return sbHash.ToString();
		}
	}
}
