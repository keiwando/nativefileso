using System;
using Microsoft.Win32;

namespace Keiwando.NativeFileSO {

	public class NativeFileSOWindows: INativeFileSODesktop {


		public event Action<OpenedFile> FileWasOpened;

		public void OpenFile(SupportedFileType[] supportedTypes) { 
		
		}

		public void OpenFile(SupportedFileType[] supportedTypes, Action<bool, OpenedFile> onOpen) { 
		
		}

		public void SaveFile(FileToSave file) { 
			
		}
	}
}
