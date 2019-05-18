using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

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
            ServicePointManager.ServerCertificateValidationCallback = (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true;
            return new StreamReader(webRequest.GetResponse().GetResponseStream()).ReadToEnd().Replace("\"", "");
        }
    }
}
