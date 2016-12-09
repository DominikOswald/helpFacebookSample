using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;

namespace FacebookPostOnPage
{
    internal class Program
    {
        private const string filename = "facebookaccess.txt";

        private static void Main(string[] args)
        {
            // Create Facebook app for the appId, appSecret https://developers.facebook.com/apps
            var facebookApi = new FacebookApi("XXXX", "YYYYYY");
            var isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User |
                                                        IsolatedStorageScope.Assembly, null, null);
            var curUserToken = "";
            try
            {
                curUserToken = ReadFromFile(isoStore);
            }
            catch (Exception)
            {
                // DO something
                //throw;
            }

            // get default user token for your project https://developers.facebook.com/tools/accesstoken/
            if (string.IsNullOrEmpty(curUserToken))
                curUserToken =
                    "ZZZZd65XML7cBAAOrobEwDtHCSjyINIz1sglWNV75KGDBTOGsnoJKGwQWpn4UBUGRk4EYNAZBc92qMFxIeliycZCFydXRwywYMrCei6CUkJZAVRfRT3yXEZAN6D2SUaYFe0qsK6l1vuWm0eWZAQjWr1OjDYSKVJUCNYxX3QOJVhwZDZD";

            try
            {
                // get long-lived tokens (60days)
                var newUserAccessToken = facebookApi.RefreshUserAccessToken(curUserToken);

                // Save the new token 
                WriteToFile(isoStore, newUserAccessToken);

                // get page access token
                var pageAccess = facebookApi.GetPageAccessToken(newUserAccessToken, "dmmosoftware");
                if (string.IsNullOrEmpty(pageAccess))
                {
                    Console.Write("ERROR: No user access token!");
                    // TODO - ERROR handling
                }
                else
                {
                    var message = "Testnachricht: " + RandomString(20);
                    facebookApi.PostToFacebook(pageAccess, "dmmosoftware", message);

                    var id = facebookApi.ResponseJsonObject["id"];
                    Console.WriteLine("PostToFacebook new id: " + id);

                    facebookApi.DeleteFacebookPost(pageAccess, id.ToString());

                    var success = facebookApi.ResponseJsonObject["success"];
                    Console.WriteLine("Delete post: " + success);
                }
            }
            catch (Exception exception)
            {
                Trace.Write(exception);
                throw;
            }
            Console.ReadKey();
        }

        #region helper

        /// <summary>
        ///     Writes to file.
        /// </summary>
        /// <param name="isoStore">The iso store.</param>
        /// <param name="curUserAccess">The current user access.</param>
        private static void WriteToFile(IsolatedStorageFile isoStore, string curUserAccess)
        {
            StreamWriter writer = null;

            writer = new StreamWriter(new IsolatedStorageFileStream(
                filename, FileMode.Create, isoStore));

            writer.WriteLine(curUserAccess);

            writer.Close();
        }

        /// <summary>
        ///     Reads from file.
        /// </summary>
        /// <param name="isoStore">The iso store.</param>
        /// <returns></returns>
        private static string ReadFromFile(IsolatedStorageFile isoStore)
        {
            var reader = new StreamReader(new IsolatedStorageFileStream(
                filename, FileMode.Open, isoStore));

            var sb = reader.ReadLine();
            reader.Close();

            return sb;
        }

        /// <summary>
        ///     Randoms the string.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHDTZHDIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        #endregion
    }
}