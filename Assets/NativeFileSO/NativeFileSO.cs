using System;
using System.IO;
using UnityEngine;

namespace Keiwando.NativeFileSO { 

	/// <summary>
	/// 
	/// </summary>
	public class NativeFileSO : INativeFileSO {

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR
		private static INativeFileSO nativeFileSO = NativeFileSOMacWin.shared;
#elif UNITY_IOS || UNITY_ANDROID
		private static INativeFileSO nativeFileSO = NativeFileSOMobile.shared;
#else
		private static INativeFileSO nativeFileSO = null;
#endif

		public static readonly NativeFileSO shared = new NativeFileSO();

		private NativeFileSO() {}

		public void OpenFile(SupportedFileType[] supportedTypes, Action<bool, OpenedFile> onCompletion) {
			nativeFileSO.OpenFile(supportedTypes, onCompletion);
		}

		public void OpenFiles(SupportedFileType[] supportedTypes, Action<bool, OpenedFile[]> onCompletion) {
			nativeFileSO.OpenFiles(supportedTypes, onCompletion);
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

		public void SaveFile(string srcPath) {
			nativeFileSO.SaveFile(new FileToSave(srcPath));
		}
	}
}


