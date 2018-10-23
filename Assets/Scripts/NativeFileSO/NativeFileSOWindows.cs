using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using UnityEngine;
using Ookii.Dialogs;

namespace Keiwando.NativeFileSO {

	public class NativeFileSOWindows: INativeFileSODesktop {

		private class Win32Window: IWin32Window {
			public Win32Window(IntPtr ptr) {
				Handle = ptr;
			}
			public IntPtr Handle { get; set; }
		}

		[DllImport("user32.dll")]
		private static extern IntPtr GetActiveWindow();

		public static NativeFileSOWindows shared = new NativeFileSOWindows();
		
		public event Action<OpenedFile> FileWasOpened;

		private Action<bool, OpenedFile> _callback;
		private bool isBusy = false;

		public void OpenFile(SupportedFileType[] supportedTypes) { 
			
			if (isBusy) { return; }
			isBusy = true;

			var dialog = new VistaOpenFileDialog();
			
			dialog.Multiselect = false;

			if (supportedTypes != null && supportedTypes.Length > 0) {
				dialog.Filter = EncodeFilters(supportedTypes);
			}

			var result = dialog.ShowDialog(new Win32Window(GetActiveWindow()));

			if (result == DialogResult.OK) {
				var path = dialog.FileName;
				SendFileOpenedEvent(true, NativeFileSOMacWin.FileFromPath(path));
			} else {
				SelectionWasCancelled();
			}

			dialog.Dispose();
			isBusy = false;
		}

		public void OpenFile(SupportedFileType[] supportedTypes, Action<bool, OpenedFile> onOpen) { 
			
			if (isBusy) return;

			_callback = onOpen;
			OpenFile(supportedTypes);
		}

		public void SaveFile(FileToSave file) { 
			
			if (isBusy) return;
			isBusy = true;

			var dialog = new VistaSaveFileDialog();
			
			dialog.FileName = file.Name;
			dialog.DefaultExt = file.Extension;
			if (dialog.DefaultExt.Length > 0) {
				dialog.AddExtension = true;
				dialog.SupportMultiDottedExtensions = true;
			}
			if (file.FileType != null) {
				dialog.Filter = EncodeFilters(new []{ file.FileType });
			}

			var result = dialog.ShowDialog(new Win32Window(GetActiveWindow()));
			if (result == DialogResult.OK) {
				NativeFileSOMacWin.SaveFileToPath(file, dialog.FileName);
			} else {
				SelectionWasCancelled();
			}
			isBusy = false;
		}

		private void SelectionWasCancelled() {

			isBusy = false;
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
		}

		private String EncodeFilters(SupportedFileType[] types) {
			return string.Join("|", types.Select(delegate(SupportedFileType x){
				var ext = x.Extension.Equals(string.Empty) ? "*" : x.Extension;
				return string.Format("{0} (*.{1})|*.{1}", x.Name, ext);
			}).ToArray());

		}
	}
}
