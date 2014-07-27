namespace PictureHandlerLibrary {
    public interface IPictureDirectory {
        string Directory { get; set; }
        string[] GetFileList();
        void RenameAllFiles(IPictureDirectory targetDirectory, bool recursive = false);
        void ResizeAllFiles(IPictureDirectory targetDirectory);
    }
}