using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestController : MonoBehaviour {

	[SerializeField]
	private Button exportTestButton;

	[SerializeField]
	private Button importTestButton;

	void Start () {

		exportTestButton.onClick.AddListener(() => ExportTest());
		importTestButton.onClick.AddListener(() => ImportTest());
	}

	private void ExportTest() {

		NativeSaveOpen.SaveFile();
	}

	private void ImportTest() {

		NativeSaveOpen.OpenFile();
	}
}
