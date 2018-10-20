using System;
using System.IO;

namespace Keiwando.NativeFileSO {

	public class FileToSave {

		public string SrcPath { get; private set; }
		public string Name { get; private set; }
		public string Extension { get; private set; }
		public string MimeType { get; private set; }

		public FileToSave(string srcPath) {
			this.SrcPath = srcPath;
			this.Name = System.IO.Path.GetFileName(srcPath);
			this.Extension = System.IO.Path.GetExtension(srcPath);
			this.MimeType = "*/*";
		}

		public FileToSave(string srcPath, string newName) 
			: this(srcPath) {

			this.Name = newName;
		}

		public FileToSave(string srcPath, string newName, string extension, 
		                  string mimetype = "*/*") 
			: this(srcPath, newName) {

			this.Extension = extension;
			this.MimeType = mimetype;
		}
	}
}
