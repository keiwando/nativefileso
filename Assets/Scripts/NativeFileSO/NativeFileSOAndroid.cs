//#define UNITY_ANDROID
using System;
using System.Linq;

using UnityEngine;

#if UNITY_ANDROID
namespace Keiwando.NativeFileSO {
	
	public class NativeFileSOAndroid: INativeFileSOMobile {

		public static NativeFileSOAndroid shared = new NativeFileSOAndroid();

#pragma warning disable 0067
		public event Action<OpenedFile> FileWasOpened;
#pragma warning restore 0067

		private AndroidJavaObject Activity { 
			get {
				if (_activity == null) {
					using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) { 
						_activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
					}
				}
				return _activity;
			}
		} 
		private AndroidJavaObject _activity;

		private AndroidJavaClass JavaNativeSO { 
			get {
				if (_javaNativeSO == null) {
					_javaNativeSO = new AndroidJavaClass("com.keiwando.lib_nativefileso.NativeFileSO");
				}
				return _javaNativeSO;
			}
		}
		private AndroidJavaClass _javaNativeSO;

		private NativeFileSOAndroid() { }

		public void LoadIfTemporaryFileAvailable() { 
			
			var isAvailable = JavaNativeSO.CallStatic<bool>("IsTemporaryFileAvailable", Activity);

			if (isAvailable) {
				JavaNativeSO.CallStatic("LoadTemporaryFile", Activity);
			}
			Debug.Log("Is Temporary File available: " + isAvailable);
		}

		public bool IsFileLoaded() { 
			return JavaNativeSO.CallStatic<bool>("IsFileLoaded");
		}

		public OpenedFile GetOpenedFile() { 
			byte[] byteContents = JavaNativeSO.CallStatic<byte[]>("GetFileByteContents");
			string filename = JavaNativeSO.CallStatic<string>("GetFileName");

			// Reset the loaded data
			JavaNativeSO.CallStatic("ResetLoadedFile");

			return new OpenedFile(filename, byteContents);
		}

		public void OpenFile(SupportedFileType[] supportedTypes) { 

			string encodedMimeTypes = EncodeMimeTypes(supportedTypes.Select(x => x.MimeType).ToArray());

			if (supportedTypes == null || supportedTypes.Length == 0) {
				JavaNativeSO.CallStatic("OpenFile", Activity, SupportedFileType.Any.MimeType);
			} else {
				JavaNativeSO.CallStatic("OpenFile", Activity, encodedMimeTypes);
			}
		}

		public void SaveFile(FileToSave file) { 
		
			JavaNativeSO.CallStatic("SaveFile", Activity, file.SrcPath, file.MimeType);
		}

		private string EncodeMimeTypes(string[] extensions) {
			return string.Join(" ", extensions);
		}
	}
}
#endif
