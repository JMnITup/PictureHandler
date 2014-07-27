#region

using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using ExifLib;
using FileSystemLibrary;
using PictureHandlerLibrary.ImageCompression;

#endregion

namespace PictureHandlerLibrary.FileHandler {
    public class JpgFileHandler : FileHandler, IFileHandler {
        public const string ExpectedRenamedPattern = @"[0-9]{4}-[0-9]{2}-[0-9]{2}_[0-9]{2}\.[0-9]{2}\.[0-9]{2}_" + RenamedIdentifier + "_[0-9]{4}.JPG";
        public const string ExpectedCompressedPattern = @"[0-9]{4}-[0-9]{2}-[0-9]{2}_[0-9]{2}\.[0-9]{2}\.[0-9]{2}_" + CompressedIdentifier + "_[0-9]{4}.JPG";

        #region Overrides of FileHandler

        public override string GetNewRenamedFileName() {
            string datePart = null;
            try {
                datePart = ExifReader.GetOriginalDateTimeFormattedString(FileName);
            } catch (ExifLibException ex) {
                if (ex.Message.Contains("Could not find Exif data block")) {
                    Debug.WriteLine("File '" + FileName + "' does not contain Exif block");
                    throw new FileLoadException("File '" + FileName + "' does not contain Exif block");
                }
            } catch (EvaluateException ex) {
                if (ex.Message.Contains("Picture Original DateTime") && ex.Message.Contains("outside of accepted range")) {
                    Debug.WriteLine("File '" + FileName + "' has Exif block, but refers to invalid date");
                    throw new FileLoadException("File '" + FileName + "' has Exif block, but refers to invalid date");
                }
            } catch (FormatException ex) {
                if (ex.Message.Contains("String was not recognized as a valid DateTime")) {
                    Debug.WriteLine("Invalid datetime for file '" + FileName + "'");
                    throw new FileLoadException("Invalid datetime for file '" + FileName + "'");
                }
            }

            Match namePart = Regex.Match(FileName, @".*_([0-9]{4}\.jpg)", RegexOptions.IgnoreCase);
            string expectedName = datePart + "_" + RenamedIdentifier + "_" + namePart.Groups[1];
            if (Regex.IsMatch(expectedName, ExpectedRenamedPattern)) {
                if (Regex.IsMatch(FileName, ExpectedCompressedPattern)) {
                    throw new FileLoadException("Attempting to Rename already Compressed file");
                }
                return expectedName;
            }
            throw new FileLoadException("Expected name does not match defined pattern: '" + expectedName + "'");
        }

        public override string GetNewCompressedFileName() {
            Match namePart = Regex.Match(FileName, @"([^\\]*)_" + RenamedIdentifier + @"_([0-9]{4}\.jpg)", RegexOptions.IgnoreCase);
            string expectedName = namePart.Groups[1] + "_" + CompressedIdentifier + "_" + namePart.Groups[2];
            if (Regex.IsMatch(expectedName, ExpectedCompressedPattern)) {
                return expectedName;
            }
            throw new FileLoadException("Expected name does not match defined pattern: '" + expectedName + "'");
        }

        #endregion

        protected readonly IExifReader ExifReader;
        private readonly ICompress _compressor = new JpgCompressor();

        #region Implementation of IFileHandler

        public JpgFileHandler(string fileName, IFileSystem fileSystem = null, ICompress compressor = null, IExifReader exifReader = null) : base(fileName, fileSystem) {
            if (!fileName.EndsWith(".jpg", true, null)) {
                throw new ArgumentException("JpgFileHandler created with a file name not recognized as Jpg");
            }
            if (compressor != null) {
                _compressor = compressor;
            }
            if (exifReader == null) {
                ExifReader = new ExifReaderManager();
            } else {
                ExifReader = exifReader;
            }
        }

        #endregion

        public override string PerformCompressAndMove(IPictureDirectory targetDirectory) {
            string originalFileName = FileName;
            Match namePart = Regex.Match(FileName, @"\\([^\\]*)_" + RenamedIdentifier + @"_([0-9]{4}\.jpg)", RegexOptions.IgnoreCase);
            if (namePart.Groups.Count <= 1) {
                Debug.WriteLine("Attempt to compress un-matching named file '" + FileName + "'");
                throw new FileLoadException("Can only compress renamed files");
            }
            string shortOriginalFileName = namePart.Groups[1] + "_" + RenamedIdentifier + "_" + namePart.Groups[2];
            string shortNewFileName = namePart.Groups[1] + "_" + CompressedIdentifier + "_" + namePart.Groups[2];
            //string expectedName = datePart + "_JM" + namePart.Groups[2];
            string compressedFileName = targetDirectory.Directory + "\\" + shortNewFileName;
            if (FileSystem.FileExists(compressedFileName)) {
                Debug.WriteLine("File already exists in target location '" + FileName + "'");
                throw new IOException("File already exists in target location: " + compressedFileName);
            }

            _compressor.Compress(originalFileName, compressedFileName);
            if (FileSystem.FileExists(compressedFileName)) {
                FileSystem.DeleteFile(originalFileName);
                FileName = compressedFileName;
            }

            return compressedFileName;
        }
    }
}