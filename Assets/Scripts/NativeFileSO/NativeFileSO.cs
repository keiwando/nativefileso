using System;
using System.IO;
using UnityEngine;

namespace Keiwando.NativeFileSO { 

	public class NativeFileSO : INativeFileSO {

		public event Action<string> FileWasOpened;

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
	private static INativeFileSO nativeFileSO = new NativeFileSOMacWin();
#elif UNITY_IOS || UNITY_ANDROID
		private static INativeFileSO nativeFileSO = new NativeFileSOMobile();
#else
	private static INativeFileSO nativeFileSO = null;
#endif

		public static readonly NativeFileSO shared = new NativeFileSO();

		private NativeFileSO() {
			NativeFileSOMobileCallback.instance.FileWasOpened +=
				delegate (object o, string contents) {

					if (FileWasOpened != null) {
						FileWasOpened(contents);
					}
				};
		}

		public string OpenFile(string[] extensions) {

			var path = nativeFileSO.OpenFile(extensions);

			Debug.Log("Path : " + path);

			if (path == "") return "";

			string contents = File.ReadAllText(path);
			Debug.Log(contents);

			return path;
		}

		public void SaveFile(string srcPath,
							 string filename,
							 string extension) {

			nativeFileSO.SaveFile(srcPath, filename, extension);
		}
	}
}


