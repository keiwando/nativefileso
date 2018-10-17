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

		public string TryOpenURL() {

#if UNITY_IOS
			return Marshal.PtrToStringAnsi(pluginGetOpenURL());
#elif UNITY_ANDROID

			AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

			AndroidJavaClass nativeFileSO = new AndroidJavaClass("com.keiwando.lib_nativefileso.NativeFileSO");

			return nativeFileSO.CallStatic<string>("GetFileContents");
#endif
		}

		public string OpenFile(string[] extensions) {

			AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

			AndroidJavaClass nativeFileSO = new AndroidJavaClass("com.keiwando.lib_nativefileso.NativeFileSO");

			if (extensions == null || extensions.Length == 0) {
				nativeFileSO.CallStatic("OpenFile", currentActivity, "*");
			} else {
				nativeFileSO.CallStatic("OpenFile", currentActivity, extensions[0]);
			}

			return "";
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


