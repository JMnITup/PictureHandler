#region

#endregion

#region

using FileSystemLibrary;

#endregion

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