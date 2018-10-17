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

		NativeFileSO.shared.FileWasOpened += delegate(string contents) {

			ShowContents(contents);
		};
	}

	private void ExportTest() {

		var testFilePath = Path.Combine(Application.persistentDataPath, "Test.evol");
		NativeFileSO.shared.SaveFile(testFilePath, "Test", "evol");
	}

	private void ImportTest() {

		var extensions = new string[] { "evol", "creat" };
		var contents = NativeFileSO.shared.OpenFile(extensions);

		//ShowContents(contents);
	}

	private void ShowContents(string contents) { 
		var output = string.Format("File Contents: \n{0}\n ---END OF FILE---", contents);
		Debug.Log(output);
		textField.text = output;
	}
}
