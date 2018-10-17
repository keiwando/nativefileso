using System.IO;
using UnityEngine;

namespace Keiwando.NativeFileSO { 

	public class NativeFileSOMobileCallback : MonoBehaviour {

		public delegate void FileWasOpenedHandler(object sender, string contents);

		public event FileWasOpenedHandler FileWasOpened;

		public static NativeFileSOMobileCallback instance;

		void Awake() {
			if (instance == null) {
				instance = this;
				DontDestroyOnLoad(this.gameObject);
			} else {
				Destroy(this.gameObject);
			}
		}

		private void OnApplicationFocus(bool focus) {

			if (focus) {
				TryOpenURL();
			}
		}

		private void OnApplicationPause(bool pause) {
			if (!pause) {
				TryOpenURL();
			}
		}

		private void TryOpenURL() {
			var contents = new NativeFileSOMobile().TryOpenURL();

			if (FileWasOpened != null) {
				FileWasOpened(this, contents);
			}
		}

		public  void AndroidDidOpenTextFile(string contents) {

			print("AndroidDidOpenTextFile");

			if (FileWasOpened != null) {
				FileWasOpened(this, contents);
			}
		}
	}
}
