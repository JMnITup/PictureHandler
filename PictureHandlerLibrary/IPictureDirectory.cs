// /*
// JamesM
// 2013 06 13 9:53 PM
// 2013 06 14 12:09 PM
// IPictureDirectory.cs
// PictureHandler
// PictureHandler
// */

namespace PictureHandlerLibrary {
	public interface IPictureDirectory {
		string Directory { get; set; }
		string[] GetFileList();
		void RenameAllFiles(IPictureDirectory targetDirectory);
		void ResizeAllFiles(IPictureDirectory targetDirectory);
	}
}