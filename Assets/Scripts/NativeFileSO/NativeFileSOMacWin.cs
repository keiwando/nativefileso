using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Linq;

namespace Keiwando.NativeFileSO { 

	public class NativeFileSOMacWin : INativeFileSO {

		private delegate void UnityCallbackPathSelected(string path);

#if UNITY_STANDALONE_OSX
	private const string libname = "NativeFileSOMac";
#elif UNITY_STANDALONE_WIN
	private const string libname = "";
#else
		private const string libname = "NativeFileSO";
#endif

		[DllImport(libname)]
		private static extern void pluginSetCallback(NativeFileSO.UnityCallbackFunction callback);

		[DllImport(libname)]
		private static extern IntPtr pluginOpenFile(string extensions);

		[DllImport(libname)]
		private static extern IntPtr pluginSaveFile(string name, string extension);


		public event Action<OpenedFile> FileWasOpened;

		public void OpenFile(SupportedFileType[] fileTypes) {

			var extensions = fileTypes.Select(x => x.Extension).ToArray();

			var pathPtr = pluginOpenFile(EncodeExtensions(extensions));
			var path = Marshal.PtrToStringAnsi(pathPtr);

			Debug.Log("Path : " + path);

			if (path == "" || FileWasOpened == null) return;

			byte[] data = File.ReadAllBytes(path);

			var name = Path.GetFileName(path);

			var file = new OpenedFile(name, data);

			FileWasOpened(file);
		}

		public void SaveFile(FileToSave file) {

			var pathPtr = pluginSaveFile(file.Name, file.Extension);
			var path = Marshal.PtrToStringAnsi(pathPtr);

			Debug.Log("Save Path : " + path);

			File.Copy(file.SrcPath, path);
		}

		private static void PluginDidOpenFile() { 
			
		}

		// MARK: - Helpers
		private string EncodeExtensions(string[] extensions) {

			return string.Join("%", extensions);
		}
	}
}


