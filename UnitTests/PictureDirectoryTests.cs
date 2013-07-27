// /*
// JamesM
// 2013 06 13 9:42 PM
// 2013 06 14 12:09 PM
// PictureDirectoryTests.cs
// UnitTests
// PictureHandler
// */

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
	[DeploymentItem("TestData\\eula.1041.txt", "TestData")]
	[DeploymentItem("TestData\\IMG_6867.JPG", "TestData")]
	public class PictureDirectoryTests {
		private readonly IPictureDirectoryFactory _directoryFactory = new PictureDirectoryFactory();
		private readonly IFileHandlerFactory _fileFactory = new FileHandlerFactory();

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
		public void FactoryGetDirectoryWithExistantDirReturnsDirectory() {
			IPictureDirectory dir = _directoryFactory.GetDirectory(TestConstants.ExistingDirectory);
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
			var fileSystem = new MockFileSystem();

			// Act 
			//IPictureDirectory dir = _directoryFactory.GetDirectory(TestConstants.ExistingDirectory);
			var dir = new MockPictureDirectory {Directory = TestConstants.ExistingDirectory};

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
			var fileSystem = new FileSystem();
			string testDir = DeleteTempTestDir(fileSystem);

			// Act
			IPictureDirectory newDir = _directoryFactory.GetOrCreateDirectory(testDir);

			// Assert
			Assert.AreEqual(newDir.Directory, testDir);
			Assert.IsTrue(fileSystem.DirectoryExists(testDir));

			DeleteTempTestDir(fileSystem);
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
			var fileSystem = new FileSystem();
			string testDir = CreateTempTestDir(fileSystem);

			// Act

			IPictureDirectory newDir = _directoryFactory.GetOrCreateDirectory(testDir);

			// Assert
			Assert.AreEqual(newDir.Directory, testDir);
			Assert.IsTrue(fileSystem.DirectoryExists(testDir));

			DeleteTempTestDir(fileSystem);
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
		[DeploymentItem("TestData\\IMG_6867.JPG", "TestData")]
		[DeploymentItem("TestData\\2013-05-29_19.39.18_COMPRESSED_6867.JPG", "TestData")]
		[DeploymentItem("TestData\\2013-05-29_19.39.18_SUPERCOMPRESSED_6867.JPG", "TestData")]
		public void RenameOnFilesThatWouldOverwriteLeavesOldFiles() {
			// Arrange
			var fileSystem = new FileSystem();

			fileSystem.DeleteDirectoryAndAllFiles(TestConstants.ExistingDirectory);
			fileSystem.CreateDirectory(TestConstants.ExistingDirectory);
			fileSystem.CopyFile("TestData\\IMG_6867.JPG", TestConstants.ExistingDirectory + "\\IMG_6867.JPG");
			fileSystem.CopyFile("TestData\\2013-05-29_19.39.18_COMPRESSED_6867.JPG", TestConstants.ExistingDirectory + "\\2013-05-29_19.39.18_COMPRESSED_6867.JPG");
			fileSystem.CopyFile("TestData\\2013-05-29_19.39.18_SUPERCOMPRESSED_6867.JPG",
													TestConstants.ExistingDirectory + "\\2013-05-29_19.39.18_SUPERCOMPRESSED_6867.JPG");

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
			var fileSystem = new FileSystem();
			IPictureDirectory dir = _directoryFactory.GetDirectory(TestConstants.ExistingDirectory);
			IPictureDirectory newDir = _directoryFactory.GetOrCreateDirectory(TestConstants.NewDirectory);
			//fileSystem.DeleteFile();
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
			Assert.IsTrue(fileSystem.FileExists(TestConstants.ExistingTxtFullFileName));
			Assert.IsTrue(!fileSystem.FileExists(TestConstants.ExistingDirectory + "\\" + TestConstants.ExistingJpgDesiredNewRenamedShortFileName));
			Assert.IsTrue(!fileSystem.FileExists(TestConstants.NewDirectory + "\\" + TestConstants.ExistingJpgShortFileName));
			Assert.IsTrue(fileSystem.FileExists(TestConstants.NewDirectory + "\\" + TestConstants.ExistingJpgDesiredNewRenamedShortFileName));
		}

		[TestMethod]
		public void RenameAllFilesThrowsNoExceptionsOnExistingFile() {
			// Arrange
			var fileSystem = new FileSystem();
			IPictureDirectory dir = _directoryFactory.GetDirectory(TestConstants.ExistingDirectory);
			IPictureDirectory newDir = _directoryFactory.GetOrCreateDirectory(TestConstants.NewDirectory);
			IFileHandler fileHandler = _fileFactory.GetFileHandler(TestConstants.ExistingJpgFullFileName);
			string newFullFileName = newDir.Directory + "\\" + fileHandler.GetNewRenamedFileName();
			fileSystem.CopyFile(fileHandler.FileName, newFullFileName);

			// Act
			dir.RenameAllFiles(newDir);

			// Assert
			Assert.IsTrue(fileSystem.FileExists(TestConstants.ExistingJpgFullFileName));
		}

		private string CreateTempTestDir(IFileSystem fileSystem) {
			if (!fileSystem.DirectoryExists(TestConstants.TempDirectory)) {
				fileSystem.CreateDirectory(TestConstants.TempDirectory);
			}
			return TestConstants.TempDirectory;
		}
	}
}