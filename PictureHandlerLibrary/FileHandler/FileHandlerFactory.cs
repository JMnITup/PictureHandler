#region

using System;
using FileSystemLibrary;
using PictureHandlerLibrary.ImageCompression;

#endregion

namespace PictureHandlerLibrary.FileHandler {
	public class FileHandlerFactory : IFileHandlerFactory {
		private readonly IFileSystem _fileSystem = new FileSystem();

		public FileHandlerFactory(IFileSystem fileSystem = null) {
			if (fileSystem != null) {
				_fileSystem = fileSystem;
			}
		}

		#region Implementation of IFileHandlerFactory

		public IFileHandler GetFileHandler(string fileName, IFileSystem fileSystem = null, ICompress compressor = null, IExifReader exifReader = null) {
			if (fileName.EndsWith(".jpg", true, null)) {
				return new JpgFileHandler(fileName, fileSystem: fileSystem, compressor: compressor, exifReader: exifReader);
			}
			throw new NotImplementedException("File type is not yet implemented: " + fileName);
		}

		public bool Exists(string directoryName) {
			return (_fileSystem.FileExists(directoryName));
		}

		#endregion
	}
}