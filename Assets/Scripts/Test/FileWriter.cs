using System;
using System.IO;

public class FileWriter {

	public static void WriteTestFile(string path) {

		var filename = "Test.evol";
		var fullPath = Path.Combine(path, filename);
		var contents = "Test Evolution Simulation Save";

		File.WriteAllText(fullPath, contents);
	}
}

