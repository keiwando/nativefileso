//#define UNITY_IOS
//#undef UNITY_ANDROID


using System;
using AOT;

namespace Keiwando.NativeFileSO {

	public class NativeFileSOMobile: INativeFileSO {

		#if UNITY_IOS
		private INativeFileSOMobile nativeFileSO = NativeFileSOiOS.shared;
#elif UNITY_ANDROID
		private INativeFileSOMobile nativeFileSO = NativeFileSOAndroid.shared;
#else
		private INativeFileSOMobile nativeFileSO = null;
#endif

		public static NativeFileSOMobile shared = new NativeFileSOMobile();

		public event Action<OpenedFile> FileWasOpened;

		private Action<bool, OpenedFile> _callback;
		private bool isBusy = false;

		private NativeFileSOMobile() {

			//Debug.Log("Registering callback on " + NativeFileSOMobileCallback.instance);

			NativeFileSOUnityEvent.UnityReceivedControl += delegate {
				TryRetrieveOpenedFile();
				isBusy = false;
			};
		}


		public void OpenFile(SupportedFileType[] supportedTypes) {

			if (isBusy) return;

			isBusy = true;
			nativeFileSO.OpenFile(supportedTypes);
		}

		public void OpenFile(SupportedFileType[] supportedTypes, Action<bool, OpenedFile> onOpen) {

			if (isBusy) return;

			_callback = onOpen;
			OpenFile(supportedTypes);
		}

		public void SaveFile(FileToSave file) {

			if (isBusy) return;

			isBusy = true;
			nativeFileSO.SaveFile(file);
		}

		private void TryRetrieveOpenedFile() {

			nativeFileSO.LoadIfTemporaryFileAvailable();

			if (nativeFileSO.IsFileLoaded()) {

				var file = nativeFileSO.GetOpenedFile();

				SendFileOpenedEvent(true, file);
			} else {
				SendFileOpenedEvent(false, null);
			}
		}

		private void SendFileOpenedEvent(bool fileWasOpened, OpenedFile file) {

			if (_callback != null) {
				_callback(fileWasOpened, file);
				_callback = null;
				return;
			}

			if (fileWasOpened && FileWasOpened != null) {
				FileWasOpened(file);
			}
		}

		[MonoPInvokeCallback(typeof(NativeFileSO.UnityCallbackFunction))]
		internal static void FileWasOpenedCallback() {
			shared.TryRetrieveOpenedFile();
			shared.isBusy = false;
		}

	}
}


