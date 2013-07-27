#region

using System;
using System.Diagnostics;
using PictureHandlerLibrary;

#endregion

namespace UnitTests.MockClasses {
	[DebuggerDisplay("{Directory}")]
	internal class MockPictureDirectory : IPictureDirectory {
		#region Implementation of IPictureDirectory

		public string Directory { get; set; }

		public string[] GetFileList() {
			throw new NotImplementedException();
		}

		public void RenameAllFiles(IPictureDirectory targetDirectory) {
			throw new NotImplementedException();
		}

		public void ResizeAllFiles(IPictureDirectory targetDirectory) {
			throw new NotImplementedException();
		}

		#endregion
	}
}