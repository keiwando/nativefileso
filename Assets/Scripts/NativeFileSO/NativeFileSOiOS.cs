#define UNITY_IOS
using System;
using System.Runtime.InteropServices;
using System.Linq;

#if UNITY_IOS

namespace Keiwando.NativeFileSO {

	public class NativeFileSOiOS: INativeFileSOMobile {

		public event Action<OpenedFile> FileWasOpened;

		public static NativeFileSOiOS shared = new NativeFileSOiOS();

		[DllImport("__Internal")]
		private static extern void pluginSetCallback(NativeFileSO.UnityCallbackFunction callback);

		[DllImport("__Internal")]
		private static extern Boolean pluginIsFileLoaded();

		[DllImport("__Internal")]
		private static extern IntPtr pluginGetData();

		[DllImport("__Internal")]
		private static extern ulong pluginGetDataByteCount();

		[DllImport("__Internal")]
		private static extern IntPtr pluginGetFilename();

		[DllImport("__Internal")]
		private static extern void pluginResetLoadedFile();

		[DllImport("__Internal")]
		private static extern void pluginOpenFile(string utis);

		[DllImport("__Internal")]
		private static extern void pluginSaveFile(string srcPath, string name);

		private NativeFileSOiOS() {
			pluginSetCallback(NativeFileSOMobile.FileWasOpenedCallback);
		}

		public bool IsFileLoaded() { 
			return pluginIsFileLoaded();
		}

		public OpenedFile GetOpenedFile() {

			if (!IsFileLoaded()) {
				return null;
			}
			
			byte[] byteContents = new byte[pluginGetDataByteCount()];
			Marshal.Copy(pluginGetData(), byteContents, 0, byteContents.Length);
			String filename = Marshal.PtrToStringAnsi(pluginGetFilename());

			pluginResetLoadedFile();

			return new OpenedFile(filename, byteContents);
		}

		public void OpenFile(SupportedFileType[] supportedTypes) { 

			if (supportedTypes != null && supportedTypes.Length > 0) {
				string encodedUTIs = EncodeUTIs(supportedTypes.Select(x => x.AppleUTI).ToArray());
				pluginOpenFile(encodedUTIs);
			} else {
				pluginOpenFile(SupportedFileType.Any.AppleUTI);
			}
		}

		public void SaveFile(FileToSave file) { 
			pluginSaveFile(file.SrcPath, file.Name);
		}

		public void LoadIfTemporaryFileAvailable() {}

		private string EncodeUTIs(string[] extensions) {

			return string.Join("%", extensions);
		}
	}
}

#endif