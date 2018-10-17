using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Keiwando.NativeFileSO { 

	public class NativeFileSOMacWin : INativeFileSO {

#if UNITY_STANDALONE_OSX
	private const string libname = "NativeFileSOMac";
#elif UNITY_STANDALONE_WIN
	private const string libname = "";
#else
		private const string libname = "NativeFileSO";
#endif

		[DllImport(libname)]
		private static extern IntPtr _openFile(string extensions);

		[DllImport(libname)]
		private static extern IntPtr _saveFile(string name, string extension);

		public string OpenFile(string[] extensions) {

			var pathPtr = _openFile(EncodeExtensions(extensions));
			return Marshal.PtrToStringAnsi(pathPtr);
		}

		public void SaveFile(string srcPath,
							 string filename,
							 string extension) {

			var pathPtr = _saveFile(filename, extension);
			var path = Marshal.PtrToStringAnsi(pathPtr);

			Debug.Log("Save Path : " + path);

			File.Copy(srcPath, path);
		}

		// MARK: - Helpers
		private string EncodeExtensions(string[] extensions) {

			return string.Join("%", extensions);
		}
	}
}


