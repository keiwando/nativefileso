using System;
namespace Keiwando.NativeFileSO {

	public static class SupportedFilePreferences {

		/// <summary>
		/// The file types that the application is capable of opening.
		/// </summary>
		public static readonly SupportedFileType[] supportedFileTypes = new SupportedFileType[] {
			


			// TODO: Remove from final build
			//CustomFileTypes.creat,
			//CustomFileTypes.evol,

			//SupportedFileType.PDF,
			//SupportedFileType.JPEG

			//,
			SupportedFileType.Any
		};
	}
}
