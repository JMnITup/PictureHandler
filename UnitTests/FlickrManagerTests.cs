#region

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockFileSystemLibrary;
using PictureHandlerLibrary;
using PictureHandlerLibrary.FileHandler;
using UnitTests.MockClasses;

#endregion

namespace UnitTests {
	[TestClass]
	public class FlickrManagerTests {
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
	}
}