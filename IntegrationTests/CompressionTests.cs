#region

using System.IO;
using FileSystemLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictureHandlerLibrary;
using PictureHandlerLibrary.FileHandler;
using PictureHandlerLibrary.ImageCompression;
using UnitTests;

#endregion

namespace IntegrationTests {
	[TestClass]
	[DeploymentItem("TestData\\eula.1041.txt", "TestData")]
	[DeploymentItem("TestData\\IMG_6867.JPG", "TestData")]
	public class CompressionTests {
		private readonly IPictureDirectoryFactory _directoryFactory = new PictureDirectoryFactory();
		private readonly IFileHandlerFactory _fileHandlerFactory = new FileHandlerFactory();

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
		public void CompressingJpgResultsInSmallerFile() {
			// Arrange
			var fileSystem = new FileSystem();
			const string fileToCompress = TestConstants.ExistingJpgFullFileName;
			var compressor = new JpgCompressor();
			const string newFileName = "CompressedResult.jpg";

			// Act
			compressor.Compress(fileToCompress, newFileName);

			// Assert
			Assert.IsTrue(fileSystem.FileExists(fileToCompress));
			Assert.IsTrue(fileSystem.FileExists(newFileName));
			Assert.IsTrue(fileSystem.GetFileLength(fileToCompress) > 0);
			Assert.IsTrue(fileSystem.GetFileLength(newFileName) > 0);
			Assert.IsTrue(fileSystem.GetFileLength(fileToCompress) > fileSystem.GetFileLength(newFileName));
		}

		[TestMethod]
		public void DoNotCompressAlreadyCompressedFiles() {
			// Arrange
			//FileHandler fileHandler = new JpgFileHandler();

			var fileSystem = new FileSystem();
			fileSystem.DeleteDirectoryAndAllFiles(TestConstants.NewDirectory);
			fileSystem.DeleteDirectoryAndAllFiles(TestConstants.TempDirectory);
			IFileHandler fileHandler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingJpgFullFileName, fileSystem);
			IPictureDirectory tempDir = _directoryFactory.GetOrCreateDirectory(TestConstants.TempDirectory);
			IPictureDirectory newDir = _directoryFactory.GetOrCreateDirectory(TestConstants.NewDirectory);
			string originalFileName = fileHandler.FileName;
			string tempFileName = fileHandler.PerformRenameAndMove(tempDir);
			string compressedFileName = fileHandler.PerformCompressAndMove(newDir);
			bool caughtExpectedException = false;

			// Act
			try {
				string compressedFileName2 = fileHandler.PerformCompressAndMove(tempDir);
			} catch (IOException) {
				caughtExpectedException = true;
			}

			// Assert
			Assert.IsTrue(caughtExpectedException);
		}
	}
}