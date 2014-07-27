namespace UnitTests {
    public class TestConstants {
        public const string ExistingDirectory = "ExistingDirectory";
        public const string NonExistingDirectory = "NonExistantTestData";
        public const string TempDirectory = @"TempDirectory";
        public const string NewDirectory = @"NewDirectory";

        public const string ExistingJpgShortFileName = @"\IMG_6867.JPG";

        public const string ExistingJpgFullFileName = ExistingDirectory + @"\" + ExistingJpgShortFileName;
        public const string ExistingJpgDesiredNewRenamedShortFileName = "2013-05-29_19.39.18_RENAMED_6867.JPG";
        public const string ExistingJpgDesiredNewCompressedShortFileName = "2013-05-29_19.39.18_COMPRESSED_6867.JPG";
        public const string ExistingTxtFullFileName = ExistingDirectory + @"\eula.1041.txt";
        public const string NonExistingJpgFileNameInExistingDirectory = ExistingDirectory + @"\IMG_6136.JPG";
        public const string NonExistingJpgFileNameInNonExistingDirectory = NonExistingDirectory + @"\IMG_6136.JPG";
    }
}