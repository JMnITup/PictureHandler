#region

using System;
using System.Collections.Generic;
using System.IO;
using PictureHandlerLibrary;

#endregion

namespace UnitTests.MockClasses {
	internal class MockFileSystem : IFileSystem {
		private readonly Dictionary<string, File> _fileList = new Dictionary<string, File>();

		public void AddFile(File newFile) {
			_fileList.Add(newFile.FileName, newFile);
		}

		public void AddFile(string fileName, long fileSize, DateTime? exifDateTimeOriginal = null) {
			var newFile = new File(fileName, fileSize);
			newFile.ExifDateTimeOriginal = exifDateTimeOriginal;
			_fileList.Add(fileName, newFile);
		}

		public void ResizeFile(string fileName, long fileSize) {
			_fileList[fileName].FileSize = fileSize;
		}

		public DateTime? GetMockExifData(string fileName) {
			return _fileList[fileName].ExifDateTimeOriginal;
		}

		#region Implementation of IFileSystem

		public bool FileExists(string pathName) {
			return _fileList.ContainsKey(pathName);
		}

		public string[] GetFilesInDirectory(string directory) {
			throw new NotImplementedException();
		}

		public void MoveFile(string fileToMove, string newLocation) {
			if (!FileExists(fileToMove)) {
				throw new FileNotFoundException(string.Format("File to move ({0}) not found in mock", fileToMove));
			}
			if (FileExists(newLocation)) {
				throw new IOException(string.Format("Attempt to move file to already existing location ({0}) in mock", newLocation));
			}
			File updatedFile = _fileList[fileToMove];
			updatedFile.FileName = newLocation;
			DeleteFile(fileToMove);
			AddFile(updatedFile);
		}

		public bool DirectoryExists(string directoryName) {
			throw new NotImplementedException();
		}

		public void DeleteDirectory(string directoryName) {
			throw new NotImplementedException();
		}

		public void CreateDirectory(string directoryName) {
			throw new NotImplementedException();
		}

		public void DeleteFile(string fileName) {
			_fileList.Remove(fileName);
		}

		public void CopyFile(string fromFileName, string toFileName) {
			if (!FileExists(fromFileName)) {
				throw new FileNotFoundException(string.Format("File to copy ({0}) not found in mock", fromFileName));
			}
			if (FileExists(toFileName)) {
				throw new IOException(string.Format("Attempt to copy file to already existing location ({0}) in mock", toFileName));
			}
			File updatedFile = _fileList[fromFileName];
			updatedFile.FileName = toFileName;
			AddFile(updatedFile);
		}

		public void DeleteDirectoryAndAllFiles(string directoryName) {
			throw new NotImplementedException();
		}

		public long GetFileLength(string fileName) {
			if (!FileExists(fileName)) {
				throw new FileNotFoundException(string.Format("File to get legth of ({0}) not found in mock", fileName));
			}
			return _fileList[fileName].FileSize;
		}

		public void CopyFiles(string sourceDirectory, string targetDirectory) {
			throw new NotImplementedException();
		}

		#endregion

		internal class File {
			public string FileName;
			public long FileSize;

			public File(string fileName, long fileSize) {
				FileName = fileName;
				FileSize = fileSize;
			}

			public DateTime? ExifDateTimeOriginal { get; set; }
		}
	}
}