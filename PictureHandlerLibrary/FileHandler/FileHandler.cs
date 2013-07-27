#region

using System.IO;
using FileSystemLibrary;

#endregion

namespace PictureHandlerLibrary.FileHandler {
	public abstract class FileHandler : IFileHandler {
/*
		protected FileHandler() {
			
		}*/
		public const string RenamedIdentifier = "RENAMED";
		public const string CompressedIdentifier = "COMPRESSED";
		protected readonly IFileSystem FileSystem = new FileSystem();
		private string _fileName;

		protected FileHandler(string fileName, IFileSystem fileSystem = null) {
			if (fileSystem != null) {
				FileSystem = fileSystem;
			}
			FileName = fileName;
		}

		#region Implementation of IFileHandler

		public virtual string FileName {
			get { return _fileName; }
			set {
				if (!FileSystem.FileExists(value)) {
					throw new FileNotFoundException();
				}
				_fileName = value;
			}
		}

		public abstract string GetNewRenamedFileName();
		public abstract string GetNewCompressedFileName();

		public string PerformRenameAndMove(IPictureDirectory targetDirectory) {
			string newName = GetNewRenamedFileName();
			string newFullName = targetDirectory.Directory + "\\" + newName;
			FileSystem.MoveFile(FileName, newFullName);
			FileName = newFullName;
			return newFullName;
		}

		public abstract string PerformCompressAndMove(IPictureDirectory targetDirectory);

/*		public string PerformCompressAndMove(IPictureDirectory targetDirectory) {
			// TODO: This is specific to JPG, but currently handled at parent class level
			string originalFileName = FileName;
			Match namePart = Regex.Match(FileName, @"\\([^\\]*)_" + RenamedIdentifier + @"_([0-9]{4}\.jpg)", RegexOptions.IgnoreCase);
			if (namePart.Groups.Count <= 1) {
				throw new FileLoadException("Can only compress renamed files");
			}
			string shortOriginalFileName = namePart.Groups[1] + "_" + RenamedIdentifier + "_" + namePart.Groups[2];
			string shortNewFileName = namePart.Groups[1] + "_" + CompressedIdentifier + "_" + namePart.Groups[2];
			//string expectedName = datePart + "_JM" + namePart.Groups[2];
			string compressedFileName = targetDirectory.Directory + "\\" + shortNewFileName;
			if (File.Exists(compressedFileName)) {
				throw new IOException("File already exists in target location: " + compressedFileName);
			}
			//ICompress compressor = new JpgCompressor();
			//compressor.Compress(originalFileName, compressedFileName);
			if (File.Exists(compressedFileName)) {
				File.Delete(originalFileName);
				FileName = compressedFileName;
			}

			return compressedFileName;
		}*/

		#endregion
	}
}