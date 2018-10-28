//#define UNITY_IOS
using System;
using System.Runtime.InteropServices;
using System.Linq;
using UnityEngine;

#if UNITY_IOS

namespace Keiwando.NativeFileSO {

	public class NativeFileSOiOS: INativeFileSOMobile {

		[StructLayout(LayoutKind.Sequential)]
		private struct NativeOpenedFile {
			public IntPtr filename;
			public IntPtr data;
			public int dataLength;
		}

		public static NativeFileSOiOS shared = new NativeFileSOiOS();

		[DllImport("__Internal")]
		private static extern void pluginSetCallback(NativeFileSO.UnityCallbackFunction callback);

		[DllImport("__Internal")]
		private static extern int pluginGetNumberOfOpenedFiles();

		[DllImport("__Internal")]
		private static extern NativeOpenedFile pluginGetOpenedFileAtIndex(int i);

		[DllImport("__Internal")]
		private static extern void pluginResetLoadedFile();

		[DllImport("__Internal")]
		private static extern void pluginOpenFile(string utis, bool canSelectMultiple);

		[DllImport("__Internal")]
		private static extern void pluginSaveFile(string srcPath, string name);

		private static OpenedFile[] _noFiles = new OpenedFile[0];

		private NativeFileSOiOS() {
			pluginSetCallback(NativeFileSOMobile.FileWasOpenedCallback);
		}

		public OpenedFile[] GetOpenedFiles() {

			var numOfLoadedFiles = pluginGetNumberOfOpenedFiles();
			if (numOfLoadedFiles == 0) return _noFiles;

			Debug.Log(string.Format("Files loaded: {0}", numOfLoadedFiles));

			var files = new OpenedFile[numOfLoadedFiles];
			for (int i = 0; i < numOfLoadedFiles; i++) {
				Debug.Log(string.Format("Current index: {0}", i));
				var nativeOpenedFile = pluginGetOpenedFileAtIndex(i);

				byte[] byteContents = new byte[nativeOpenedFile.dataLength];
				Marshal.Copy(nativeOpenedFile.data, byteContents, 0, byteContents.Length);
				string filename = Marshal.PtrToStringAnsi(nativeOpenedFile.filename);

				files[i] = new OpenedFile(filename, byteContents);
			}

			pluginResetLoadedFile();

			return files;
		}

		public void OpenFiles(SupportedFileType[] supportedTypes, bool canSelectMultiple) {

			var encodedUTIs = SupportedFileType.Any.AppleUTI;

			if (supportedTypes != null && supportedTypes.Length > 0) {
				encodedUTIs = EncodeUTIs(supportedTypes.Select(x => x.AppleUTI).ToArray());
			}

			pluginOpenFile(encodedUTIs, canSelectMultiple);
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