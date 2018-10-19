using System;

namespace Keiwando.NativeFileSO { 

	public interface INativeFileSO {

		event Action<OpenedFile> FileWasOpened;

		void OpenFile(SupportedFileType[] supportedTypes);

		void SaveFile(string srcPath,
					  string filename,
					  string extension);
	}
}


