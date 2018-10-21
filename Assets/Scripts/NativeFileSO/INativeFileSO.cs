using System;

namespace Keiwando.NativeFileSO { 

	public interface INativeFileSO {

		event Action<OpenedFile> FileWasOpened;

		void OpenFile(SupportedFileType[] supportedTypes);
		void OpenFile(SupportedFileType[] supportedTypes, Action<bool, OpenedFile> onOpen);

		void SaveFile(FileToSave file);
	}
}


