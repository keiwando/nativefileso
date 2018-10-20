//#define UNITY_IOS
//#undef UNITY_ANDROID


using System;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Keiwando.NativeFileSO {

	public class NativeFileSOMobile : INativeFileSO {


#if UNITY_IOS

		[DllImport("__Internal")]
		private static extern void pluginSetCallback(NativeFileSOMobileCallback.UnityCallbackFunction callback);

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

#if UNITY_IOS
			pluginSetCallback(NativeFileSOMobileCallback.IOSFileWasOpened);
#endif
		}

		public bool LoadIfTemporaryAvailable() {
#if UNITY_IOS
			return pluginIsFileLoaded();
#elif UNITY_ANDROID

			AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaClass nativeFileSO = new AndroidJavaClass("com.keiwando.lib_nativefileso.NativeFileSO");
			AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			var isAvailable = nativeFileSO.CallStatic<bool>("IsTemporaryFileAvailable", currentActivity);

			if (isAvailable) {
				nativeFileSO.CallStatic("LoadTemporaryFile", currentActivity);
			}
			Debug.Log("Is Temporary File available: " + isAvailable);

			return isAvailable;
#endif
		}

		public bool IsFileOpened() {
#if UNITY_IOS
			return pluginIsFileLoaded();
#elif UNITY_ANDROID

			AndroidJavaClass nativeFileSO = new AndroidJavaClass("com.keiwando.lib_nativefileso.NativeFileSO");
			return nativeFileSO.CallStatic<bool>("IsFileLoaded");
#endif
		}

		public OpenedFile GetOpenedFile() {

#if UNITY_IOS
			//string textContents = Marshal.PtrToStringAnsi(pluginGetStringContents());
			byte[] byteContents = new byte[pluginGetDataByteCount()];
			Marshal.Copy(pluginGetData(), byteContents, 0, byteContents.Length);
			//bool isTextFile = pluginIsTextFile();
			String filename = Marshal.PtrToStringAnsi(pluginGetFilename());
			//String extension = Marshal.PtrToStringAnsi(pluginGetExtension());

			pluginResetLoadedFile();

			return new OpenedFile(filename, byteContents);

#elif UNITY_ANDROID

			AndroidJavaClass nativeFileSO = new AndroidJavaClass("com.keiwando.lib_nativefileso.NativeFileSO");

			byte[] byteContents = nativeFileSO.CallStatic<byte[]>("GetFileByteContents");
			string filename = nativeFileSO.CallStatic<string>("GetFileName");

			// Reset the loaded data
			nativeFileSO.CallStatic("ResetLoadedFile");

			return new OpenedFile(filename, byteContents);
#endif
		}

		public void OpenFile(SupportedFileType[] supportedTypes) {

#if UNITY_IOS
			if (supportedTypes != null && supportedTypes.Length > 0) {
				string encodedUTIs = EncodeUTIs(supportedTypes.Select(x => x.AppleUTI).ToArray());
				pluginOpenFile(encodedUTIs);
			} else {
				pluginOpenFile(SupportedFileType.Any.AppleUTI);
			}

#elif UNITY_ANDROID

			string encodedMimeTypes = EncodeMimeTypes(supportedTypes.Select(x => x.MimeType).ToArray());

			AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

			AndroidJavaClass nativeFileSO = new AndroidJavaClass("com.keiwando.lib_nativefileso.NativeFileSO");

			if (supportedTypes == null || supportedTypes.Length == 0) {
				nativeFileSO.CallStatic("OpenFile", currentActivity, SupportedFileType.Any.MimeType);
			} else {
				nativeFileSO.CallStatic("OpenFile", currentActivity, encodedMimeTypes);
			}
#endif
		}

		public void SaveFile(FileToSave file) {

#if UNITY_IOS

			pluginSaveFile(file.SrcPath, file.Name);

#elif UNITY_ANDROID

			AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
			AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

			AndroidJavaClass nativeFileSO = new AndroidJavaClass("com.keiwando.lib_nativefileso.NativeFileSO");

			nativeFileSO.CallStatic("SaveFile", currentActivity, file.SrcPath, file.MimeType);
#endif
		}


		private string EncodeUTIs(string[] extensions) {

			return string.Join("%", extensions);
		}

		private string EncodeMimeTypes(string[] extensions) {
			return string.Join(" ", extensions);
		}
	}
}


