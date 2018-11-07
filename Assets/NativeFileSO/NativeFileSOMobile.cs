using System;
using AOT;
using UnityEngine;

namespace Keiwando.NativeFileSO {

	/// <summary>
	/// Provides methods for native file open and share functionality on 
	/// Android and iOS.
	/// </summary>
	/// <remarks>
	/// See <see cref="NativeFileSO"/> for a more in-depth overview of how to
	/// use the individual methods.
	/// </remarks>
	public class NativeFileSOMobile : INativeFileSO {

#if UNITY_IOS
		private INativeFileSOMobile nativeFileSO = NativeFileSOiOS.shared;
#elif UNITY_ANDROID
		private INativeFileSOMobile nativeFileSO = NativeFileSOAndroid.shared;
#else
		private INativeFileSOMobile nativeFileSO = null;
#endif

		public static NativeFileSOMobile shared = new NativeFileSOMobile();

		/// <summary>
		/// Occurs when files were opened without the user being manually 
		/// prompted to open files from within the app. This occurs if certain
		/// file types have been associated with the application.
		/// 
		/// See <see cref="SupportedFilePreferences"/> for more information on
		/// file associations.
		/// </summary>
		public event Action<OpenedFile[]> FilesWereOpened;

		private Action<bool, OpenedFile[]> _callback;
		private bool isBusy = false;

		private NativeFileSOMobile() {

#if !UNITY_EDITOR

			NativeFileSOUnityEvent.UnityReceivedControl += delegate {
				TryRetrieveOpenedFile();
				isBusy = false;
			};
#endif
		}

		public void OpenFile(SupportedFileType[] supportedTypes, Action<bool, OpenedFile> onCompletion) {

			if (isBusy) return;
			isBusy = true;

			_callback = delegate (bool wasOpened, OpenedFile[] openedFiles) {
				if (onCompletion != null) {
					onCompletion(wasOpened, wasOpened ? openedFiles[0] : null);
				}
			};
			nativeFileSO.OpenFiles(supportedTypes, false);
		}

		public void OpenFiles(SupportedFileType[] supportedTypes, Action<bool, OpenedFile[]> onCompletion) {

			if (isBusy) return;
			isBusy = true;
			_callback = onCompletion;

			nativeFileSO.OpenFiles(supportedTypes, true);
		}

		public void SaveFile(FileToSave file) {

			if (isBusy) return;

			isBusy = true;
			nativeFileSO.SaveFile(file);
		}

		private void TryRetrieveOpenedFile() {

			if (nativeFileSO == null) return;

			var files = nativeFileSO.GetOpenedFiles();
			SendFileOpenedEvent(files.Length > 0, files);
		}

		private void SendFileOpenedEvent(bool fileWasOpened, OpenedFile[] file) {
			
			if (_callback != null) {
				_callback(fileWasOpened, file);
				_callback = null;
				return;
			}

			if (fileWasOpened && FilesWereOpened != null) {
				FilesWereOpened(file);
			}
		}

#if UNITY_IOS
		[MonoPInvokeCallback(typeof(NativeFileSOiOS.UnityCallbackFunction))]
		internal static void FileWasOpenedCallback() {
			shared.TryRetrieveOpenedFile();
			shared.isBusy = false;
		}
#endif
	}
}


