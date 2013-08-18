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
	// TODO: In all classes, delete MockPictureDirectory and fix all tests to run with it
	[TestClass]
	public class PictureDirectoryTests {
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
		public void FactoryGetDirectoryWithExistantDirReturnsDirectory() {
			// Arrange
			_fileSystem.CreateDirectory(TestConstants.ExistingDirectory);

			// Act
			IPictureDirectory dir = _directoryFactory.GetDirectory(TestConstants.ExistingDirectory);

			// Arrange
			Assert.AreEqual(dir.Directory, TestConstants.ExistingDirectory);
		}

		[TestMethod]
		[ExpectedException(typeof (DirectoryNotFoundException))]
		public void FactoryGetDirectoryWithNonExistantDirThrowsException() {
			_directoryFactory.GetDirectory(TestConstants.NonExistingDirectory);
			Assert.Fail();
		}

		[TestMethod]
		[ExpectedException(typeof (ArgumentNullException))]
		public void FactoryGetDirectoryWithNullThrowsException() {
			_directoryFactory.GetDirectory(null);
			Assert.Fail();
		}

		[TestMethod]
		public void GetFileListFromPictureDirectoryUsingExistingDirectoryWithFiles_HasEntries() {
			// Arrange
			_fileSystem.CreateDirectory(TestConstants.ExistingDirectory);
			_fileSystem.AddFile(TestConstants.ExistingJpgFullFileName, 100, new DateTime());

			IPictureDirectory dir = _directoryFactory.GetDirectory(TestConstants.ExistingDirectory);

			// Act 
			//IPictureDirectory dir = _directoryFactory.GetDirectory(TestConstants.ExistingDirectory);
			string[] x = dir.GetFileList();

			// Assert
			Assert.IsTrue(x.Length > 0);
		}

		[TestMethod]
		[ExpectedException(typeof (DirectoryNotFoundException))]
		public void GetListOfInvalidDirectory_Fails() {
			// Arrange

			// Act
			IPictureDirectory dir = _directoryFactory.GetDirectory(TestConstants.NonExistingDirectory);
			dir.Directory = TestConstants.NonExistingDirectory;

			// Assert
			Assert.Fail();
		}

		[TestMethod]
		public void CheckingExistanceOfExistingDirectory_IsTrue() {
			// Arrange
			_fileSystem.CreateDirectory(TestConstants.ExistingDirectory);

			// Act
			Assert.IsTrue(_directoryFactory.Exists(TestConstants.ExistingDirectory));
		}

		[TestMethod]
		public void CheckingExistanceOfNonExistingDirectory_IsFalse() {
			// Arrange

			// Act
			Assert.IsFalse(_directoryFactory.Exists(TestConstants.NonExistingDirectory));
		}

		[TestMethod]
		public void GetOrCreateCreatesNewDirectory() {
			// Arrange
			string testDir = DeleteTempTestDir(_fileSystem);

			// Act
			IPictureDirectory newDir = _directoryFactory.GetOrCreateDirectory(testDir);

			// Assert
			Assert.AreEqual(newDir.Directory, testDir);
			Assert.IsTrue(_fileSystem.DirectoryExists(testDir));

			DeleteTempTestDir(_fileSystem);
		}

		private static string DeleteTempTestDir(IFileSystem fileSystem) {
			if (fileSystem.DirectoryExists(TestConstants.TempDirectory)) {
				fileSystem.DeleteDirectory(TestConstants.TempDirectory);
			}
			return TestConstants.TempDirectory;
		}

		[TestMethod]
		public void GetOrCreateGetsExistingDirectory() {
			// Arrange
			string testDir = CreateTempTestDir(_fileSystem);

			// Act

			IPictureDirectory newDir = _directoryFactory.GetOrCreateDirectory(testDir);

			// Assert
			Assert.AreEqual(newDir.Directory, testDir);
			Assert.IsTrue(_fileSystem.DirectoryExists(testDir));

			DeleteTempTestDir(_fileSystem);
		}

		[TestMethod]
		[ExpectedException(typeof (ArgumentNullException))]
		public void GetOrCreateNullDirectory_Fails() {
			// Arrange

			// Act
			IPictureDirectory newDir = _directoryFactory.GetOrCreateDirectory(null);

			// Assert
			Assert.Fail();
		}

		[TestMethod]
		//[DeploymentItem("TestData\\IMG_6867.JPG", "TestData")]
		//[DeploymentItem("TestData\\2013-05-29_19.39.18_COMPRESSED_6867.JPG", "TestData")]
		//[DeploymentItem("TestData\\2013-05-29_19.39.18_SUPERCOMPRESSED_6867.JPG", "TestData")]
		public void RenameOnFilesThatWouldOverwriteLeavesOldFiles() {
			// Arrange

			//_fileSystem.DeleteDirectoryAndAllFiles(TestConstants.ExistingDirectory);
			_fileSystem.CreateDirectory(TestConstants.ExistingDirectory);
			_fileSystem.AddFile(TestConstants.ExistingDirectory + "\\IMG_6867.JPG", 1000000, new DateTime(2013, 5, 29, 19, 39, 18));
			_fileSystem.AddFile(TestConstants.ExistingDirectory + "\\2013-05-29_19.39.18_COMPRESSED_6867.JPG", 400000, new DateTime(2013, 5, 29, 19, 39, 18));
			//_fileSystem.AddFile(TestConstants.ExistingDirectory + "\\eula.1041.txt", 1000000, new DateTime());
			_fileSystem.AddFile(TestConstants.ExistingDirectory + "\\2013-05-29_19.39.18_SUPERCOMPRESSED_6867.JPG", 100000, new DateTime(2013, 5, 29, 19, 39, 18));

			IPictureDirectory dir = _directoryFactory.GetDirectory(TestConstants.ExistingDirectory);
			IPictureDirectory newDir = _directoryFactory.GetOrCreateDirectory(TestConstants.NewDirectory);
			int oldFileCount = dir.GetFileList().Length;

			// Act
			dir.RenameAllFiles(newDir);

			// Assert
			Assert.AreEqual(oldFileCount, 3);
			Assert.AreEqual(dir.GetFileList().Length, 2);
			Assert.AreEqual(newDir.GetFileList().Length, 1);
		}

		[TestMethod]
		public void ProcessRenameOnAllFilesInDirectory() {
			// Arrange
			_fileSystem.AddFile(TestConstants.ExistingDirectory + "\\IMG_6867.JPG", 1000000, new DateTime(2013, 5, 29, 19, 39, 18));
			_fileSystem.AddFile(TestConstants.ExistingDirectory + "\\2013-05-29_19.39.18_COMPRESSED_6867.JPG", 400000, new DateTime(2013, 5, 29, 19, 39, 18));
			_fileSystem.AddFile(TestConstants.ExistingDirectory + "\\eula.1041.txt", 1000000, new DateTime());
			_fileSystem.AddFile(TestConstants.ExistingDirectory + "\\2013-05-29_19.39.18_SUPERCOMPRESSED_6867.JPG", 100000, new DateTime(2013, 5, 29, 19, 39, 18));

			IPictureDirectory dir = _directoryFactory.GetDirectory(TestConstants.ExistingDirectory);
			IPictureDirectory newDir = _directoryFactory.GetOrCreateDirectory(TestConstants.NewDirectory);
			//_fileSystem.DeleteFile();
			/*var oldFileList = dir.GetFileList();
			List<string> expectedNewFile = new List<string>();
			foreach (string oldFile in oldFileList) {
				try {
					expectedNewFile.Add(_fileFactory.GetFileHandler(oldFile).GetNewRenamedFileName());
				} catch (NotImplementedException) {}
			}*/

			//Act
			dir.RenameAllFiles(newDir);

			//Assert
			Assert.IsTrue(_fileSystem.FileExists(TestConstants.ExistingTxtFullFileName));
			Assert.IsTrue(!_fileSystem.FileExists(TestConstants.ExistingDirectory + "\\" + TestConstants.ExistingJpgDesiredNewRenamedShortFileName));
			Assert.IsTrue(!_fileSystem.FileExists(TestConstants.NewDirectory + "\\" + TestConstants.ExistingJpgShortFileName));
			Assert.IsTrue(_fileSystem.FileExists(TestConstants.NewDirectory + "\\" + TestConstants.ExistingJpgDesiredNewRenamedShortFileName));
		}

		[TestMethod]
		public void RenameAllFilesThrowsNoExceptionsOnExistingFile() {
			// Arrange
			_fileSystem.AddFile(TestConstants.ExistingDirectory + "\\IMG_6867.JPG", 1000000, new DateTime(2013, 5, 29, 19, 39, 18));
			_fileSystem.AddFile(TestConstants.ExistingDirectory + "\\2013-05-29_19.39.18_COMPRESSED_6867.JPG", 400000, new DateTime(2013, 5, 29, 19, 39, 18));
			_fileSystem.AddFile(TestConstants.ExistingDirectory + "\\eula.1041.txt", 1000000, new DateTime());
			_fileSystem.AddFile(TestConstants.ExistingDirectory + "\\2013-05-29_19.39.18_SUPERCOMPRESSED_6867.JPG", 100000, new DateTime(2013, 5, 29, 19, 39, 18));

			IPictureDirectory dir = _directoryFactory.GetDirectory(TestConstants.ExistingDirectory);
			IPictureDirectory newDir = _directoryFactory.GetOrCreateDirectory(TestConstants.NewDirectory);
			IFileHandler fileHandler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingJpgFullFileName, _fileSystem, new MockCompress(_fileSystem),
																																		new MockExifReader(_fileSystem));
			string newFullFileName = newDir.Directory + "\\" + fileHandler.GetNewRenamedFileName();
			_fileSystem.CopyFile(fileHandler.FileName, newFullFileName);

			// Act
			dir.RenameAllFiles(newDir);

			// Assert
			Assert.IsTrue(_fileSystem.FileExists(TestConstants.ExistingJpgFullFileName));
		}

		private string CreateTempTestDir(IFileSystem fileSystem) {
			if (!fileSystem.DirectoryExists(TestConstants.TempDirectory)) {
				fileSystem.CreateDirectory(TestConstants.TempDirectory);
			}
			return TestConstants.TempDirectory;
		}
	}
}