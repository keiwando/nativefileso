using System;
using System.IO;
using UnityEngine;

namespace Keiwando.NativeFileSO { 

	public class NativeFileSO : INativeFileSO {

		public delegate void UnityCallbackFunction();

		public event Action<OpenedFile> FileWasOpened;

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
		private static INativeFileSO nativeFileSO = NativeFileSOMacWin.shared;
#elif UNITY_IOS || UNITY_ANDROID
		private static INativeFileSO nativeFileSO = new NativeFileSOMobile();
#else
	private static INativeFileSO nativeFileSO = null;
#endif

		public static readonly NativeFileSO shared = new NativeFileSO();

		private NativeFileSO() {

			nativeFileSO.FileWasOpened += OnFileOpened;

		}

		public void OpenFile(SupportedFileType[] supportedTypes) {

			nativeFileSO.OpenFile(supportedTypes);
		}

		public void OpenFile(SupportedFileType[] supportedTypes, Action<bool, OpenedFile> onOpen) {

		}

		public void SaveFile(FileToSave file) {
			nativeFileSO.SaveFile(file);
		}

		public void SaveFile(string srcPath,
							 string filename,
							 string extension) {

			var file = new FileToSave(srcPath, string.Format("{0}.{1}", filename, extension), extension);

			nativeFileSO.SaveFile(file);
		}

		private void OnFileOpened(OpenedFile file) {
			Debug.Log("OnFileOpened");

			if (FileWasOpened != null) {
				FileWasOpened(file);
			}
		}
	}
}


