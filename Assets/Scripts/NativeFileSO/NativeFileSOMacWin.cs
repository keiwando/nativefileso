using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public class NativeFileSOMacWin : INativeFileSO {

#if UNITY_STANDALONE_OSX
	private const string libname = "NativeFileSOMac";
#elif UNITY_STANDALONE_WIN
	private const string libname = "";
#else
	private const string libname = "";
#endif

	[DllImport(libname)]
	private static extern IntPtr _openFile();

	[DllImport(libname)]
	private static extern IntPtr _saveFile();

	public void OpenFile() { 

		var pathPtr = _openFile();
		var path = Marshal.PtrToStringAnsi(pathPtr);
		Debug.Log("Path : " + path);

		string contents = File.ReadAllText(path);
		Debug.Log(contents);
	}

	public void SaveFile(string srcPath) { 

		var pathPtr = _saveFile();
		var path = Marshal.PtrToStringAnsi(pathPtr);
		Debug.Log("Path : " + path);

		File.Copy(srcPath, path);
	}
}
