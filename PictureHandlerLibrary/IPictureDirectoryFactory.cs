namespace PictureHandlerLibrary {
	public interface IPictureDirectoryFactory {
		bool Exists(string directoryName);
		IPictureDirectory GetOrCreateDirectory(string directoryName);
		IPictureDirectory GetDirectory(string directoryName);
	}
}