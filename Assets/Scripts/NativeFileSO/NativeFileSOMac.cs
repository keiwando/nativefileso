#if UNITY_STANDALONE_OSX
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;
using UnityEngine;

namespace Keiwando.NativeFileSO {

	public class NativeFileSOMac: INativeFileSODesktop {

		[DllImport("NativeFileSOMac")]
		private static extern void pluginSetCallback(UnityCallbackPathSelected callback);

		[DllImport("NativeFileSOMac")]
		private static extern void pluginOpenFile(string extensions);

		[DllImport("NativeFileSOMac")]
		private static extern void pluginSaveFile(string name, string extension);

		private delegate void UnityCallbackPathSelected(bool pathSelected, IntPtr path);

		public static NativeFileSOMac shared = new NativeFileSOMac();

		public event Action<OpenedFile> FileWasOpened;

		private Action<bool, OpenedFile> _callback;
		private bool isBusy = false;
		private FileToSave _cachedFileToSave;

		private NativeFileSOMac() {
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

			pluginSetCallback(DidSelectPathForOpen);
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

			pluginSetCallback(DidSelectPathForSave);

			_cachedFileToSave = file;
			pluginSaveFile(file.Name, file.Extension);
		}

		// MARK: - INativeFileSODesktop

		public void OpenFiles(SupportedFileType[] fileTypes, bool canSelectMultiple,
					   string title, string directory, 
					   Action<bool, OpenedFile[]> onCompletion) {

		}

		public OpenedFile[] OpenFilesSync(SupportedFileType[] fileTypes, bool canSelectMultiple, 
								   string title, string directory) {

		}

		public void SelectOpenPaths(SupportedFileType[] fileTypes, bool canSelectMultiple,
							 string title, string directory,
							 Action<bool, string[]> onCompletion) {

		}

		public string[] SelectOpenPathsSync(SupportedFileType[] fileTypes, bool canSelectMultiple,
									 string title, string directory) {

		}

		public void SaveFile(FileToSave file, string title, string directory) {
			// TODO: Implement
		}

		// MARK: - Private Functions

		private static void DidSelectPathForOpen(bool pathSelected, IntPtr pathPtr) {

			if (!pathSelected) {
				shared.SelectionWasCancelled();
				return;
			}

			var path = Marshal.PtrToStringAnsi(pathPtr);

			Debug.Log("Path : " + path);

			shared.SendFileOpenedEvent(true, NativeFileSOMacWin.FileFromPath(path));
			shared.isBusy = false;
		}

		private static void DidSelectPathForSave(bool pathSelected, IntPtr pathPtr) {

			if (!pathSelected) {
				shared.SelectionWasCancelled();
				return;
			}

			shared.isBusy = false;

			var toSave = shared._cachedFileToSave;
			if (toSave == null) return;

			var path = Marshal.PtrToStringAnsi(pathPtr);

			Debug.Log("Save Path : " + path);

			NativeFileSOMacWin.SaveFileToPath(toSave, path);
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
#endif