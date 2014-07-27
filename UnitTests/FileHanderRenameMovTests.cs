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
    public class FileHanderRenameMovTests {
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
        public void InstantiateMovRenamerWithExistingFile() {
            // Arrange
            _fileSystem.AddFile(TestConstants.ExistingMovFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));

            // Act
            IFileHandler fileHandler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingMovFullFileName, _fileSystem, new MockCompress(_fileSystem),
                new MockExifReader(_fileSystem));

            // Assert
            Assert.IsTrue(true);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void InstantiateMovRenamerWithNonExistingFileInExistingDir() {
            // Arrange
            _fileSystem.AddFile(TestConstants.ExistingMovFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));

            // Act
            _fileHandlerFactory.GetFileHandler(TestConstants.NonExistingMovFileNameInExistingDirectory, _fileSystem, new MockCompress(_fileSystem),
                new MockExifReader(_fileSystem));

            // Assert
            Assert.Fail("Excepted exception");
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void InstantiateMovRenamerWithNonExistingFileInNonExistingDir() {
            // Arrange
            _fileSystem.AddFile(TestConstants.ExistingMovFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));

            // Act
            _fileHandlerFactory.GetFileHandler(TestConstants.NonExistingMovFileNameInNonExistingDirectory, _fileSystem);

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void InstantiateRenamerOnMovAndVerifyMovRenamer() {
            // Arrange
            _fileSystem.AddFile(TestConstants.ExistingMovFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));

            // Act
            IFileHandler handler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingMovFullFileName, _fileSystem, new MockCompress(_fileSystem),
                new MockExifReader(_fileSystem));

            // Assert
            Assert.AreEqual(handler.GetType(), typeof(MovFileHandler));
        }

        [TestMethod]
        public void PreviewMovFileNameReturnsNonEmptyString() {
            // Arrange
            _fileSystem.AddFile(TestConstants.ExistingMovFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));
            IFileHandler handler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingMovFullFileName, _fileSystem, new MockCompress(_fileSystem),
                new MockExifReader(_fileSystem));

            // Act
            string previewRename = handler.GetNewRenamedFileName();

            // Assert
            Assert.IsTrue(previewRename.Length > 0);
        }

        [TestMethod]
        public void MovGetNewFileNameIsProperlyNamed() {
            // Arrange
            _fileSystem.AddFile(TestConstants.ExistingMovFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));

            // Act
            IFileHandler handler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingMovFullFileName, _fileSystem, new MockCompress(_fileSystem),
                new MockExifReader(_fileSystem));

            // Assert
            Assert.AreEqual(handler.GetNewRenamedFileName(), TestConstants.ExistingMovDesiredNewRenamedShortFileName);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateMovFileHandlerWithNonMovName_Fails() {
            // Arrange
            _fileSystem.AddFile(TestConstants.ExistingTxtFullFileName, 1050, null);

            // Act
            new MovFileHandler(TestConstants.ExistingTxtFullFileName, _fileSystem);

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void PerformRenameAndMoveOnNullTargetDirectory_Fails() {
            // Arrange
            _fileSystem.AddFile(TestConstants.ExistingMovFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));
            IFileHandler handler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingMovFullFileName, _fileSystem, new MockCompress(_fileSystem),
                new MockExifReader(_fileSystem));

            // Act
            handler.PerformRenameAndMove(null);

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void RenameMovFile() {
            // Arrange
            _fileSystem.AddFile(TestConstants.ExistingMovFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));
            _fileSystem.CreateDirectory(TestConstants.NewDirectory);

            IFileHandler handler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingMovFullFileName, _fileSystem, new MockCompress(_fileSystem),
                new MockExifReader(_fileSystem));
            //var newDir = new MockPictureDirectory {Directory = TestConstants.NewDirectory};
            IPictureDirectory newDir = _directoryFactory.GetDirectory(TestConstants.NewDirectory);

            // Act
            handler.PerformRenameAndMove(newDir);

            // Assert
            Assert.IsTrue(!_fileSystem.FileExists(TestConstants.ExistingMovFullFileName));
            Assert.IsTrue(!_fileSystem.FileExists(TestConstants.ExistingDirectory + "\\" + TestConstants.ExistingMovShortFileName));
            Assert.IsTrue(!_fileSystem.FileExists(TestConstants.ExistingDirectory + "\\" + TestConstants.ExistingMovDesiredNewRenamedShortFileName));
            Assert.IsTrue(!_fileSystem.FileExists(TestConstants.NewDirectory + "\\" + TestConstants.ExistingMovShortFileName));
            Assert.IsTrue(_fileSystem.FileExists(TestConstants.NewDirectory + "\\" + TestConstants.ExistingMovDesiredNewRenamedShortFileName));
        }

        [TestMethod]
        [ExpectedException(typeof(FileLoadException))]
        public void RenameMovWithMinValueDateThrowsFileLoadException() {
            RenameMovWithSpecifiedDate(DateTime.MinValue);
        }

        [TestMethod]
        [ExpectedException(typeof(FileLoadException))]
        public void RenameMovWithMaxValueDateThrowsFileLoadException() {
            RenameMovWithSpecifiedDate(DateTime.MaxValue);
        }

        [TestMethod]
        [ExpectedException(typeof(FileLoadException))]
        public void RenameMovWithFutureDateThrowsFileLoadException() {
            RenameMovWithSpecifiedDate(DateTime.Now.AddYears(1));
        }

        [TestMethod]
        public void RenamingAlreadyRenamedFilesResultsInSameName() {
            // Arrange
            _fileSystem.AddFile(TestConstants.ExistingMovFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));
            _fileSystem.CreateDirectory(TestConstants.NewDirectory);
            IFileHandler fileHandler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingMovFullFileName, _fileSystem, new MockCompress(_fileSystem),
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
        [ExpectedException(typeof(IOException))]
        public void RenamingToExistingFileFromOtherFileThrowsIoException() {
            // Arrange
            _fileSystem.AddFile(TestConstants.ExistingMovFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));
            _fileSystem.CreateDirectory(TestConstants.NewDirectory);
            IFileHandler fileHandler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingMovFullFileName, _fileSystem, new MockCompress(_fileSystem),
                new MockExifReader(_fileSystem));

            var newDir = new MockPictureDirectory {Directory = TestConstants.NewDirectory};
            _fileSystem.CopyFile(TestConstants.ExistingMovFullFileName, newDir.Directory + "\\" + fileHandler.GetNewRenamedFileName());

            // Act
            fileHandler.PerformRenameAndMove(newDir);

            // Assert
            Assert.Fail("Expected exception");
        }

        [TestMethod]
        [ExpectedException(typeof(IOException))]
        public void RenamingToExistingFileWithSameNameFails() {
            // Arrange
            _fileSystem.AddFile(TestConstants.ExistingMovFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));
            _fileSystem.CreateDirectory(TestConstants.NewDirectory);
            IFileHandler fileHandler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingMovFullFileName, _fileSystem, new MockCompress(_fileSystem),
                new MockExifReader(_fileSystem));
            var newDir = new MockPictureDirectory {Directory = TestConstants.NewDirectory};
            string firstFileName = fileHandler.PerformRenameAndMove(newDir);

            // Act
            string secondFileName = fileHandler.PerformRenameAndMove(newDir);

            // Assert
            Assert.AreEqual(firstFileName, secondFileName);
            Assert.AreNotEqual(secondFileName, TestConstants.ExistingMovFullFileName);
        }

        [TestMethod]
        public void MovFileHandlerGetNewFileNameReturnsSameNameForAlreadyRenamedFile() {
            // Arrange
            _fileSystem.AddFile(TestConstants.ExistingMovFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));
            _fileSystem.CreateDirectory("\\tempdir");
            _fileSystem.CreateDirectory("\\newdir");

            var tempDir = new MockPictureDirectory {Directory = "\\tempdir"};
            var newDir = new MockPictureDirectory {Directory = "\\newdir"};
            IFileHandler fileHandler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingMovFullFileName, _fileSystem, null, new MockExifReader(_fileSystem));
            string newFileName = fileHandler.GetNewRenamedFileName();
            string movedFileName = fileHandler.PerformRenameAndMove(tempDir);
            IFileHandler fileHandler2 = _fileHandlerFactory.GetFileHandler(movedFileName, _fileSystem, null, new MockExifReader(_fileSystem));

            // Act
            string newFileName2 = fileHandler2.GetNewRenamedFileName();

            // Assert
            Assert.AreEqual(newFileName, newFileName2);
        }

        [TestMethod]
        public void MovRenamerGetNewFileNameMatchesExpectedPatternOnFirstRename() {
            // Arrange
            _fileSystem.AddFile(TestConstants.ExistingMovFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));
            IFileHandler fileHandler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingMovFullFileName, _fileSystem, null, new MockExifReader(_fileSystem));

            // Act
            string newFileName = fileHandler.GetNewRenamedFileName();

            // Assert
            Assert.IsTrue(Regex.IsMatch(newFileName, MovFileHandler.ExpectedRenamedPattern));
        }

        [TestMethod]
        public void MovRenamerGetNewFileNameMatchesExpectedPatternOnSecondRename() {
            // Arrange
            _fileSystem.AddFile(TestConstants.ExistingMovFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));
            _fileSystem.CreateDirectory("newdir");
            IFileHandler fileHandler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingMovFullFileName, _fileSystem, null, new MockExifReader(_fileSystem));
            string renamedFileName = fileHandler.PerformRenameAndMove(new MockPictureDirectory {Directory = "newdir"});

            // Act
            string newFileName2 = fileHandler.GetNewRenamedFileName();

            // Assert
            Assert.IsTrue(Regex.IsMatch(newFileName2, MovFileHandler.ExpectedRenamedPattern));
        }

        [TestMethod]
        public void DoNotRenameCompressedPatternFileNames() {
            // Arrange
            _fileSystem.AddFile(TestConstants.ExistingMovFullFileName, 100000, new DateTime(2013, 5, 29, 19, 39, 18));
            IFileHandler fileHandler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingMovFullFileName, _fileSystem, new MockCompress(_fileSystem),
                new MockExifReader(_fileSystem));

            IPictureDirectory newDir = _directoryFactory.GetOrCreateDirectory(TestConstants.NewDirectory);
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

        private void RenameMovWithSpecifiedDate(DateTime providedDate) {
            // Arrange
            _fileSystem.AddFile(TestConstants.ExistingMovFullFileName, 100000, providedDate);

            IFileHandler fileHandler = _fileHandlerFactory.GetFileHandler(TestConstants.ExistingMovFullFileName, _fileSystem, null, new MockExifReader(_fileSystem));
            var newDir = new MockPictureDirectory {Directory = TestConstants.NewDirectory}; //.GetOrCreateDirectory(TestConstants.NewDirectory);

            // Act
            string test = fileHandler.PerformRenameAndMove(newDir);

            // Assert
            Assert.IsTrue(_fileSystem.FileExists(TestConstants.ExistingMovFullFileName));
        }
    }
}