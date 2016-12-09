using System.Collections.Generic;
using Facebook;
using Newtonsoft.Json.Linq;

namespace FacebookPostOnPage
{
    public class FacebookApi
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FacebookApi" /> class.
        /// </summary>
        /// <param name="appId">The application identifier.</param>
        /// <param name="appSecret">The application secret.</param>
        public FacebookApi(string appId, string appSecret)
        {
            ResponseJsonObject = new JsonObject();
            AppId = appId;
            AppSecret = appSecret;
        }

        public string AppId { get; }

        public string AppSecret { get; }

        public JsonObject ResponseJsonObject { get; set; }


        /// <summary>
        ///     Gets the page access token.
        /// </summary>
        /// <param name="userAccessToken">The user access token.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <returns></returns>
        public string GetPageAccessToken(string userAccessToken, string pageName)
        {
            var fbClient = new FacebookClient();
            fbClient.AppId = AppId;
            fbClient.AppSecret = AppSecret;
            fbClient.AccessToken = userAccessToken;
            var fbParams = new Dictionary<string, object>();
            var publishedResponse = fbClient.Get("/me/accounts", fbParams) as JsonObject;
            var data = JArray.Parse(publishedResponse["data"].ToString());

            foreach (var account in data)
                if (account["name"].ToString().ToLower().Equals(pageName))
                    return account["access_token"].ToString();

            return string.Empty;
        }

        /// <summary>
        ///     Refreshes the user access token.
        /// </summary>
        /// <param name="currentAccessToken">The current access token.</param>
        /// <returns></returns>
        public string RefreshUserAccessToken(string currentAccessToken)
        {
            var fbClient = new FacebookClient();
            var fbParams = new Dictionary<string, object>();
            fbParams["client_id"] = AppId; // app ID https://developers.facebook.com/apps/947691342016439/dashboard/
            fbParams["grant_type"] = "fb_exchange_token";
            fbParams["client_secret"] = AppSecret;
            fbParams["fb_exchange_token"] = currentAccessToken;
            var publishedResponse = fbClient.Get("/oauth/access_token", fbParams) as JsonObject;
            return publishedResponse["access_token"].ToString();
        }

        /// <summary>
        /// Posts to facebook.
        /// </summary>
        /// <param name="pageAccessToken">The page access token.</param>
        /// <param name="pageid">The pageid.</param>
        /// <param name="facebookParameters">The facebook parameters.</param>
        public void PostToFacebook(string pageAccessToken, string pageid, Dictionary<string, object> facebookParameters)
        {
            var fbClient = new FacebookClient(pageAccessToken);
            fbClient.AppId = AppId;
            fbClient.AppSecret = AppSecret;
            // https://developers.facebook.com/docs/graph-api/reference/v2.6/post
            //Dictionary<string, object> fbParams = new Dictionary<string, object>();
            //fbParams["message"] = message;
            //fbParams["link"] =
            //fbParams["picture"] =
            //fbParams["name"] =
            //fbParams["caption"] =
            //fbParams["description"] =
            var publishedResponse = fbClient.Post($"/{pageid}/feed", facebookParameters);
            if (publishedResponse != null)
                if (typeof(JsonObject) == publishedResponse.GetType())
                {
                    var respJsonObject = (JsonObject) publishedResponse;
                    ResponseJsonObject = respJsonObject;
                }
        }

        /// <summary>
        /// Deletes the facebook post.
        /// </summary>
        /// <param name="pageAccessToken">The page access token.</param>
        /// <param name="pageid">The pageid.</param>
        /// <param name="facebookParameters">The facebook parameters.</param>
        public void DeleteFacebookPost(string pageAccessToken, string pageid,
            Dictionary<string, object> facebookParameters)
        {
            var fbClient = new FacebookClient(pageAccessToken);
            fbClient.AppId = AppId;
            fbClient.AppSecret = AppSecret;

            var publishedResponse = fbClient.Delete($"/{pageid}", facebookParameters);
            if (publishedResponse != null)
                if (typeof(JsonObject) == publishedResponse.GetType())
                {
                    var respJsonObject = (JsonObject) publishedResponse;
                    ResponseJsonObject = respJsonObject;
                }
        }

        /// <summary>
        /// Posts to facebook.
        /// </summary>
        /// <param name="pageAccessToken">The page access token.</param>
        /// <param name="pageid">The pageid.</param>
        /// <param name="message">The message.</param>
        public void PostToFacebook(string pageAccessToken, string pageid, string message)
        {
            var fbParamters = new Dictionary<string, object>();
            fbParamters["message"] = message;
            PostToFacebook(pageAccessToken, pageid, fbParamters);
        }

        /// <summary>
        /// Deletes the facebook post.
        /// </summary>
        /// <param name="pageAccessToken">The page access token.</param>
        /// <param name="pageid">The pageid.</param>
        public void DeleteFacebookPost(string pageAccessToken, string pageid)
        {
            var fbParamters = new Dictionary<string, object>();
            DeleteFacebookPost(pageAccessToken, pageid, fbParamters);
        }
    }
}