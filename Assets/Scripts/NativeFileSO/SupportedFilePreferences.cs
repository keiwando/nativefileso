using System;
namespace Keiwando.NativeFileSO {

	public static class SupportedFilePreferences {

		/// <summary>
		/// The file types that the application is capable of opening.
		/// </summary>
		public static readonly SupportedFileType[] supportedFileTypes = new SupportedFileType[] {
			
			SupportedFileType.PlainText,

			// TODO: Remove from final build
			CustomFileTypes.creat,
			CustomFileTypes.evol
		};
	}
}
