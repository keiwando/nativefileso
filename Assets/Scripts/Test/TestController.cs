using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Keiwando.NativeFileSO;

public class TestController : MonoBehaviour {

	[SerializeField]
	private Button exportTestButton;

	[SerializeField]
	private Button importTestButton;

	[SerializeField]
	private Text textField;

	void Start () {

		FileWriter.WriteTestFile(Application.persistentDataPath);

		exportTestButton.onClick.AddListener(() => ExportTest());
		importTestButton.onClick.AddListener(() => ImportTest());

		NativeFileSOMobile.shared.FileWasOpened += delegate(OpenedFile file) {

			ShowContents(file);
		};
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			SaveTestTitleDirectory();
		} else if (Input.GetKeyDown(KeyCode.Alpha2)) {
			OpenPathsTest();
		} else if (Input.GetKeyDown(KeyCode.Alpha3)) {
			OpenFilesTest();
		}
	}

	private void ExportTest() {

		NativeFileSO.shared.SaveFile(GetFileToSave());
	}

	private void ImportTest() {

		var types = new SupportedFileType[] { 
			CustomFileTypes.creat, CustomFileTypes.evol, SupportedFileType.Any
		};

		NativeFileSO.shared.OpenFile(types, delegate(bool wasFileOpened, OpenedFile file){
			if(wasFileOpened) {
				ShowContents(file);
			}
		});
	}

	private void SaveTestTitleDirectory() {
		NativeFileSOMacWin.shared.SaveFile(GetFileToSave(), "Custom Title", @"C:\Users");
	}

	private void OpenPathsTest() {
		NativeFileSOMacWin.shared.SelectOpenPaths(new []{ SupportedFileType.Any }, true, 
								"Custom Title", @"C:\Users", delegate(bool werePathsSelected, string[] paths){
			if (werePathsSelected) {
				textField.text = string.Format("Selected paths:\n{0}", string.Join("\n", paths));
			} else {
				textField.text = "Path selection was cancelled.";
			}
		});
	}

	private void OpenFilesTest() {
		NativeFileSOMacWin.shared.OpenFiles(new []{ SupportedFileType.Any }, true, 
								"Custom Title", @"C:\Users", delegate(bool wereFilesSelected, OpenedFile[] files){
			if (wereFilesSelected) {
				textField.text = string.Format("Selected file contents:\n{0}", string.Join("\n", files.Select(x => x.ToUTF8String()).ToArray()));
			} else {
				textField.text = "File selection was cancelled.";
			}
		});
	}

	private FileToSave GetFileToSave() {
		var testFilePath = Path.Combine(Application.persistentDataPath, "Test.evol");
		return new FileToSave(testFilePath, "Test.evol", CustomFileTypes.evol);
	}

	private void ShowContents(OpenedFile file) {

		var output = string.Format("File Contents: \n{0}\n --- EOF ---\n{1} bytes\n{2}\n{3}", 
		                           file.ToUTF8String(), file.Data.Length, 
		                           file.Name, file.Extension);
		Debug.Log(output);
		textField.text = output;
	}
}
