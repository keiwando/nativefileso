using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestController : MonoBehaviour {

	[SerializeField]
	private Button exportTestButton;

	void Start () {

		exportTestButton.onClick.AddListener(() => ExportTest());
	}

	private void ExportTest() {


	}
}
