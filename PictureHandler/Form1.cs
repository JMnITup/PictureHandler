// /*
// JamesM
// 2013 06 13 9:32 PM
// 2013 06 17 10:18 PM
// Form1.cs
// PictureHandlerUI
// PictureHandler
// */

#region

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using PictureHandlerLibrary;

#endregion

namespace PictureHandler {
	public partial class Form1 : Form {
		public Form1() {
			InitializeComponent();
		}

		private void tbToConvertFolder_TextChanged(object sender, EventArgs e) {}

		private void btnProcessRawInput_Click(object sender, EventArgs e) {
			var pictureDirectoryFactory = new PictureDirectoryFactory();
			IPictureDirectory rawDirectory = null;
			bool readyToProcess = true;
			try {
				rawDirectory = pictureDirectoryFactory.GetDirectory(tbRawInputFolder.Text);
				tbRawInputFolder.BackColor = DefaultBackColor;
			} catch (DirectoryNotFoundException ex) {
				tbRawInputFolder.BackColor = Color.Red;

				readyToProcess = false;
				return;
			}
			IPictureDirectory renameDirectory = null;
			try {
				renameDirectory = pictureDirectoryFactory.GetOrCreateDirectory(tbRenamedFolder.Text);
				tbRawInputFolder.BackColor = DefaultBackColor;
			} catch (DirectoryNotFoundException ex) {
				tbRawInputFolder.BackColor = Color.Red;
				readyToProcess = false;
			}

			if (readyToProcess) {
				rawDirectory.RenameAllFiles(renameDirectory);
			}
		}

		private void btnProcessRenamed_Click(object sender, EventArgs e) {
			var directoryFactory = new PictureDirectoryFactory();
			IPictureDirectory renamedDirectory = null;
			bool readyToProcess = true;
			try {
				renamedDirectory = directoryFactory.GetDirectory(tbRenamedFolder.Text);
				tbRenamedFolder.BackColor = DefaultBackColor;
			} catch (DirectoryNotFoundException ex) {
				tbRenamedFolder.BackColor = Color.Red;

				readyToProcess = false;
				return;
			}
			IPictureDirectory resizedDirectory = null;
			try {
				resizedDirectory = directoryFactory.GetOrCreateDirectory(tbResizedFolder.Text);
				tbRawInputFolder.BackColor = DefaultBackColor;
			} catch (DirectoryNotFoundException ex) {
				tbRawInputFolder.BackColor = Color.Red;
				readyToProcess = false;
			}

			if (readyToProcess) {
				renamedDirectory.ResizeAllFiles(resizedDirectory);
			}
		}
	}
}