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


		public event Action<OpenedFile> FileWasOpened;

		public void OpenFile(string[] extensions) {

			var pathPtr = _openFile(EncodeExtensions(extensions));
			var path = Marshal.PtrToStringAnsi(pathPtr);

			Debug.Log("Path : " + path);

			if (path == "" || FileWasOpened == null) return;

			byte[] data = File.ReadAllBytes(path);

			string contents = "";
			bool isTextFile = true;
			try {
				contents = File.ReadAllText(path);
				isTextFile = false;
			} catch (Exception e) {
				Debug.Log(e.StackTrace);
			}

			var name = Path.GetFileName(path);
			var extension = Path.GetExtension(path);

			var file = new OpenedFile {
				name = name,
				extension = extension,
				isTextFile = isTextFile,
				stringContents = contents,
				data = data
			};

			FileWasOpened(file);
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


