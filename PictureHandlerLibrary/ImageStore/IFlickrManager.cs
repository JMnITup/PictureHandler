namespace PictureHandlerLibrary.ImageStore {
	public interface IFlickrManager {
		string UploadPhoto(string imageName, string title, string description, string tags, bool isPublic, bool isFamily, bool isFriend);
		void DeletePhoto(string photoId);
	}
}