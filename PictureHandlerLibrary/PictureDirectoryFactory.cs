#region

#endregion

#region

using FileSystemLibrary;
using PictureHandlerLibrary.FileHandler;

#endregion

namespace PictureHandlerLibrary {
	public class PictureDirectoryFactory : IPictureDirectoryFactory {
		private readonly IExifReader _exifReader = new ExifReaderManager();
		private readonly IFileSystem _fileSystem = new FileSystem();

		public PictureDirectoryFactory(IFileSystem fileSystem = null, IExifReader exifReader = null) {
			if (fileSystem != null) {
				_fileSystem = fileSystem;
			}
			if (exifReader != null) {
				_exifReader = exifReader;
			}
		}

		public bool Exists(string directoryName) {
			return (_fileSystem.DirectoryExists(directoryName));
		}

		public IPictureDirectory GetOrCreateDirectory(string directoryName) {
			if (!Exists(directoryName)) {
				_fileSystem.CreateDirectory(directoryName);
			}
			return GetDirectory(directoryName);
		}

		public IPictureDirectory GetDirectory(string directoryName) {
			var dir = new PictureDirectory(directory: directoryName, fileSystem: _fileSystem, exifReader: _exifReader);
			return dir;
		}
	}
}