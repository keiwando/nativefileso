using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Keiwando.NativeFileSO {

	public class NativeFileSOMobile : INativeFileSO {

#if UNITY_IOS

		[DllImport("__Internal")]
		private static extern IntPtr pluginOpenFile(string extensions);

		[DllImport("__Internal")]
		private static extern void pluginSaveFile(string srcPath, string name);

		[DllImport("__Internal")]
		private static extern IntPtr pluginGetOpenURL();

#elif UNITY_ANDROID

#endif

		public event Action<OpenedFile> FileWasOpened;

		public NativeFileSOMobile() {

			//Debug.Log("Registering callback on " + NativeFileSOMobileCallback.instance);

			NativeFileSOMobileCallback.instance.FileWasOpened += delegate (OpenedFile file) {
				if (FileWasOpened != null) { 
					FileWasOpened(file);
				}
				Debug.Log("File Was Opened - SOMobile");
			};
		}

		public bool LoadIfTemporaryAvailable() {
#if UNITY_IOS
			return false;
#elif UNITY_ANDROID
			AndroidJavaClass nativeFileSO = new AndroidJavaClass("com.keiwando.lib_nativefileso.NativeFileSO");
			var isAvailable = nativeFileSO.CallStatic<bool>("IsTemporaryFileAvailable");

			if (isAvailable) {
				nativeFileSO.CallStatic("LoadTemporaryFile");
			}
			Debug.Log("Is Temporary File available: " + isAvailable);

			return isAvailable;
#endif
		}

		public bool IsFileOpened() {
#if UNITY_IOS
			return true;
#elif UNITY_ANDROID

			AndroidJavaClass nativeFileSO = new AndroidJavaClass("com.keiwando.lib_nativefileso.NativeFileSO");
			return nativeFileSO.CallStatic<bool>("IsFileLoaded");
#endif
		}

		public OpenedFile GetOpenedFile() {

#if UNITY_IOS
			return Marshal.PtrToStringAnsi(pluginGetOpenURL());
#elif UNITY_ANDROID

			AndroidJavaClass nativeFileSO = new AndroidJavaClass("com.keiwando.lib_nativefileso.NativeFileSO");

			string textContents = nativeFileSO.CallStatic<string>("GetFileTextContents");
			byte[] byteContents = nativeFileSO.CallStatic<byte[]>("GetFileByteContents");
			bool isTextFile = nativeFileSO.CallStatic<bool>("IsTextFile");
			string filename = nativeFileSO.CallStatic<string>("GetFileName");
			string extension = nativeFileSO.CallStatic<string>("GetFileExtension");

			// Reset the loaded data
			nativeFileSO.CallStatic("ResetLoadedFile");

			return new OpenedFile {
				name = filename,
				extension = extension,
				isTextFile = isTextFile,
				stringContents = textContents,
				data = byteContents
			};
#endif
		}

		public void OpenFile(string[] extensions) {

			AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

			AndroidJavaClass nativeFileSO = new AndroidJavaClass("com.keiwando.lib_nativefileso.NativeFileSO");

			if (extensions == null || extensions.Length == 0) {
				nativeFileSO.CallStatic("OpenFile", currentActivity, "*");
			} else {
				nativeFileSO.CallStatic("OpenFile", currentActivity, extensions[0]);
			}
		}

		public void SaveFile(string srcPath,
							 string filename,
							 string extension) {

#if UNITY_IOS

			pluginSaveFile(srcPath, filename);

#elif UNITY_ANDROID

		AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

		AndroidJavaClass nativeFileSO = new AndroidJavaClass("com.keiwando.lib_nativefileso.NativeFileSO");

		nativeFileSO.CallStatic("SaveFile", currentActivity, srcPath);
#endif
		}


	}
}


