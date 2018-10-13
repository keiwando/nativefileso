using System;
using System.IO;
using UnityEngine;

public class NativeFileSO: INativeFileSO {

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
	private static INativeFileSO nativeFileSO = new NativeFileSOMacWin();
#elif UNITY_IOS || UNITY_ANDROID
	private static INativeFileSO nativeFileSO = new NativeFileSOMobile();
#else
	private static INativeFileSO nativeFileSO = null;
#endif

	public static readonly NativeFileSO shared = new NativeFileSO();

	public string OpenFile(string[] extensions) {
		
		var path = nativeFileSO.OpenFile(extensions);

		Debug.Log("Path : " + path);

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
