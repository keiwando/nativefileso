using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class NativeFileSOMobile : INativeFileSO {

#if UNITY_IOS

	[DllImport("__Internal")]
	private static extern IntPtr _openFile(string extensions);

	[DllImport("__Internal")]
	private static extern void _saveFile(string srcPath, string name);

#elif UNITY_ANDROID

#endif



	public string OpenFile(string[] extensions) {

		return "";
	}

	public void SaveFile(string srcPath,
						 string filename,
						 string extension) {

#if UNITY_IOS

		_saveFile(srcPath, filename);

#elif UNITY_ANDROID

		AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

		AndroidJavaClass nativeFileSO = new AndroidJavaClass("com.keiwando.lib_nativefileso.NativeFileSO");

		nativeFileSO.CallStatic("SaveFile", currentActivity, srcPath);
#endif
	}
}
