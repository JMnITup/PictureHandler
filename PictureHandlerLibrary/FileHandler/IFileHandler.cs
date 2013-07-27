namespace PictureHandlerLibrary.FileHandler {
	public interface IFileHandler {
		string FileName { set; get; }
		string GetNewRenamedFileName();
		string GetNewCompressedFileName();
		string PerformRenameAndMove(IPictureDirectory targetDirectory);
		string PerformCompressAndMove(IPictureDirectory targetDirectory);
	}
}