using System;
using System.Runtime.InteropServices;

public class NativeFileSOMobile: INativeFileSO { 

#if UNITY_IOS
	private const string libname = "__Internal";
#elif UNITY_ANDROID
	private const string libname = "";
#else
	private const string libname = "NativeFileSO";
#endif

	[DllImport(libname)]
	private static extern IntPtr _openFile(string extensions);

	[DllImport(libname)]
	private static extern void _saveFile(string srcPath, string name);

	public string OpenFile(string[] extensions) {

		return "";
	}

	public void SaveFile(string srcPath, 
	                     string filename, 
	                     string extension) {

		_saveFile(srcPath, filename);
	}
}
