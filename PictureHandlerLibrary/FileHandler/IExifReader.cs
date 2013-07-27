#region

using System;

#endregion

namespace PictureHandlerLibrary.FileHandler {
	public interface IExifReader {
		DateTime GetOriginalDateTime(string fileName);
		string GetOriginalDateTimeFormattedString(string fileName);
	}
}