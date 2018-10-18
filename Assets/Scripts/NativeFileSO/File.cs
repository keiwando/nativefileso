using System;
namespace Keiwando.NativeFileSO {

	public class OpenedFile {

		public string name = "";
		public string extension = "";

		public bool isTextFile = false;
		public string stringContents = "";
		public byte[] data;
	}
}
