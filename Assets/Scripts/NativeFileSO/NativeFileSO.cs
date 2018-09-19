using System;

public class NativeFileSO {

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
	private static INativeFileSO nativeFileSO = new NativeFileSOMacWin();
#elif UNITY_IOS || UNITY_ANDROID
	private static INativeFileSO nativeFileSO = new NativeFileSOMobile();
#else
	private static INativeFileSO nativeFileSO = null;
#endif

	public static readonly NativeFileSO shared = new NativeFileSO();

	public void OpenFile() {
		nativeFileSO.OpenFile();
	}

	public void SaveFile(string srcPath) {
		nativeFileSO.SaveFile(srcPath);
	}
}
