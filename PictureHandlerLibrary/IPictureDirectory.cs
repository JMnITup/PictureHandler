namespace PictureHandlerLibrary {
	public interface IPictureDirectory {
		string Directory { get; set; }
		string[] GetFileList();
		void RenameAllFiles(IPictureDirectory targetDirectory);
		void ResizeAllFiles(IPictureDirectory targetDirectory);
	}
}