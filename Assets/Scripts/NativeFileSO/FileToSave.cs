using System;
using System.IO;

namespace Keiwando.NativeFileSO {

	public class FileToSave {

		public string SrcPath { get; private set; }
		public string Name { get; private set; }
		public string Extension { get; private set; }
		public string MimeType { get; private set; }
		public SupportedFileType FileType { get; private set; }

		public FileToSave(string srcPath, SupportedFileType fileType = null) {
			this.SrcPath = srcPath;
			this.Name = System.IO.Path.GetFileName(srcPath);
			this.Extension = System.IO.Path.GetExtension(srcPath);
			this.MimeType = "*/*";
			this.FileType = fileType;
		}

		public FileToSave(string srcPath, string newName, SupportedFileType fileType = null) 
			: this(srcPath, fileType) {

			this.Name = newName;
			this.Extension = System.IO.Path.GetExtension(newName);
		}

		public FileToSave(string srcPath, string newName, string extension, 
		                  string mimetype = "*/*", SupportedFileType fileType = null) 
			: this(srcPath, newName, fileType) {

			this.Extension = extension;
			this.MimeType = mimetype;
			this.FileType = fileType;
		}
	}
}
