using System;

public interface INativeFileSO {

	string OpenFile(string[] extensions);

	void SaveFile(string srcPath, 
	              string filename, 
	              string extension);
}
