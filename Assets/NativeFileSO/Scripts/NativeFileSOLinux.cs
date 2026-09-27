// 	Copyright (c) 2019 Keiwan Donyagard
//  Copyright (c) 2026 Zean Isnomo (zeankundev)
// 
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

#if UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX

using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;

namespace Keiwando.NFSO {
    // <summary>
    // Implements the open/save dialog for Linux using Zenity
    // </summary>
    public class NativeFileSOLinux : INativeFileSODesktop {
        public static NativeFileSOLinux shared = new NativeFileSOLinux();
        private static OpenedFile[] _noFiles = new OpenedFile[0];
        private static string[] _noPaths = new string[0];
        private const string zenityBin = "zenity";
        private bool isBusy = false;
        private bool zenityAvailabilityChecked = false;
		private bool zenityIsAvailable = false;

        public void OpenFile(SupportedFileType[] fileTypes, Action<bool, OpenedFile> onCompletion) {
			var openedFiles = OpenFilesSync(fileTypes, false);
 
			if (onCompletion != null) {
				if (openedFiles.Length > 0) {
					onCompletion(true, openedFiles[0]);
				} else {
					onCompletion(false, null);
				}
			}
		}

        public void OpenFiles(SupportedFileType[] supportedTypes, Action<bool, OpenedFile[]> onCompletion) {
			OpenFiles(supportedTypes, true, "", "", onCompletion);
		}
 
		public void SaveFile(FileToSave file) {
			SaveFile(file, "", "");
		}

        public void OpenFiles(SupportedFileType[] fileTypes, bool canSelectMultiple,
					   		  string title, string directory,
					   		  Action<bool, OpenedFile[]> onCompletion) {
 
			var openedFiles = OpenFilesSync(fileTypes, canSelectMultiple, title, directory);
			isBusy = false;
			var filesWereOpened = openedFiles.Length != 0;
			if (onCompletion != null) {
				onCompletion(filesWereOpened, openedFiles);
			}
		}

        public OpenedFile[] OpenFilesSync(SupportedFileType[] fileTypes, bool canSelectMultiple = true,
								   		  string title = "", string directory = "") {
 
			var paths = SelectOpenPathsSync(fileTypes, canSelectMultiple, title, directory);
			var openedFiles = new List<OpenedFile>();
 
			foreach (var path in paths) {
				var file = NativeFileSOMacWin.FileFromPath(path);
				if (file != null) {
					openedFiles.Add(file);
				}
			}
			isBusy = false;
 
			return openedFiles.ToArray();
		}

        public void SelectOpenPaths(SupportedFileType[] fileTypes, bool canSelectMultiple,
							 		string title, string directory,
							 		Action<bool, string[]> onCompletion) {
 
			var paths = SelectOpenPathsSync(fileTypes, canSelectMultiple, title, directory);
			isBusy = false;
			if (onCompletion != null) {
				var pathsSelected = paths.Length != 0;
				onCompletion(pathsSelected, paths);
			}
		}

        public string[] SelectOpenPathsSync(SupportedFileType[] fileTypes, bool canSelectMultiple = true,
									 		string title = "", string directory = "") {
 
			if (isBusy) { return _noPaths; }
			if (!IsZenityAvailable()) {
				LogZenityMissing();
				return _noPaths;
			}
			isBusy = true;
 
			var args = new StringBuilder();
			args.Append("--file-selection");
 
			if (canSelectMultiple) {
				args.Append(" --multiple --separator=\"\\n\"");
			}
 
			if (!string.IsNullOrEmpty(title)) {
				args.AppendFormat(" --title={0}", EscapeArg(title));
			}
 
			if (!string.IsNullOrEmpty(directory)) {
				var dir = EnsureStringEndsWith(directory, Path.DirectorySeparatorChar.ToString());
				args.AppendFormat(" --filename={0}", EscapeArg(dir));
			}
 
			AppendFileFilters(args, fileTypes);
 
			var result = RunZenity(args.ToString());
			isBusy = false;
 
			if (result.Cancelled || string.IsNullOrEmpty(result.Output)) {
				return _noPaths;
			}
 
			return result.Output
				.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
				.ToArray();
		}

        public void SaveFile(FileToSave file, string title, string directory) {
 
			if (isBusy) return;
			if (!IsZenityAvailable()) {
				LogZenityMissing();
				return;
			}
			isBusy = true;
 
			var args = new StringBuilder();
			args.Append("--file-selection --save --confirm-overwrite");
 
			if (!string.IsNullOrEmpty(title)) {
				args.AppendFormat(" --title={0}", EscapeArg(title));
			}
 
			string defaultPath;
			if (string.IsNullOrEmpty(directory)) {
				defaultPath = file.Name;
			} else {
				defaultPath = CreateFilenameForSaveDialog(directory, file.Name);
			}
			args.AppendFormat(" --filename={0}", EscapeArg(defaultPath));
 
			if (file.FileType != null) {
				AppendFileFilters(args, new[] { file.FileType });
			}
 
			var result = RunZenity(args.ToString());
			isBusy = false;
 
			if (!result.Cancelled && !string.IsNullOrEmpty(result.Output)) {
				var path = result.Output.Trim('\n', '\r');
				path = EnsureExtension(path, file.Extension);
				NativeFileSOMacWin.SaveFileToPath(file, path);
			}
		}

        public void SelectSavePath(FileToSave file,
								   string title,
								   string directory,
								   Action<bool, string> onCompletion) {
 
			var path = SelectSavePathSync(file, title, directory);
			if (onCompletion != null) {
				onCompletion(path != null, path);
			}
		}
 
		public string SelectSavePathSync(FileToSave file,
								  		 string title,
										 string directory) {
			return SelectSavePathSync(new SupportedFileType[] { file.FileType }, file.Name, title, directory);
		}
 
		public void SelectSavePath(SupportedFileType[] fileTypes,
												string defaultFileName,
												string title,
												string directory,
												Action<bool, string> onCompletion) {
 
			var path = SelectSavePathSync(fileTypes, defaultFileName, title, directory);
			if (onCompletion != null) {
				onCompletion(path != null, path);
			}
		}
 
		public string SelectSavePathSync(SupportedFileType[] fileTypes,
															string defaultFileName,
															string title,
															string directory) {
 
			if (isBusy) { return null; }
			if (!IsZenityAvailable()) {
				LogZenityMissing();
				return null;
			}
			isBusy = true;
 
			var args = new StringBuilder();
			args.Append("--file-selection --save --confirm-overwrite");
 
			if (!string.IsNullOrEmpty(title)) {
				args.AppendFormat(" --title={0}", EscapeArg(title));
			}
 
			string defaultPath;
			if (string.IsNullOrEmpty(directory)) {
				defaultPath = defaultFileName;
			} else {
				defaultPath = CreateFilenameForSaveDialog(directory, defaultFileName);
			}
			args.AppendFormat(" --filename={0}", EscapeArg(defaultPath));
 
			string defaultExt = null;
			if (fileTypes != null && fileTypes.Length > 0) {
				defaultExt = fileTypes[0].Extension.Split('|').FirstOrDefault();
				AppendFileFilters(args, fileTypes);
			}
 
			var result = RunZenity(args.ToString());
			isBusy = false;
 
			if (result.Cancelled || string.IsNullOrEmpty(result.Output)) {
				return null;
			}
 
			var path = result.Output.Trim('\n', '\r');
			if (!string.IsNullOrEmpty(defaultExt)) {
				path = EnsureExtension(path, defaultExt);
			}
			return path;
		}
        private struct ZenityResult {
			public bool Cancelled;
			public string Output;
		}
 
		private ZenityResult RunZenity(string arguments) {
 
			var result = new ZenityResult { Cancelled = true, Output = "" };
 
			try {
				var psi = new ProcessStartInfo {
					FileName = zenityBin,
					Arguments = arguments,
					UseShellExecute = false,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					CreateNoWindow = true
				};
 
				using (var process = Process.Start(psi)) {
					var output = process.StandardOutput.ReadToEnd();
					var error = process.StandardError.ReadToEnd();
					process.WaitForExit();
 
					if (process.ExitCode == 0) {
						result.Cancelled = false;
						result.Output = output;
					} else if (process.ExitCode != 1) {
						if (!string.IsNullOrEmpty(error)) {
							Debug.LogWarning("Zenity error: " + error);
						}
					}
				}
			} catch (Exception e) {
				Debug.LogError("Failed to run zenity. Is it installed? " + e.Message);
			}
 
			return result;
		}
 
		private bool IsZenityAvailable() {
 
			if (zenityAvailabilityChecked) {
				return zenityIsAvailable;
			}
			zenityAvailabilityChecked = true;
 
			try {
				var psi = new ProcessStartInfo {
					FileName = zenityBin,
					Arguments = "--version",
					UseShellExecute = false,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					CreateNoWindow = true
				};
				using (var process = Process.Start(psi)) {
					process.WaitForExit();
					zenityIsAvailable = process.ExitCode == 0;
				}
			} catch {
				zenityIsAvailable = false;
			}
 
			return zenityIsAvailable;
		}
 
		private void LogZenityMissing() {
			Debug.LogError(
				"NativeFileSO: Zenity is not installed or not available on the PATH. " +
				"Install it with your distro's package manager, e.g. `sudo apt install zenity`."
			);
		}

        private string CreateFilenameForSaveDialog(string directory, string name) {
 
			var sep = Path.DirectorySeparatorChar.ToString();
			if (!directory.EndsWith(sep)) {
				return string.Format("{0}{1}{2}", directory, sep, name);
			} else {
				return string.Format("{0}{1}", directory, name);
			}
		}
 
		private string EnsureStringEndsWith(string self, string end) {
			if (!self.EndsWith(end)) {
				return self + end;
			} else {
				return self;
			}
		}
 
		private string EnsureExtension(string path, string extension) {
			if (string.IsNullOrEmpty(extension)) return path;
 
			var ext = extension.Split('|')[0];
			if (string.IsNullOrEmpty(ext)) return path;
 
			var dotExt = "." + ext;
			if (!path.EndsWith(dotExt, StringComparison.OrdinalIgnoreCase)) {
				return path + dotExt;
			}
			return path;
		}
 
		private void AppendFileFilters(StringBuilder args, SupportedFileType[] types) {
			if (types == null || types.Length == 0) return;
 
			foreach (var type in types) {
				var ext = "*.*";
				if (!type.Extension.Equals(string.Empty)) {
					ext = string.Join(" ", type.Extension.Split('|').Select(
						str => string.Format("*.{0}", str)
					).ToArray());
				}
				args.AppendFormat(" --file-filter={0}", EscapeArg(string.Format("{0} | {1}", type.Name, ext)));
			}
			args.Append(" --file-filter=\"All files | *\"");
		}
 
		/// <summary>
		/// Quotes a string for safe use as a single shell argument passed to Process.Start,
		/// which on Linux runs the argument string through /bin/sh -c.
		/// </summary>
		private string EscapeArg(string arg) {
			if (arg == null) arg = "";
			var escaped = arg
				.Replace("\\", "\\\\")
				.Replace("\"", "\\\"")
				.Replace("`", "\\`")
				.Replace("$", "\\$");
			return "\"" + escaped + "\"";
		}
    }
}

#endif