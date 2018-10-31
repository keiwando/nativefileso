using System;
using System.IO;

namespace Keiwando.NativeFileSO {

	public class OpenedFile {

		public string Name { get; private set; }
		public string Extension { get; private set; }
		public byte[] Data { 
			get {
				return _data;
			}
		}

		private string _utf8String;

		public OpenedFile(string filename, byte[] data) {
			this._data = data;
			this.Name = filename;

			this.Extension = Path.GetExtension(filename);
		} 

		private byte[] _data;

		public string ToUTF8String() {

			if (_utf8String == null) {
				try {
					_utf8String = System.Text.Encoding.UTF8.GetString(_data);
				} catch {
					_utf8String = "";
				}
			}

			return _utf8String;
		}
	}
}
