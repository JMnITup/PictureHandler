#region

using System.Diagnostics;
using FlickrNet;
using PictureHandlerLibrary.Properties;

#endregion

namespace PictureHandlerLibrary.ImageStore {
    public class FlickrManager : IFlickrManager {
        public const string ApiKey = "e26b93d5a34d1251df61a7f2ab0df682";
        public const string SharedSecret = "34a360710547f2cb";
        private static Flickr _authInstance;

        public FlickrManager() {
            _authInstance = GetAuthInstance();
        }

        public static OAuthAccessToken OAuthToken {
            get { return Settings.Default.OAuthToken; }
            set {
                Settings.Default.OAuthToken = value;
                Settings.Default.Save();
            }
        }

        public string UploadPhoto(string imageName, string title, string description, string tags, bool isPublic, bool isFamily, bool isFriend) {
            return _authInstance.UploadPicture(imageName, title, description, tags, isPublic, isFamily, isFriend);
        }

        #region Implementation of IFlickrManager

        public void DeletePhoto(string photoId) {
            _authInstance.PhotosDelete(photoId);
        }

        #endregion

        public static Flickr GetInstance() {
            return new Flickr(ApiKey, SharedSecret);
        }

        public static Flickr GetAuthInstance() {
            var f = new Flickr(ApiKey, SharedSecret);
            if (OAuthToken == null) {
                BuildOAuthToken();
            }
            f.OAuthAccessToken = OAuthToken.Token;
            f.OAuthAccessTokenSecret = OAuthToken.TokenSecret;
            return f;
        }

        public static void BuildOAuthToken() {
            Flickr f = GetInstance();
            OAuthRequestToken requestToken = f.OAuthGetRequestToken("oob");

            string url = f.OAuthCalculateAuthorizationUrl(requestToken.Token, AuthLevel.Delete);

            Process.Start(url);
            // TODO: This is not interactive at the moment, must be handled via debug session
            // Put a debug breakpoint on the process.start, run tests in debug, and modify code in-place with browser token during execution
            const string verificationNumber = "974-138-162";

            OAuthAccessToken accessToken = f.OAuthGetAccessToken(requestToken, verificationNumber);
            OAuthToken = accessToken;

            //Step2GroupBox.Enabled = true;
        }
    }
}