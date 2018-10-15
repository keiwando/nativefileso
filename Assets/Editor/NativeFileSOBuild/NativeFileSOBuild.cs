using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Android;

public class NativeFileSOBuild {

	private static readonly string[] supportedExtensions = new string[] {
		"txt", "evol", "creat"
	};

	public int callbackOrder { get { return 0; } }

	[PostProcessBuildAttribute(1)]
	public static void OnPostProcessingBuild(BuildTarget target, 
	                                         string pathToProject) {

		if (target == BuildTarget.iOS) {
			PostProcessIOS(pathToProject);
		}
	}

	private static void PostProcessIOS(string pathToProject) { 
	
	}

	[MenuItem("NativeFileSO/GereateAndroidManifestForOpeningFiles")]
	public static void GenerateAndroidManifestForOpeningFiles() {

		var aarPath = CombinePaths("..", "Plugins", "Android", "NativeFileSOManifestContainer.aar");

		Directory.CreateDirectory(aarPath);
	}

	private static string CombinePaths(params string[] paths) {

		var combined = "";

		foreach (var path in paths) { 
			combined = Path.Combine(combined, path);
		}

		return combined;
	}

}
