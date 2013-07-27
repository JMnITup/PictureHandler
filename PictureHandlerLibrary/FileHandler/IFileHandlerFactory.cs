#region

using FileSystemLibrary;
using PictureHandlerLibrary.ImageCompression;

#endregion

namespace PictureHandlerLibrary.FileHandler {
	public interface IFileHandlerFactory {
		IFileHandler GetFileHandler(string fileName, IFileSystem fileSystem = null, ICompress compressor = null, IExifReader exifReader = null);
	}
}