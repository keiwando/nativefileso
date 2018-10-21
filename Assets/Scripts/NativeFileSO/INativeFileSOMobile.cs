using System;
namespace Keiwando.NativeFileSO {

	public interface INativeFileSOMobile {

		event Action<OpenedFile> FileWasOpened;

		void OpenFile(SupportedFileType[] supportedTypes);
		void SaveFile(FileToSave file);

		bool IsFileLoaded();
		void LoadIfTemporaryFileAvailable();
		OpenedFile GetOpenedFile();
	}
}
