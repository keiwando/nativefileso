using System;
using System.IO;

public class FileWriter {

	public static void WriteTestFile(string path) {

		var filename = "NativeFileSOTest.txt";
		var fullPath = Path.Combine(path, filename);
		var contents = "Native File SO Test file";

		File.WriteAllText(fullPath, contents);
	}
}

