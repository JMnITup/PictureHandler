// /*
// JamesM
// 2013 06 14 12:15 AM
// 2013 06 14 12:09 PM
// IPictureDirectoryFactory.cs
// PictureHandler
// PictureHandler
// */

namespace PictureHandlerLibrary {
	public interface IPictureDirectoryFactory {
		bool Exists(string directoryName);
		IPictureDirectory GetOrCreateDirectory(string directoryName);
		IPictureDirectory GetDirectory(string directoryName);
	}
}