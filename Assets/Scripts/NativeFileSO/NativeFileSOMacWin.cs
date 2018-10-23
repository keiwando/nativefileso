using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Linq;

namespace Keiwando.NativeFileSO {

	public class NativeFileSOMacWin : INativeFileSO {

#if UNITY_STANDALONE_OSX
		private INativeFileSODesktop nativeFileSO = NativeFileSOMac.shared;
#elif UNITY_STANDALONE_WIN
		private INativeFileSODesktop nativeFileSO = NativeFileSOWindows.shared;
#else
		private INativeFileSODesktop nativeFileSO = null;
#endif

		public static NativeFileSOMacWin shared = new NativeFileSOMacWin();

		private delegate void UnityCallbackPathSelected(bool pathSelected, IntPtr path);

		public event Action<OpenedFile> FileWasOpened;


		private NativeFileSOMacWin() {

			nativeFileSO.FileWasOpened += delegate(OpenedFile file) {
				if (FileWasOpened != null) {
					FileWasOpened(file);
				}
			};
		}

		public void OpenFile(SupportedFileType[] fileTypes) {

			nativeFileSO.OpenFile(fileTypes);
		}

		public void OpenFile(SupportedFileType[] supportedTypes, Action<bool, OpenedFile> onOpen) {
			nativeFileSO.OpenFile(supportedTypes, onOpen);
		}

		public void SaveFile(FileToSave file) {
			nativeFileSO.SaveFile(file);
		}

		// MARK: - Helpers
		public static OpenedFile FileFromPath(string path) {
			byte[] data = File.ReadAllBytes(path);
			var name = Path.GetFileName(path);
			return new OpenedFile(name, data);
		}

		public static void SaveFileToPath(FileToSave file, string path) {
			File.Copy(file.SrcPath, path);
		}
	}
}


