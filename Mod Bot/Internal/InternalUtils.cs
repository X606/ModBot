using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalModBot
{
	/// <summary>
	/// 
	/// </summary>
	public class InternalUtils
	{
		/// <summary> </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string GetSubdomain(string path)
		{
			string[] subDomainsArray = path.Split(new char[] { '/' });

			List<string> subDomainsList = new List<string>(subDomainsArray);
			subDomainsList.RemoveAt(subDomainsList.Count - 1);

			return subDomainsList.Join("/") + "/";
		}
	}
}
