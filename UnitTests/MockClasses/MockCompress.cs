#region

using System;
using FileSystemLibrary;
using MockFileSystemLibrary;
using PictureHandlerLibrary.ImageCompression;

#endregion

namespace UnitTests.MockClasses {
	internal class MockCompress : ICompress {
		public IFileSystem FileSystem;

		public MockCompress(IFileSystem fileSystem) {
			FileSystem = fileSystem;
		}

		#region Implementation of ICompress

		public void Compress(string originalFileName, string compressedFileName) {
			if (!FileSystem.FileExists(originalFileName)) {
				throw new Exception(string.Format("Mock class failed to find original filename: {0}", originalFileName));
			}
			if (FileSystem.FileExists(compressedFileName)) {
				throw new Exception(string.Format("Mock class attempted to compress to already existing filename: {0}", compressedFileName));
			}

			((MockFileSystem) FileSystem).CopyFile(originalFileName, compressedFileName);
			((MockFileSystem) FileSystem).ResizeFile(compressedFileName, 1);
		}

		#endregion
	}
}