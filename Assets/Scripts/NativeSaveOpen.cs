
using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class NativeSaveOpen {

#if UNITY_STANDALONE_OSX
	private const string libname = "UnityNativeFileImportExport";
#else
	private static string libname = ";
#endif

	[DllImport(libname)]
	private static extern IntPtr _openFile();

	[DllImport(libname)]
	private static extern IntPtr _saveFile();

	[DllImport(libname)]
	private static extern void test();


	public static void OpenFile() {

#if UNITY_STANDALONE_OSX //&& !UNITY_EDITOR

		var pathPtr = _openFile();
		var path = Marshal.PtrToStringAnsi(pathPtr);
		Debug.Log("Path : " + path);
		//test();
#endif
	}

	public static void SaveFile() {

#if UNITY_STANDALONE_OSX //&& !UNITY_EDITOR

		var pathPtr = _saveFile();
		var path = Marshal.PtrToStringAnsi(pathPtr);
		Debug.Log("Path : " + path);
		//test();
#endif
	}
}

