#region

using System;
using System.IO;
using FileSystemLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockFileSystemLibrary;
using PictureHandlerLibrary;
using PictureHandlerLibrary.FileHandler;
using UnitTests.MockClasses;

#endregion

namespace UnitTests {
	[TestClass]
	/*[DeploymentItem("TestData\\eula.1041.txt", "TestData")]
	[DeploymentItem("TestData\\IMG_6867.JPG", "TestData")]*/
	public class CompressionTests {
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

		[TestMethod]
		public void PerformCompressAndMoveRemovesOldFileAndCreatesNew() {
			// Arrange
			var targetDirectory = new MockPictureDirectory();
			targetDirectory.Directory = "\\mockdir";

			IFileSystem fileSystem = new MockFileSystem();
			((MockFileSystem) fileSystem).AddFile("\\mockdir\\test_RENAMED_2353.JPG", 100000);

			var mockCompressor = new MockCompress(fileSystem);
			var fileHandler = new JpgFileHandler("\\mockdir\\test_RENAMED_2353.JPG", fileSystem: fileSystem, compressor: mockCompressor);

			// Act
			fileHandler.PerformCompressAndMove(targetDirectory);

			// Assert
			Assert.IsTrue(!fileSystem.FileExists("\\mockdir\\test_RENAMED_2353.JPG"));
			Assert.IsTrue(fileSystem.FileExists("\\mockdir\\test_COMPRESSED_2353.JPG"));
		}

/*		[TestMethod]
		public void CompressingJpgMovesFile() {
			// Arrange
			IFileSystem fileSystem = new FileSystem();
			var mockCompressor = MockRepository.GenerateMock<ICompress>();

			fileSystem.DeleteDirectoryAndAllFiles(TestConstants.NewDirectory);
			fileSystem.DeleteDirectoryAndAllFiles(TestConstants.TempDirectory);
			IFileHandler fileHandler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingJpgFullFileName, compressor: mockCompressor);
			IPictureDirectory tempDir = _directoryFactory.GetOrCreateDirectory(TestConstants.TempDirectory);
			IPictureDirectory newDir = _directoryFactory.GetOrCreateDirectory(TestConstants.NewDirectory);
			string originalFileName = fileHandler.FileName;
			string tempFileName = fileHandler.PerformRenameAndMove(tempDir);

			// Act
			string compressedFileName = fileHandler.PerformCompressAndMove(newDir);

			// Assert
			Assert.IsTrue(fileSystem.FileExists(newDir + "\\" + TestConstants.ExistingJpgDesiredNewCompressedShortFileName));
			Assert.IsTrue(fileSystem.FileExists(compressedFileName));
			Assert.IsTrue(!fileSystem.FileExists(TestConstants.ExistingJpgFullFileName));
			Assert.IsTrue(!fileSystem.FileExists(originalFileName));
			Assert.IsTrue(!fileSystem.FileExists(tempFileName));
		}*/

		[TestMethod]
		public void DoNotCompressAlreadyCompressedFiles() {
			// Arrange
			var fileSystem = new MockFileSystem();
			fileSystem.AddFile(TestConstants.ExistingJpgFullFileName, 100000, new DateTime(2013, 2, 2));

			IFileHandler fileHandler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingJpgFullFileName, fileSystem, new MockCompress(fileSystem),
																																		new MockExifReader(fileSystem));
			//IPictureDirectory tempDir = _directoryFactory.GetOrCreateDirectory(TestConstants.TempDirectory);
			var tempDir = new MockPictureDirectory {Directory = TestConstants.TempDirectory};
			//IPictureDirectory newDir = _directoryFactory.GetOrCreateDirectory(TestConstants.NewDirectory);
			var newDir = new MockPictureDirectory {Directory = TestConstants.NewDirectory};
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

		[TestMethod]
		[ExpectedException(typeof (FileLoadException))]
		public void DoNotCompressNonRenamedOriginalFile() {
			// Arrange
			var fileSystem = new MockFileSystem();
			fileSystem.AddFile(TestConstants.ExistingJpgFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));

			IFileHandler fileHandler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingJpgFullFileName, fileSystem, new MockCompress(fileSystem),
																																		new MockExifReader(fileSystem));
			var newDir = new MockPictureDirectory {Directory = TestConstants.NewDirectory};


			// Act
			fileHandler.PerformCompressAndMove(newDir);

			// Assert
			Assert.Fail();
		}

		[TestMethod]
		public void AttemptToOverwriteExistingFileInCompressedLocationThrowsIoExceptionAndResultsInNoChanges() {
			// Arrange
			var fileSystem = new MockFileSystem();
			fileSystem.AddFile(TestConstants.ExistingJpgFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));
			string movedFileName = TestConstants.ExistingDirectory + "\\0000-00-00_00.00.00_RENAMED_6867.JPG";
			fileSystem.MoveFile(TestConstants.ExistingJpgFullFileName, movedFileName);
			IFileHandler fileHandler = _fileHandlerFactory.GetFileHandler(movedFileName, fileSystem, new MockCompress(fileSystem), new MockExifReader(fileSystem));
			var newDir = new MockPictureDirectory {Directory = TestConstants.NewDirectory};

			string expectedNewFilename = fileHandler.GetNewCompressedFileName();
			//fileSystem.CopyFile(TestConstants.ExistingTxtFullFileName, TestConstants.NewDirectory + "\\" + expectedNewFilename);
			fileSystem.AddFile(TestConstants.NewDirectory + "\\" + expectedNewFilename, 10453, null);
			long blockingFileSize = fileSystem.GetFileLength(TestConstants.NewDirectory + "\\" + expectedNewFilename);
			bool expectedExceptionCaught = false;

			// Act
			try {
				string newFileName = fileHandler.PerformCompressAndMove(newDir);
			} catch (IOException) {
				expectedExceptionCaught = true;
			}
			long newFileSize = fileSystem.GetFileLength(TestConstants.NewDirectory + "\\" + expectedNewFilename);

			// Assert
			Assert.IsTrue(expectedExceptionCaught);
			Assert.AreEqual(blockingFileSize, newFileSize);
			Assert.AreEqual(fileHandler.FileName, movedFileName);
			Assert.IsTrue(fileSystem.FileExists(movedFileName));
		}
	}
}