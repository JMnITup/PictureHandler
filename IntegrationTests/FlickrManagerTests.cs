#region

using FileSystemLibrary;
using FlickrNet;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictureHandlerLibrary.ImageStore;
using UnitTests;

#endregion

namespace IntegrationTests {
	[TestClass]
	[DeploymentItem("TestData\\eula.1041.txt", "TestData")]
	[DeploymentItem("TestData\\IMG_6867.JPG", "TestData")]
	public class FlickrManagerIntegrationTests {
		// Use TestInitialize to run code before running each test 
		[TestInitialize]
		public void MyTestInitialize() {
			var fs = new FileSystem();
			fs.DeleteDirectoryAndAllFiles(TestConstants.ExistingDirectory);
			fs.DeleteDirectoryAndAllFiles(TestConstants.NewDirectory);
			fs.DeleteDirectoryAndAllFiles(TestConstants.TempDirectory);
			fs.DeleteDirectoryAndAllFiles(TestConstants.NonExistingDirectory);
			fs.CreateDirectory(TestConstants.ExistingDirectory);
			fs.CopyFiles("TestData", TestConstants.ExistingDirectory);
		}

		[ClassCleanup]
		public static void MyClassCleanup() {
			var fs = new FileSystem();
			fs.DeleteDirectoryAndAllFiles(TestConstants.ExistingDirectory);
			fs.DeleteDirectoryAndAllFiles(TestConstants.NewDirectory);
			fs.DeleteDirectoryAndAllFiles(TestConstants.TempDirectory);
			fs.DeleteDirectoryAndAllFiles(TestConstants.NonExistingDirectory);
		}

		[TestMethod]
		public void GetInstanceOfFlickrManager() {
			// Arrange
			// Act
			Flickr flickr = FlickrManager.GetInstance();

			// Assert
			Assert.IsNotNull(flickr);
		}

		[TestMethod]
		public void GetAuthedInstanceOfFlickrManager() {
			// Arrange
			// Act
			Flickr flickr = FlickrManager.GetAuthInstance();
			//FlickrManager.BuildOAuthToken();


			// Assert
			Assert.IsNotNull(flickr);
			Assert.IsNotNull(flickr.OAuthAccessToken);
		}

		[TestMethod]
		[Ignore]
		[DeploymentItem("TestData\\2013-05-29_19.39.18_SUPERCOMPRESSED_6867.JPG", "TestData")]
		public void UploadPrivateImageToFlickr() {
			// Arrange
			Flickr flickr = FlickrManager.GetAuthInstance();
			string imageName = TestConstants.ExistingDirectory + "\\2013-05-29_19.39.18_SUPERCOMPRESSED_6867.JPG";

			// Act
			string result = flickr.UploadPicture(imageName, "test", "testing", "", false, false, false);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsTrue(result.Length > 1);

			flickr.PhotosDelete(result);
		}

		[TestMethod]
		[Ignore]
		[DeploymentItem("TestData\\2013-05-29_19.39.18_SUPERCOMPRESSED_6867.JPG", "TestData")]
		public void UploadImageReturnsString() {
			// Arrange
			IFlickrManager flickr = new FlickrManager();
			string imageName = TestConstants.ExistingDirectory + "\\2013-05-29_19.39.18_SUPERCOMPRESSED_6867.JPG";

			// Act
			string result = flickr.UploadPhoto(imageName, "test", "testing", "", false, false, false);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsTrue(result.Length > 1);

			flickr.DeletePhoto(result);
		}
	}
}