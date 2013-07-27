#region

using System.Drawing;
using System.Drawing.Imaging;

#endregion

namespace PictureHandlerLibrary.ImageCompression {
	public class JpgCompressor : ICompress {
		public const long JpgCompressionValue = 80L;

		#region Implementation of ICompress

		public void Compress(string originalFileName, string compressedFileName) {
			// Get a bitmap.
			using (var bmp1 = new Bitmap(originalFileName)) {
				ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);

				// Create an Encoder object based on the GUID 
				// for the Quality parameter category.
				Encoder myEncoder =
					Encoder.Quality;

				// Create an EncoderParameters object. 
				// An EncoderParameters object has an array of EncoderParameter 
				// objects. In this case, there is only one 
				// EncoderParameter object in the array.
				var myEncoderParameters = new EncoderParameters(1);

				var myEncoderParameter = new EncoderParameter(myEncoder, JpgCompressionValue);
				myEncoderParameters.Param[0] = myEncoderParameter;
				// TODO: change this to a file stream?
				bmp1.Save(compressedFileName, jgpEncoder, myEncoderParameters);
			}
		}

		private ImageCodecInfo GetEncoder(ImageFormat format) {
			ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

			foreach (ImageCodecInfo codec in codecs) {
				if (codec.FormatID == format.Guid) {
					return codec;
				}
			}
			return null;
		}

		#endregion
	}
}