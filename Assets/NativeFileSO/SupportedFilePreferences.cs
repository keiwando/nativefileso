using System;
namespace Keiwando.NativeFileSO {

	public static class SupportedFilePreferences {

		/// <summary>
		/// The file types that the application is capable of opening.
		/// </summary>
		public static readonly SupportedFileType[] supportedFileTypes = {
			


			// TODO: Remove from final build
			Demo.CustomFileTypes.creat,
			Demo.CustomFileTypes.evol,

			SupportedFileType.PDF,
			SupportedFileType.JPEG

			//,
			//SupportedFileType.Any
		};
	}
}
