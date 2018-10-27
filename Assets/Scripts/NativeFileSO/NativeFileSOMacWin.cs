using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace Keiwando.NativeFileSO {

	public class NativeFileSOMacWin : INativeFileSO {

#if UNITY_STANDALONE_OSX
		private INativeFileSODesktop nativeFileSO = NativeFileSOMac.shared;
#elif UNITY_STANDALONE_WIN
		private INativeFileSODesktop nativeFileSO = NativeFileSOWindows.shared;
#else
		private INativeFileSODesktop nativeFileSO = null;
#endif

		public static NativeFileSOMacWin shared = new NativeFileSOMacWin();

		private delegate void UnityCallbackPathSelected(bool pathSelected, IntPtr path);

		private NativeFileSOMacWin() {}

		// MARK: - INativeFileSO Implementation

		public void OpenFile(SupportedFileType[] fileTypes, Action<bool, OpenedFile> onCompletion) {
			nativeFileSO.OpenFile(fileTypes, onCompletion);
		}

		public void OpenFiles(SupportedFileType[] fileTypes, Action<bool, OpenedFile[]> onCompletion) {
			nativeFileSO.OpenFiles(fileTypes, true, "", "", onCompletion);
		}

		public void SaveFile(FileToSave file) {
			nativeFileSO.SaveFile(file);
		}

		// MARK: - Desktop Only functionality

		public void OpenFiles(SupportedFileType[] fileTypes, 
		                      bool canSelectMultiple,
					   		  string title, 
		                      string directory, 
					   		  Action<bool, OpenedFile[]> onCompletion) {
			
			nativeFileSO.OpenFiles(fileTypes, canSelectMultiple, title, directory, onCompletion);
		}

		public OpenedFile[] OpenFilesSync(SupportedFileType[] fileTypes, 
		                                  bool canSelectMultiple, 
								   		  string title, 
		                                  string directory) {
			
			return nativeFileSO.OpenFilesSync(fileTypes, canSelectMultiple, title, directory);
		}

		public void SelectOpenPaths(SupportedFileType[] fileTypes, 
		                            bool canSelectMultiple,
							 		string title, 
		                            string directory,
							 		Action<bool, string[]> onCompletion) {
			
			nativeFileSO.SelectOpenPaths(fileTypes, canSelectMultiple, title, directory, onCompletion);
		}

		public string[] SelectOpenPathsSync(SupportedFileType[] fileTypes, 
		                                    bool canSelectMultiple,
									 		string title, 
		                                    string directory) {
			
			return nativeFileSO.SelectOpenPathsSync(fileTypes, canSelectMultiple, title, directory);
		}

		public void SaveFile(FileToSave file, string title, string directory) {
			nativeFileSO.SaveFile(file, title, directory);
		}

		public void SelectSavePath(FileToSave file,
								   string title,
								   string directory,
								   Action<bool, string> onCompletion) {
			nativeFileSO.SelectSavePath(file, title, directory, onCompletion);
		}

		public string SelectSavePathSync(FileToSave file,
								  		 string title,
										 string directory) {
			return nativeFileSO.SelectSavePathSync(file, title, directory);
		}

		// MARK: - Helpers
		public static OpenedFile FileFromPath(string path) {

			try {
				byte[] data = File.ReadAllBytes(path);
				var name = Path.GetFileName(path);
				return new OpenedFile(name, data);
			} catch (Exception e) {
				Debug.Log(e.StackTrace);
				return null;
			}
		}

		public static OpenedFile[] FilesFromPaths(string[] paths) {
			var files = new List<OpenedFile>();

			foreach (var path in paths) {
				var file = FileFromPath(path);
				if (file != null) {
					files.Add(file);
				}
			}
			return files.ToArray();
		}

		public static void SaveFileToPath(FileToSave file, string path) {
			File.Copy(file.SrcPath, path, true);
		}
	}
}


