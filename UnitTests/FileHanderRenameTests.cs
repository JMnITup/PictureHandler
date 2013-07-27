#region

using System;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockFileSystemLibrary;
using PictureHandlerLibrary;
using PictureHandlerLibrary.FileHandler;
using UnitTests.MockClasses;

#endregion

namespace UnitTests {
	[TestClass]
	//[DeploymentItem("TestData\\eula.1041.txt", "TestData")]
	//[DeploymentItem("TestData\\IMG_6867.JPG", "TestData")]
	public class FileHanderRenameTests {
		private readonly IPictureDirectoryFactory _directoryFactory = new PictureDirectoryFactory();
		private readonly IFileHandlerFactory _fileHandlerFactory = new FileHandlerFactory();

		// Use TestInitialize to run code before running each test 
		/*[TestInitialize]
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
		}*/

		[TestMethod]
		public void InstantiateJpgRenamerWithExistingFile() {
			// Arrange
			var fileSystem = new MockFileSystem();
			fileSystem.AddFile(TestConstants.ExistingJpgFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));

			// Act
			IFileHandler fileHandler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingJpgFullFileName, fileSystem, new MockCompress(fileSystem),
																																		new MockExifReader(fileSystem));

			// Assert
			Assert.IsTrue(true);
		}

		[TestMethod]
		[ExpectedException(typeof (FileNotFoundException))]
		public void InstantiateJpgRenamerWithNonExistingFileInExistingDir() {
			// Arrange
			var fileSystem = new MockFileSystem();
			fileSystem.AddFile(TestConstants.ExistingJpgFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));

			// Act
			_fileHandlerFactory.GetFileHandler(TestConstants.NonExistingJpgFileNameInExistingDirectory, fileSystem, new MockCompress(fileSystem),
																				 new MockExifReader(fileSystem));

			// Assert
			Assert.Fail("Excepted exception");
		}

		[TestMethod]
		[ExpectedException(typeof (FileNotFoundException))]
		public void InstantiateJpgRenamerWithNonExistingFileInNonExistingDir() {
			// Arrange
			var fileSystem = new MockFileSystem();
			fileSystem.AddFile(TestConstants.ExistingJpgFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));

			// Act
			_fileHandlerFactory.GetFileHandler(TestConstants.NonExistingJpgFileNameInNonExistingDirectory);

			// Assert
			Assert.Fail();
		}

		[TestMethod]
		public void InstantiateRenamerOnJpgAndVerifyJpgRenamer() {
			// Arrange
			var fileSystem = new MockFileSystem();
			fileSystem.AddFile(TestConstants.ExistingJpgFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));

			// Act
			IFileHandler handler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingJpgFullFileName, fileSystem, new MockCompress(fileSystem),
																																new MockExifReader(fileSystem));

			// Assert
			Assert.AreEqual(handler.GetType(), typeof (JpgFileHandler));
		}

		[TestMethod]
		public void PreviewJpgFileNameReturnsNonEmptyString() {
			// Arrange
			var fileSystem = new MockFileSystem();
			fileSystem.AddFile(TestConstants.ExistingJpgFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));
			IFileHandler handler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingJpgFullFileName, fileSystem, new MockCompress(fileSystem),
																																new MockExifReader(fileSystem));

			// Act
			string previewRename = handler.GetNewRenamedFileName();

			// Assert
			Assert.IsTrue(previewRename.Length > 0);
		}

		[TestMethod]
		public void JpgGetNewFileNameIsProperlyNamed() {
			// Arrange
			var fileSystem = new MockFileSystem();
			fileSystem.AddFile(TestConstants.ExistingJpgFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));

			// Act
			IFileHandler handler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingJpgFullFileName, fileSystem, new MockCompress(fileSystem),
																																new MockExifReader(fileSystem));

			// Assert
			Assert.AreEqual(handler.GetNewRenamedFileName(), TestConstants.ExistingJpgDesiredNewRenamedShortFileName);
		}

		[TestMethod]
		[ExpectedException(typeof (ArgumentException))]
		public void CreateJpgFileHandlerWithNonJpgName_Fails() {
			// Arrange
			var fileSystem = new MockFileSystem();
			fileSystem.AddFile(TestConstants.ExistingTxtFullFileName, 1050, null);

			// Act
			new JpgFileHandler(TestConstants.ExistingTxtFullFileName, fileSystem);

			// Assert
			Assert.Fail();
		}

		[TestMethod]
		[ExpectedException(typeof (NullReferenceException))]
		public void PerformRenameAndMoveOnNullTargetDirectory_Fails() {
			// Arrange
			var fileSystem = new MockFileSystem();
			fileSystem.AddFile(TestConstants.ExistingJpgFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));
			IFileHandler handler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingJpgFullFileName, fileSystem, new MockCompress(fileSystem),
																																new MockExifReader(fileSystem));

			// Act
			handler.PerformRenameAndMove(null);

			// Assert
			Assert.Fail();
		}

		[TestMethod]
		public void RenameJpgFile() {
			// Arrange
			var fileSystem = new MockFileSystem();
			fileSystem.AddFile(TestConstants.ExistingJpgFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));
			fileSystem.CreateDirectory(TestConstants.NewDirectory);
			IFileHandler handler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingJpgFullFileName, fileSystem, new MockCompress(fileSystem),
																																new MockExifReader(fileSystem));
			var newDir = new MockPictureDirectory {Directory = TestConstants.NewDirectory};

			// Act
			handler.PerformRenameAndMove(newDir);

			// Assert
			Assert.IsTrue(!fileSystem.FileExists(TestConstants.ExistingJpgFullFileName));
			Assert.IsTrue(!fileSystem.FileExists(TestConstants.ExistingDirectory + "\\" + TestConstants.ExistingJpgShortFileName));
			Assert.IsTrue(!fileSystem.FileExists(TestConstants.ExistingDirectory + "\\" + TestConstants.ExistingJpgDesiredNewRenamedShortFileName));
			Assert.IsTrue(!fileSystem.FileExists(TestConstants.NewDirectory + "\\" + TestConstants.ExistingJpgShortFileName));
			Assert.IsTrue(fileSystem.FileExists(TestConstants.NewDirectory + "\\" + TestConstants.ExistingJpgDesiredNewRenamedShortFileName));
		}

		[TestMethod]
		[ExpectedException(typeof (EvaluateException))]
		public void RenameJpgWithMinValueDate() {
			RenameJpgWithSpecifiedDate(DateTime.MinValue);
		}

		[TestMethod]
		[ExpectedException(typeof (EvaluateException))]
		public void RenameJpgWithMaxValueDate() {
			RenameJpgWithSpecifiedDate(DateTime.MaxValue);
		}

		[TestMethod]
		[ExpectedException(typeof (EvaluateException))]
		public void RenameJpgWithFutureDate() {
			RenameJpgWithSpecifiedDate(DateTime.Now.AddYears(1));
		}

		[TestMethod]
		public void RenamingAlreadyRenamedFilesResultsInSameName() {
			// Arrange
			var fileSystem = new MockFileSystem();
			fileSystem.AddFile(TestConstants.ExistingJpgFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));
			fileSystem.CreateDirectory(TestConstants.NewDirectory);
			IFileHandler fileHandler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingJpgFullFileName, fileSystem, new MockCompress(fileSystem),
																																		new MockExifReader(fileSystem));

			var newDir = new MockPictureDirectory {Directory = TestConstants.NewDirectory};
			string firstRenamedFileName = fileHandler.GetNewRenamedFileName();
			fileHandler.PerformRenameAndMove(newDir);

			// Act
			string secondRenamedFileName = fileHandler.GetNewRenamedFileName();

			// Assert
			Assert.AreEqual(firstRenamedFileName, secondRenamedFileName);
		}

		[TestMethod]
		[ExpectedException(typeof (IOException))]
		public void RenamingToExistingFileFromOtherFileThrowsIoException() {
			// Arrange
			var fileSystem = new MockFileSystem();
			fileSystem.AddFile(TestConstants.ExistingJpgFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));
			fileSystem.CreateDirectory(TestConstants.NewDirectory);
			IFileHandler fileHandler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingJpgFullFileName, fileSystem, new MockCompress(fileSystem),
																																		new MockExifReader(fileSystem));

			var newDir = new MockPictureDirectory {Directory = TestConstants.NewDirectory};
			fileSystem.CopyFile(TestConstants.ExistingJpgFullFileName, newDir.Directory + "\\" + fileHandler.GetNewRenamedFileName());

			// Act
			fileHandler.PerformRenameAndMove(newDir);

			// Assert
			Assert.Fail("Expected exception");
		}

		[TestMethod]
		[ExpectedException(typeof (IOException))]
		public void RenamingToExistingFileWithSameNameFails() {
			// Arrange
			var fileSystem = new MockFileSystem();
			fileSystem.AddFile(TestConstants.ExistingJpgFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));
			fileSystem.CreateDirectory(TestConstants.NewDirectory);
			IFileHandler fileHandler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingJpgFullFileName, fileSystem, new MockCompress(fileSystem),
																																		new MockExifReader(fileSystem));
			var newDir = new MockPictureDirectory {Directory = TestConstants.NewDirectory};
			string firstFileName = fileHandler.PerformRenameAndMove(newDir);

			// Act
			string secondFileName = fileHandler.PerformRenameAndMove(newDir);

			// Assert
			Assert.AreEqual(firstFileName, secondFileName);
			Assert.AreNotEqual(secondFileName, TestConstants.ExistingJpgFullFileName);
		}

		[TestMethod]
		public void JpgFileHandlerGetNewFileNameReturnsSameNameForAlreadyRenamedFile() {
			// Arrange
			var fileSystem = new MockFileSystem();
			fileSystem.AddFile(TestConstants.ExistingJpgFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));
			fileSystem.CreateDirectory("\\tempdir");
			fileSystem.CreateDirectory("\\newdir");

			var tempDir = new MockPictureDirectory {Directory = "\\tempdir"};
			var newDir = new MockPictureDirectory {Directory = "\\newdir"};
			IFileHandler fileHandler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingJpgFullFileName, fileSystem, null, new MockExifReader(fileSystem));
			string newFileName = fileHandler.GetNewRenamedFileName();
			string movedFileName = fileHandler.PerformRenameAndMove(tempDir);
			IFileHandler fileHandler2 = _fileHandlerFactory.GetFileHandler(movedFileName, fileSystem, null, new MockExifReader(fileSystem));

			// Act
			string newFileName2 = fileHandler2.GetNewRenamedFileName();

			// Assert
			Assert.AreEqual(newFileName, newFileName2);
		}

		[TestMethod]
		public void JpgRenamerGetNewFileNameMatchesExpectedPatternOnFirstRename() {
			// Arrange
			var fileSystem = new MockFileSystem();
			fileSystem.AddFile(TestConstants.ExistingJpgFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));
			IFileHandler fileHandler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingJpgFullFileName, fileSystem, null, new MockExifReader(fileSystem));

			// Act
			string newFileName = fileHandler.GetNewRenamedFileName();

			// Assert
			Assert.IsTrue(Regex.IsMatch(newFileName, JpgFileHandler.ExpectedRenamedPattern));
		}

		[TestMethod]
		public void JpgRenamerGetNewFileNameMatchesExpectedPatternOnSecondRename() {
			// Arrange
			var fileSystem = new MockFileSystem();
			fileSystem.AddFile(TestConstants.ExistingJpgFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));
			fileSystem.CreateDirectory("newdir");
			IFileHandler fileHandler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingJpgFullFileName, fileSystem, null, new MockExifReader(fileSystem));
			string renamedFileName = fileHandler.PerformRenameAndMove(new MockPictureDirectory {Directory = "newdir"});

			// Act
			string newFileName2 = fileHandler.GetNewRenamedFileName();

			// Assert
			Assert.IsTrue(Regex.IsMatch(newFileName2, JpgFileHandler.ExpectedRenamedPattern));
		}

		[TestMethod]
		public void DoNotRenameCompressedPatternFileNames() {
			// Arrange
			var fileSystem = new MockFileSystem();
			fileSystem.AddFile(TestConstants.ExistingJpgFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));
			IFileHandler fileHandler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingJpgFullFileName, fileSystem, new MockCompress(fileSystem),
																																		new MockExifReader(fileSystem));

			var newDir = new MockPictureDirectory {Directory = TestConstants.NewDirectory};
			fileHandler.PerformRenameAndMove(newDir);
			fileHandler.PerformCompressAndMove(newDir);

			// Act
			bool caughtExpected = false;
			try {
				fileHandler.PerformRenameAndMove(newDir);
			} catch (FileLoadException ex) {
				if (ex.Message.Contains("Attempting to Rename already Compressed file")) {
					caughtExpected = true;
				}
			}

			// Assert
			Assert.IsTrue(caughtExpected);
		}

		private void RenameJpgWithSpecifiedDate(DateTime providedDate) {
			// Arrange
			var fileSystem = new MockFileSystem();
			fileSystem.AddFile(TestConstants.ExistingJpgFullFileName, 100000, providedDate);

			IFileHandler fileHandler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingJpgFullFileName, fileSystem, null, new MockExifReader(fileSystem));
			var newDir = new MockPictureDirectory {Directory = TestConstants.NewDirectory}; //.GetOrCreateDirectory(TestConstants.NewDirectory);

			// Act
			string test = fileHandler.PerformRenameAndMove(newDir);

			// Assert
			Assert.IsTrue(fileSystem.FileExists(TestConstants.ExistingJpgFullFileName));
		}

		/*
		 * TODO: IExif!
		private void RenameJpgWithSpecifiedDate(DateTime providedDate) {
			// Arrange
			IFileSystem fileSystem = new FileSystem();
			var fileHandler = MockRepository.GeneratePartialMock<JpgFileHandler>(TestConstants.ExistingJpgFullFileName);
			fileHandler.Stub(x => x.GetExifOriginalDateTimeRaw()).Return(providedDate);

			IPictureDirectory newDir = _directoryFactory.GetOrCreateDirectory(TestConstants.NewDirectory);

			// Act
			string test = fileHandler.PerformRenameAndMove(newDir);

			// Assert
			Assert.IsTrue(fileSystem.FileExists(TestConstants.ExistingJpgFullFileName));
		}*/
	}
}