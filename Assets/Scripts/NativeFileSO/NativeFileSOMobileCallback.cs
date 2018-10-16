using System.IO;
using UnityEngine;

public class NativeFileSOMobileCallback : MonoBehaviour {

	private static NativeFileSOMobileCallback instance;

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

		Debug.Log(string.Format("File Contents: \n{0}\n ---END OF FILE---", contents));
	}
}
