using System.IO;
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

		NativeFileSO.shared.FileWasOpened += delegate(OpenedFile file) {

			ShowContents(file);
		};
	}

	private void ExportTest() {

		var testFilePath = Path.Combine(Application.persistentDataPath, "Test.evol");
		NativeFileSO.shared.SaveFile(testFilePath, "Test", "evol");
	}

	private void ImportTest() {

		var types = new SupportedFileType[] { 
			CustomFileTypes.creat, CustomFileTypes.evol
		};

		NativeFileSO.shared.OpenFile(types);
	}

	private void ShowContents(OpenedFile file) {

		var output = string.Format("File Contents: \n{0}\n --- EOF ---\n{1} bytes\n{2}\n{3}", 
		                           file.ToUTF8String(), file.Data.Length, 
		                           file.Name, file.Extension);
		Debug.Log(output);
		textField.text = output;
	}
}
