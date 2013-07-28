#region

using System;
using FileSystemLibrary;
using MockFileSystemLibrary;
using PictureHandlerLibrary.FileHandler;

#endregion

namespace UnitTests.MockClasses {
	internal class MockExifReader : ExifReaderManager, IExifReader {
		private readonly IFileSystem _fileSystem;

		public MockExifReader(IFileSystem fileSystem) {
			_fileSystem = fileSystem;
		}

		#region Implementation of IExifReader

		public override DateTime GetOriginalDateTime(string fileName) {
			DateTime? nullableDateTime = ((MockFileSystem) _fileSystem).GetMockExifData(fileName);
			if (nullableDateTime == null) {
				return DateTime.MinValue;
			}
			return (DateTime) nullableDateTime;
		}

		#endregion
	}
}