using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Linq;

namespace Keiwando.NativeFileSO {

	public class NativeFileSOMacWin : INativeFileSO {

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
		private static extern void pluginSaveFile(string name, string extension);


		public static NativeFileSOMacWin shared = new NativeFileSOMacWin();

		private delegate void UnityCallbackPathSelected(bool pathSelected, IntPtr path);

		public event Action<OpenedFile> FileWasOpened;

		private Action<bool, OpenedFile> _callback;
		private bool isBusy = false;
		private FileToSave _cachedFileToSave;


		private NativeFileSOMacWin() { 
			NativeFileSOUnityEvent.UnityReceivedControl += delegate {
				SendFileOpenedEvent(false, null);
				isBusy = false;
				_cachedFileToSave = null;
			};
		}

		public void OpenFile(SupportedFileType[] fileTypes) {

			if (isBusy) return;
			isBusy = true;

			var extensions = fileTypes.Select(x => x.Extension).ToArray();

#if UNITY_STANDALONE_OSX
			pluginSetCallback(DidSelectPathForOpen);
#endif
			pluginOpenFile(EncodeExtensions(extensions));
		}

		public void OpenFile(SupportedFileType[] supportedTypes, Action<bool, OpenedFile> onOpen) {

			if (isBusy) return;

			_callback = onOpen;
			OpenFile(supportedTypes);
		}

		public void SaveFile(FileToSave file) {

			if (isBusy) return;
			isBusy = true;

#if UNITY_STANDALONE_OSX
			pluginSetCallback(DidSelectPathForSave);
#endif

			_cachedFileToSave = file;
			pluginSaveFile(file.Name, file.Extension);
		}

		private static void DidSelectPathForOpen(bool pathSelected, IntPtr pathPtr) { 

			if (!pathSelected) {
				shared.SelectionWasCancelled();
				return;
			}

			var path = Marshal.PtrToStringAnsi(pathPtr);

			Debug.Log("Path : " + path);

			byte[] data = File.ReadAllBytes(path);
			var name = Path.GetFileName(path);
			var file = new OpenedFile(name, data);

			shared.SendFileOpenedEvent(true, file);
		}

		private static void DidSelectPathForSave(bool pathSelected, IntPtr pathPtr) {

			if (!pathSelected) {
				shared.SelectionWasCancelled();
				return;
			}

			var toSave = shared._cachedFileToSave;
			if (toSave == null) return;

			var path = Marshal.PtrToStringAnsi(pathPtr);

			Debug.Log("Save Path : " + path);

			File.Copy(toSave.SrcPath, path);
		}

		private void SelectionWasCancelled() { 
			
			isBusy = false;
			_cachedFileToSave = null;
			SendFileOpenedEvent(false, null);
		}

		private void SendFileOpenedEvent(bool fileWasOpened, OpenedFile file) {

			if (_callback != null) {
				_callback(fileWasOpened, file);
				return;
			}

			if (fileWasOpened && FileWasOpened != null) {
				FileWasOpened(file);
			}
			isBusy = false;
		}

		// MARK: - Helpers
		private string EncodeExtensions(string[] extensions) {

			return string.Join("%", extensions);
		}
	}
}


