using System;
namespace Keiwando.NFSO {

	public interface INativeFileSOMobile {
		
		void OpenFiles(SupportedFileType[] supportedTypes, bool canSelectMultiple);
		void SaveFile(FileToSave file);

		OpenedFile[] GetOpenedFiles();
	}
}
