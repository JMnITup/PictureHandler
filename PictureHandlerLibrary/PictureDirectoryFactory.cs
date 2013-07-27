// /*
// JamesM
// 2013 06 13 11:34 PM
// 2013 06 14 12:09 PM
// PictureDirectoryFactory.cs
// PictureHandler
// PictureHandler
// */

#region

#endregion

using FileSystemLibrary;

namespace PictureHandlerLibrary {
	public class PictureDirectoryFactory : IPictureDirectoryFactory {
		private readonly IFileSystem _fileSystem = new FileSystem();

		public PictureDirectoryFactory(IFileSystem fileSystem = null) {
			if (fileSystem != null) {
				_fileSystem = fileSystem;
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
			var dir = new PictureDirectory(directoryName);
			return dir;
		}
	}
}