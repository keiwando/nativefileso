using System;
namespace Keiwando.NFSO {

	/// <summary>
	/// A class used by the custom post-processing build phase in order to 
	/// associate file types with the iOS and Android application.
	/// </summary>
	public static class SupportedFilePreferences {

		/// <summary>
		/// The file types that should be associated with the mobile versions
		/// of the application.
		/// </summary>
		/// <remarks>
		/// Users are going to be able to open files of the file types
		/// included in this array from outside of the application, e.g. using
		/// the native "Share" and "Open in" functionality within Android and iOS.
		/// </remarks>
		public static readonly SupportedFileType[] supportedFileTypes = {

			// Edit this list to include your desired file types

			//SupportedFileType.PDF,
			//SupportedFileType.JPEG,
			//SupportedFileType.PlainText
			//SupportedFileType.Any
		};
	}
}
