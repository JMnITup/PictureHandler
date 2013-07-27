// /*
// JamesM
// 2013 06 13 9:32 PM
// 2013 06 14 12:09 PM
// Program.cs
// PictureHandler
// PictureHandler
// */

#region

using System;
using System.Windows.Forms;

#endregion

namespace PictureHandler {
	internal static class Program {
		/// <summary>
		///   The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());
		}
	}
}