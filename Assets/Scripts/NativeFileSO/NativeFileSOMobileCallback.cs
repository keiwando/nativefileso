using System.IO;
using UnityEngine;
using AOT;

namespace Keiwando.NativeFileSO { 

	public class NativeFileSOMobileCallback : MonoBehaviour {

		public delegate void FileWasOpenedHandler(OpenedFile file);

		public event FileWasOpenedHandler FileWasOpened;

		public delegate void UnityCallbackFunction();

		public static NativeFileSOMobileCallback instance;

		void Awake() {
			if (instance == null) {
				instance = this;
				DontDestroyOnLoad(this.gameObject);
			} else {
				Destroy(this.gameObject);
			}
		}

		private void Start() {
			TryRetrieveOpenedFile();
		}

		private void OnApplicationFocus(bool focus) {

			if (focus) {
				TryRetrieveOpenedFile();
			}
		}

		private void OnApplicationPause(bool pause) {
			if (!pause) {
				TryRetrieveOpenedFile();
			}
		}

		private void TryRetrieveOpenedFile() {

			if (FileWasOpened == null) return;

			var fileSO = new NativeFileSOMobile();

			fileSO.LoadIfTemporaryAvailable(); 

			if (fileSO.IsFileOpened()) { 

				var file = new NativeFileSOMobile().GetOpenedFile();

				FileWasOpened(file);
			}
		}

		public void AndroidDidOpenTextFile(string message) {

			print("AndroidDidOpenTextFile");

			TryRetrieveOpenedFile();
		}

		[MonoPInvokeCallback(typeof(UnityCallbackFunction))]
		public static void IOSFileWasOpened() {
			instance.TryRetrieveOpenedFile();
		}
	}
}
