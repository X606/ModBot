using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace ModLibrary
{
    public static class FirebaseAccessor
    {
        /// <summary>
        /// Gets a value from firebase (Do not use in main thread) (Removes all '"' characters)
        /// </summary>
        /// <param name="URLToRead"></param>
        /// <returns></returns>
        public static string ReadFromFirebaseURL(string URLToRead)
        {
            WebRequest webRequest = WebRequest.Create(URLToRead);
            webRequest.Method = "GET";
            ServicePointManager.ServerCertificateValidationCallback = ((object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true);
            return new StreamReader(webRequest.GetResponse().GetResponseStream()).ReadToEnd().Replace("\"", "");
        }
    }
}
