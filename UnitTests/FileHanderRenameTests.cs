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
		private IPictureDirectoryFactory _directoryFactory = new PictureDirectoryFactory();
		private IFileHandlerFactory _fileHandlerFactory = new FileHandlerFactory();
		private MockFileSystem _fileSystem;

		[TestInitialize]
		public void MyTestInitialize() {
			_fileSystem = new MockFileSystem();
			_directoryFactory = new PictureDirectoryFactory(_fileSystem);
			_fileHandlerFactory = new FileHandlerFactory(_fileSystem);
			/*var fs = new FileSystem();
			fs.DeleteDirectoryAndAllFiles(TestConstants.ExistingDirectory);
			fs.DeleteDirectoryAndAllFiles(TestConstants.NewDirectory);
			fs.DeleteDirectoryAndAllFiles(TestConstants.TempDirectory);
			fs.DeleteDirectoryAndAllFiles(TestConstants.NonExistingDirectory);
			fs.CreateDirectory(TestConstants.ExistingDirectory);
			fs.CopyFiles("TestData", TestConstants.ExistingDirectory);*/
		}

		/*
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
		public void InstantiateJpgRenamerWithExistingFile() {
			// Arrange
			_fileSystem.AddFile(TestConstants.ExistingJpgFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));

			// Act
			IFileHandler fileHandler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingJpgFullFileName, _fileSystem, new MockCompress(_fileSystem),
																																		new MockExifReader(_fileSystem));

			// Assert
			Assert.IsTrue(true);
		}

		[TestMethod]
		[ExpectedException(typeof (FileNotFoundException))]
		public void InstantiateJpgRenamerWithNonExistingFileInExistingDir() {
			// Arrange
			_fileSystem.AddFile(TestConstants.ExistingJpgFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));

			// Act
			_fileHandlerFactory.GetFileHandler(TestConstants.NonExistingJpgFileNameInExistingDirectory, _fileSystem, new MockCompress(_fileSystem),
																				 new MockExifReader(_fileSystem));

			// Assert
			Assert.Fail("Excepted exception");
		}

		[TestMethod]
		[ExpectedException(typeof (FileNotFoundException))]
		public void InstantiateJpgRenamerWithNonExistingFileInNonExistingDir() {
			// Arrange
			_fileSystem.AddFile(TestConstants.ExistingJpgFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));

			// Act
			_fileHandlerFactory.GetFileHandler(TestConstants.NonExistingJpgFileNameInNonExistingDirectory);

			// Assert
			Assert.Fail();
		}

		[TestMethod]
		public void InstantiateRenamerOnJpgAndVerifyJpgRenamer() {
			// Arrange
			_fileSystem.AddFile(TestConstants.ExistingJpgFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));

			// Act
			IFileHandler handler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingJpgFullFileName, _fileSystem, new MockCompress(_fileSystem),
																																new MockExifReader(_fileSystem));

			// Assert
			Assert.AreEqual(handler.GetType(), typeof (JpgFileHandler));
		}

		[TestMethod]
		public void PreviewJpgFileNameReturnsNonEmptyString() {
			// Arrange
			_fileSystem.AddFile(TestConstants.ExistingJpgFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));
			IFileHandler handler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingJpgFullFileName, _fileSystem, new MockCompress(_fileSystem),
																																new MockExifReader(_fileSystem));

			// Act
			string previewRename = handler.GetNewRenamedFileName();

			// Assert
			Assert.IsTrue(previewRename.Length > 0);
		}

		[TestMethod]
		public void JpgGetNewFileNameIsProperlyNamed() {
			// Arrange
			_fileSystem.AddFile(TestConstants.ExistingJpgFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));

			// Act
			IFileHandler handler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingJpgFullFileName, _fileSystem, new MockCompress(_fileSystem),
																																new MockExifReader(_fileSystem));

			// Assert
			Assert.AreEqual(handler.GetNewRenamedFileName(), TestConstants.ExistingJpgDesiredNewRenamedShortFileName);
		}

		[TestMethod]
		[ExpectedException(typeof (ArgumentException))]
		public void CreateJpgFileHandlerWithNonJpgName_Fails() {
			// Arrange
			_fileSystem.AddFile(TestConstants.ExistingTxtFullFileName, 1050, null);

			// Act
			new JpgFileHandler(TestConstants.ExistingTxtFullFileName, _fileSystem);

			// Assert
			Assert.Fail();
		}

		[TestMethod]
		[ExpectedException(typeof (NullReferenceException))]
		public void PerformRenameAndMoveOnNullTargetDirectory_Fails() {
			// Arrange
			_fileSystem.AddFile(TestConstants.ExistingJpgFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));
			IFileHandler handler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingJpgFullFileName, _fileSystem, new MockCompress(_fileSystem),
																																new MockExifReader(_fileSystem));

			// Act
			handler.PerformRenameAndMove(null);

			// Assert
			Assert.Fail();
		}

		[TestMethod]
		public void RenameJpgFile() {
			// Arrange
			_fileSystem.AddFile(TestConstants.ExistingJpgFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));
			_fileSystem.CreateDirectory(TestConstants.NewDirectory);

			IFileHandler handler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingJpgFullFileName, _fileSystem, new MockCompress(_fileSystem),
																																new MockExifReader(_fileSystem));
			//var newDir = new MockPictureDirectory {Directory = TestConstants.NewDirectory};
			var newDir = _directoryFactory.GetDirectory(TestConstants.NewDirectory);

			// Act
			handler.PerformRenameAndMove(newDir);

			// Assert
			Assert.IsTrue(!_fileSystem.FileExists(TestConstants.ExistingJpgFullFileName));
			Assert.IsTrue(!_fileSystem.FileExists(TestConstants.ExistingDirectory + "\\" + TestConstants.ExistingJpgShortFileName));
			Assert.IsTrue(!_fileSystem.FileExists(TestConstants.ExistingDirectory + "\\" + TestConstants.ExistingJpgDesiredNewRenamedShortFileName));
			Assert.IsTrue(!_fileSystem.FileExists(TestConstants.NewDirectory + "\\" + TestConstants.ExistingJpgShortFileName));
			Assert.IsTrue(_fileSystem.FileExists(TestConstants.NewDirectory + "\\" + TestConstants.ExistingJpgDesiredNewRenamedShortFileName));
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
			_fileSystem.AddFile(TestConstants.ExistingJpgFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));
			_fileSystem.CreateDirectory(TestConstants.NewDirectory);
			IFileHandler fileHandler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingJpgFullFileName, _fileSystem, new MockCompress(_fileSystem),
																																		new MockExifReader(_fileSystem));

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
			_fileSystem.AddFile(TestConstants.ExistingJpgFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));
			_fileSystem.CreateDirectory(TestConstants.NewDirectory);
			IFileHandler fileHandler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingJpgFullFileName, _fileSystem, new MockCompress(_fileSystem),
																																		new MockExifReader(_fileSystem));

			var newDir = new MockPictureDirectory {Directory = TestConstants.NewDirectory};
			_fileSystem.CopyFile(TestConstants.ExistingJpgFullFileName, newDir.Directory + "\\" + fileHandler.GetNewRenamedFileName());

			// Act
			fileHandler.PerformRenameAndMove(newDir);

			// Assert
			Assert.Fail("Expected exception");
		}

		[TestMethod]
		[ExpectedException(typeof (IOException))]
		public void RenamingToExistingFileWithSameNameFails() {
			// Arrange
			_fileSystem.AddFile(TestConstants.ExistingJpgFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));
			_fileSystem.CreateDirectory(TestConstants.NewDirectory);
			IFileHandler fileHandler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingJpgFullFileName, _fileSystem, new MockCompress(_fileSystem),
																																		new MockExifReader(_fileSystem));
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
			_fileSystem.AddFile(TestConstants.ExistingJpgFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));
			_fileSystem.CreateDirectory("\\tempdir");
			_fileSystem.CreateDirectory("\\newdir");

			var tempDir = new MockPictureDirectory {Directory = "\\tempdir"};
			var newDir = new MockPictureDirectory {Directory = "\\newdir"};
			IFileHandler fileHandler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingJpgFullFileName, _fileSystem, null, new MockExifReader(_fileSystem));
			string newFileName = fileHandler.GetNewRenamedFileName();
			string movedFileName = fileHandler.PerformRenameAndMove(tempDir);
			IFileHandler fileHandler2 = _fileHandlerFactory.GetFileHandler(movedFileName, _fileSystem, null, new MockExifReader(_fileSystem));

			// Act
			string newFileName2 = fileHandler2.GetNewRenamedFileName();

			// Assert
			Assert.AreEqual(newFileName, newFileName2);
		}

		[TestMethod]
		public void JpgRenamerGetNewFileNameMatchesExpectedPatternOnFirstRename() {
			// Arrange
			_fileSystem.AddFile(TestConstants.ExistingJpgFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));
			IFileHandler fileHandler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingJpgFullFileName, _fileSystem, null, new MockExifReader(_fileSystem));

			// Act
			string newFileName = fileHandler.GetNewRenamedFileName();

			// Assert
			Assert.IsTrue(Regex.IsMatch(newFileName, JpgFileHandler.ExpectedRenamedPattern));
		}

		[TestMethod]
		public void JpgRenamerGetNewFileNameMatchesExpectedPatternOnSecondRename() {
			// Arrange
			_fileSystem.AddFile(TestConstants.ExistingJpgFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));
			_fileSystem.CreateDirectory("newdir");
			IFileHandler fileHandler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingJpgFullFileName, _fileSystem, null, new MockExifReader(_fileSystem));
			string renamedFileName = fileHandler.PerformRenameAndMove(new MockPictureDirectory {Directory = "newdir"});

			// Act
			string newFileName2 = fileHandler.GetNewRenamedFileName();

			// Assert
			Assert.IsTrue(Regex.IsMatch(newFileName2, JpgFileHandler.ExpectedRenamedPattern));
		}

		[TestMethod]
		public void DoNotRenameCompressedPatternFileNames() {
			// Arrange
			_fileSystem.AddFile(TestConstants.ExistingJpgFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));
			IFileHandler fileHandler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingJpgFullFileName, _fileSystem, new MockCompress(_fileSystem),
																																		new MockExifReader(_fileSystem));

			var newDir = _directoryFactory.GetOrCreateDirectory(TestConstants.NewDirectory);
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
			_fileSystem.AddFile(TestConstants.ExistingJpgFullFileName, 100000, providedDate);

			IFileHandler fileHandler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingJpgFullFileName, _fileSystem, null, new MockExifReader(_fileSystem));
			var newDir = new MockPictureDirectory {Directory = TestConstants.NewDirectory}; //.GetOrCreateDirectory(TestConstants.NewDirectory);

			// Act
			string test = fileHandler.PerformRenameAndMove(newDir);

			// Assert
			Assert.IsTrue(_fileSystem.FileExists(TestConstants.ExistingJpgFullFileName));
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