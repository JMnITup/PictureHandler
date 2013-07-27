#region

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictureHandlerLibrary;
using PictureHandlerLibrary.FileHandler;

#endregion

namespace UnitTests {
	[TestClass]
	//[DeploymentItem("TestData\\eula.1041.txt", "TestData")]
	//[DeploymentItem("TestData\\IMG_6867.JPG", "TestData")]
	public class FileHandlerUploaderTests {
		private readonly IPictureDirectoryFactory _directoryFactory = new PictureDirectoryFactory();
		private readonly IFileHandlerFactory _fileHandlerFactory = new FileHandlerFactory();
		/*
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
		*/
	}
}