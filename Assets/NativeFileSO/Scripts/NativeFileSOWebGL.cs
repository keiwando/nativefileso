using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Keiwando.NativeFileSO {

    public class NativeFileSOWebGL : INativeFileSO {

        public static NativeFileSOWebGL shared = new NativeFileSOWebGL();

        [DllImport("__Internal")]
        private static extern void openFile();

        private NativeFileSOWebGL() {}

        public void OpenFile (SupportedFileType[] supportedTypes, Action<bool, OpenedFile> onCompletion) {
            openFile();
        }

        public void OpenFiles (SupportedFileType[] supportedTypes, Action<bool, OpenedFile[]> onCompletion) {
			
		}

        public void SaveFile (FileToSave file) {
			
		}
    }
}