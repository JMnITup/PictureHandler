#region

using System;
using System.Data;
using ExifLib;

#endregion

namespace PictureHandlerLibrary.FileHandler {
	public class ExifReaderManager : IExifReader {
		#region Implementation of IExifReader

		public virtual DateTime GetOriginalDateTime(string fileName) {
			DateTime result;
			using (var reader = new ExifReader(fileName)) {
				reader.GetTagValue(ExifTags.DateTimeOriginal, out result);
			}
			return result;
		}

		public virtual string GetOriginalDateTimeFormattedString(string fileName) {
			DateTime result = GetOriginalDateTime(fileName);
			if (result.Year < 1980 || result.Year > DateTime.Now.Year) {
				throw new EvaluateException("Picture Original DateTime (" + result.Year + ") outside of accepted range");
			}

			return result.ToString("yyyy-MM-dd_HH.mm.ss");
		}

		#endregion
	}
}