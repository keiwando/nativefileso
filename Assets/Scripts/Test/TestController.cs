using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TestController : MonoBehaviour {

	[SerializeField]
	private Button exportTestButton;

	[SerializeField]
	private Button importTestButton;

	void Start () {

		FileWriter.WriteTestFile(Application.persistentDataPath);

		exportTestButton.onClick.AddListener(() => ExportTest());
		importTestButton.onClick.AddListener(() => ImportTest());
	}

	private void ExportTest() {

		var testFilePath = Path.Combine(Application.persistentDataPath, "Test.evol");
		NativeFileSO.shared.SaveFile(testFilePath);
	}

	private void ImportTest() {

		NativeFileSO.shared.OpenFile();
	}
}
