using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Linq;

namespace Keiwando.NativeFileSO {

	public class NativeFileSOMacWin : INativeFileSO {

		public static NativeFileSOMacWin shared = new NativeFileSOMacWin();

		private delegate void UnityCallbackPathSelected(IntPtr path);

#if UNITY_STANDALONE_OSX
		private const string libname = "NativeFileSOMac";
#elif UNITY_STANDALONE_WIN
	private const string libname = "";
#else
		private const string libname = "NativeFileSO";
#endif

#if UNITY_STANDALONE_OSX
		[DllImport(libname)]
		private static extern void pluginSetCallback(UnityCallbackPathSelected callback);
#endif

		[DllImport(libname)]
		private static extern void pluginOpenFile(string extensions);

		[DllImport(libname)]
		private static extern IntPtr pluginSaveFile(string name, string extension);


		public event Action<OpenedFile> FileWasOpened;

		private NativeFileSOMacWin() { }

		public void OpenFile(SupportedFileType[] fileTypes) {

			var extensions = fileTypes.Select(x => x.Extension).ToArray();

#if UNITY_STANDALONE_OSX
			pluginSetCallback(DidSelectPathForOpen);
#endif
			pluginOpenFile(EncodeExtensions(extensions));
		}

		public void OpenFile(SupportedFileType[] supportedTypes, Action<bool, OpenedFile> onOpen) {

		}

		public void SaveFile(FileToSave file) {

			var pathPtr = pluginSaveFile(file.Name, file.Extension);
			var path = Marshal.PtrToStringAnsi(pathPtr);

			Debug.Log("Save Path : " + path);

			File.Copy(file.SrcPath, path);
		}

		private static void DidSelectPathForOpen(IntPtr pathPtr) { 

			var path = Marshal.PtrToStringAnsi(pathPtr);

			Debug.Log("Path : " + path);

			if (path == "" || FileWasOpened == null) return;

			byte[] data = File.ReadAllBytes(path);

			var name = Path.GetFileName(path);

			var file = new OpenedFile(name, data);

			FileWasOpened(file);
		}

		private static void DidSelectPathForSave(IntPtr pathPtr) { 


		}

		// MARK: - Helpers
		private string EncodeExtensions(string[] extensions) {

			return string.Join("%", extensions);
		}
	}
}


