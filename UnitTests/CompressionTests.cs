#region

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockFileSystemLibrary;
using PictureHandlerLibrary;
using PictureHandlerLibrary.FileHandler;
using UnitTests.MockClasses;

#endregion

namespace UnitTests {
	[TestClass]
	public class CompressionTests {
		private IPictureDirectoryFactory _directoryFactory = new PictureDirectoryFactory();
		private IFileHandlerFactory _fileHandlerFactory = new FileHandlerFactory();
		private MockFileSystem _fileSystem;

		[TestInitialize]
		public void MyTestInitialize() {
			_fileSystem = new MockFileSystem();
			var mockExifReader = new MockExifReader(_fileSystem);
			_directoryFactory = new PictureDirectoryFactory(_fileSystem, mockExifReader);
			_fileHandlerFactory = new FileHandlerFactory(_fileSystem);
		}

		[TestMethod]
		public void PerformCompressAndMoveRemovesOldFileAndCreatesNew() {
			// Arrange
			var targetDirectory = new MockPictureDirectory {Directory = "\\mockdir"};

			(_fileSystem).AddFile("\\mockdir\\test_RENAMED_2353.JPG", 100000);

			var mockCompressor = new MockCompress(_fileSystem);
			var fileHandler = new JpgFileHandler("\\mockdir\\test_RENAMED_2353.JPG", fileSystem: _fileSystem, compressor: mockCompressor);

			// Act
			fileHandler.PerformCompressAndMove(targetDirectory);

			// Assert
			Assert.IsTrue(!_fileSystem.FileExists("\\mockdir\\test_RENAMED_2353.JPG"));
			Assert.IsTrue(_fileSystem.FileExists("\\mockdir\\test_COMPRESSED_2353.JPG"));
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
			_fileSystem.AddFile(TestConstants.ExistingJpgFullFileName, 100000, new DateTime(2013, 2, 2));

			IFileHandler fileHandler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingJpgFullFileName, _fileSystem, new MockCompress(_fileSystem),
																																		new MockExifReader(_fileSystem));
			//IPictureDirectory tempDir = _directoryFactory.GetOrCreateDirectory(TestConstants.TempDirectory);
			IPictureDirectory tempDir = _directoryFactory.GetOrCreateDirectory(TestConstants.TempDirectory);
			//IPictureDirectory newDir = _directoryFactory.GetOrCreateDirectory(TestConstants.NewDirectory);
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

		[TestMethod]
		[ExpectedException(typeof (FileLoadException))]
		public void DoNotCompressNonRenamedOriginalFile() {
			// Arrange
			_fileSystem.AddFile(TestConstants.ExistingJpgFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));

			IFileHandler fileHandler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingJpgFullFileName, _fileSystem, new MockCompress(_fileSystem),
																																		new MockExifReader(_fileSystem));
			var newDir = new MockPictureDirectory {Directory = TestConstants.NewDirectory};


			// Act
			fileHandler.PerformCompressAndMove(newDir);

			// Assert
			Assert.Fail();
		}

		[TestMethod]
		public void AttemptToOverwriteExistingFileInCompressedLocationThrowsIoExceptionAndResultsInNoChanges() {
			// Arrange
			_fileSystem.AddFile(TestConstants.ExistingJpgFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));
			const string movedFileName = TestConstants.ExistingDirectory + "\\0000-00-00_00.00.00_RENAMED_6867.JPG";
			_fileSystem.MoveFile(TestConstants.ExistingJpgFullFileName, movedFileName);
			IFileHandler fileHandler = _fileHandlerFactory.GetFileHandler(movedFileName, _fileSystem, new MockCompress(_fileSystem), new MockExifReader(_fileSystem));
			var newDir = new MockPictureDirectory {Directory = TestConstants.NewDirectory};

			string expectedNewFilename = fileHandler.GetNewCompressedFileName();
			//_fileSystem.CopyFile(TestConstants.ExistingTxtFullFileName, TestConstants.NewDirectory + "\\" + expectedNewFilename);
			_fileSystem.AddFile(TestConstants.NewDirectory + "\\" + expectedNewFilename, 10453, null);
			long blockingFileSize = _fileSystem.GetFileLength(TestConstants.NewDirectory + "\\" + expectedNewFilename);
			bool expectedExceptionCaught = false;

			// Act
			try {
				string newFileName = fileHandler.PerformCompressAndMove(newDir);
			} catch (IOException) {
				expectedExceptionCaught = true;
			}
			long newFileSize = _fileSystem.GetFileLength(TestConstants.NewDirectory + "\\" + expectedNewFilename);

			// Assert
			Assert.IsTrue(expectedExceptionCaught);
			Assert.AreEqual(blockingFileSize, newFileSize);
			Assert.AreEqual(fileHandler.FileName, movedFileName);
			Assert.IsTrue(_fileSystem.FileExists(movedFileName));
		}
	}
}