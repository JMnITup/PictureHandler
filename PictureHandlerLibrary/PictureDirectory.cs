#region

using System;
using System.IO;
using FileSystemLibrary;
using PictureHandlerLibrary.FileHandler;

#endregion

namespace PictureHandlerLibrary {
	public class PictureDirectory : IPictureDirectory {
		private readonly IExifReader _exifReader = new ExifReaderManager();
		private readonly IFileHandlerFactory _fileHandlerFactory = new FileHandlerFactory();
		private readonly IFileSystem _fileSystem = new FileSystem();
		private readonly PictureDirectoryFactory _pictureDirectoryFactory = new PictureDirectoryFactory();
		private string _directory;

		internal PictureDirectory(string directory = null, IFileSystem fileSystem = null, IExifReader exifReader = null) {
			if (directory == null) {
				throw new ArgumentNullException("directory");
			}
			if (fileSystem != null) {
				_fileSystem = fileSystem;
				_pictureDirectoryFactory = new PictureDirectoryFactory(fileSystem, exifReader);
				_fileHandlerFactory = new FileHandlerFactory(fileSystem);
			}
			if (exifReader != null) {
				_exifReader = exifReader;
			}
			Directory = directory;
		}

		public PictureDirectoryFactory PictureDirectoryFactory {
			get { return _pictureDirectoryFactory; }
		}

		public string Directory {
			get { return _directory; }
			set {
				if (_pictureDirectoryFactory.Exists(value)) {
					_directory = value;
				} else {
					throw new DirectoryNotFoundException();
				}
			}
		}

		public string[] GetFileList() {
			string[] files = (_fileSystem.GetFilesInDirectory(Directory));
			return files;
		}

		public void RenameAllFiles(IPictureDirectory targetDirectory) {
			string[] fileList = GetFileList();
			foreach (string fileName in fileList) {
				try {
					IFileHandler handler = _fileHandlerFactory.GetFileHandler(fileName, _fileSystem, null, _exifReader);
					try {
						string newFileName = handler.PerformRenameAndMove(targetDirectory);
					} catch (IOException) {
						// this file could not be moved - move to the next
						// TODO: log issue
					}
				} catch (NotImplementedException) {}
			}
		}

		public void ResizeAllFiles(IPictureDirectory targetDirectory) {
			string[] fileList = GetFileList();
			foreach (string fileName in fileList) {
				try {
					IFileHandler handler = _fileHandlerFactory.GetFileHandler(fileName, _fileSystem, null, _exifReader);
					try {
						string newFileName = handler.PerformCompressAndMove(targetDirectory);
					} catch (IOException) {
						// this file could not be moved - move to the next
						// TODO: log issue
					}
				} catch (NotImplementedException) {}
			}
		}

		public override string ToString() {
			return Directory;
		}
	}
}