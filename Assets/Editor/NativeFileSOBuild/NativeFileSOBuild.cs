using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEditor.Android;

namespace Keiwando.NativeFileSO {

	public class NativeFileSOBuild {

		public int callbackOrder { get { return 0; } }

		[PostProcessBuildAttribute(1)]
		public static void OnPostProcessingBuild(BuildTarget target,
												 string pathToProject) {

			if (target == BuildTarget.iOS) {
				PostProcessIOS(pathToProject);
			}
		}

		private static void PostProcessIOS(string path) {

			var pathToProject = PBXProject.GetPBXProjectPath(path);
			PBXProject project = new PBXProject();
			project.ReadFromFile(pathToProject);

			var targetName = PBXProject.GetUnityTargetName();
			var targetGUID = project.TargetGuidByName(targetName);

			AddFrameworks(project, targetGUID);
			project.WriteToFile(pathToProject);

			// Edit Plist
			var plistPath = Path.Combine(path, "Info.plist");
			var plist = new PlistDocument();
			plist.ReadFromFile(plistPath);
			var rootDict = plist.root;

			//var appID = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS);

			var documentTypesArray = rootDict.CreateArray("CFBundleDocumentTypes");

			var exportedTypesArray = rootDict.CreateArray("UTExportedTypeDeclarations");

			foreach (var supportedType in SupportedFilePreferences.supportedFileTypes) {

				var typesDict = documentTypesArray.AddDict();

				typesDict.SetString("CFBundleTypeName", supportedType.Name);
				typesDict.SetString("CFBundleTypeRole", "Viewer");
				typesDict.SetString("LSHandlerRank", supportedType.Owner ? "Owner" : "Default");

				var contentTypesArray = typesDict.CreateArray("LSItemContentTypes");
				contentTypesArray.AddString(supportedType.AppleUTI);

				var exportedTypesDict = exportedTypesArray.AddDict();
				if (supportedType.Owner) {
					// Export the File Type

					var conformsToArray = exportedTypesDict.CreateArray("UTTypeConformsTo");
					// TODO: Retrieve from supportedType
					conformsToArray.AddString("public.plain-text");
					conformsToArray.AddString("public.text");

					exportedTypesDict.SetString("UTTypeDescription", supportedType.Name);
					exportedTypesDict.SetString("UTTypeIdentifier", supportedType.AppleUTI);

					var tagSpecificationDict = exportedTypesDict.CreateDict("UTTypeTagSpecification");
					tagSpecificationDict.SetString("public.filename-extension", supportedType.Extension);
					tagSpecificationDict.SetString("public.mime-type", supportedType.MimeType);
				}
			}

			plist.WriteToFile(plistPath);
		}


		static void AddFrameworks(PBXProject project, string targetGUID) {

			// Based on eppz! (http://eppz.eu/blog/override-app-delegate-unity-ios-osx-1/)
			project.AddBuildProperty(targetGUID, "OTHER_LDFLAGS", "-ObjC");
		}

		[MenuItem("NativeFileSO/UpdateAndroidPluginForOpeningFiles")]
		public static void GenerateAndroidManifestForOpeningFiles() {

			var pluginManifestPath = CombinePaths(Application.dataPath, "..",
												  "Plugin-Projects",
												  "Android", "NativeFileSO",
												  "lib_nativefileso", "src", "main",
													 "AndroidManifest.xml");

			//Debug.Log(pluginManifestPath);

			var manifestContents = File.ReadAllText(pluginManifestPath);

			//Debug.Log(manifestContents);

			var startTag = "<!-- #NativeFileSOIntentsStart# -->";
			var endTag = "<!-- #NativeFileSOIntentsEnd# -->";

			var mimeTypes = new HashSet<string>();
			foreach (var fileType in SupportedFilePreferences.supportedFileTypes) {
				mimeTypes.Add(fileType.MimeType);
			}

			var intentFilters = new System.Text.StringBuilder(startTag);
			foreach (var mimeType in mimeTypes) {

				intentFilters.Append(GetIntentForFileBrowser(mimeType));
			}
			intentFilters.Append(endTag);

			var pattern = string.Format("{0}.*{1}", startTag, endTag);
			manifestContents = Regex.Replace(manifestContents,
											 pattern,
											 intentFilters.ToString(),
											 RegexOptions.Singleline);

			//manifestContents = manifestContents.Replace("#content#", intentFilters.ToString());
			//Debug.Log(manifestContents);

			File.WriteAllText(pluginManifestPath, manifestContents);

			// Run the Gradle Build script
			var pInfo = new System.Diagnostics.ProcessStartInfo();
#if UNITY_EDITOR_OSX
			var gradlewName = "gradlew";
			//gradlewName = "test.sh"; 
#else
			var gradlewName = "gradlew.bat";
#endif

			pInfo.FileName = CombinePaths(".", Application.dataPath, "..", 
			                                "Plugin-Projects", "Android",
			                                "NativeFileSO", gradlewName);
			pInfo.WorkingDirectory = CombinePaths(pInfo.FileName, "..");
			
			pInfo.UseShellExecute = false;
			pInfo.RedirectStandardOutput = true;
			pInfo.RedirectStandardError = true;
			pInfo.CreateNoWindow = false;

			pInfo.Arguments = "lib_nativefileso:build";
			//pInfo.Arguments = pInfo.FileName;
			//pInfo.FileName = "sh";

			Debug.Log("Building Android Plugin");

			System.Diagnostics.Process process = System.Diagnostics.Process.Start(pInfo);
			string output = process.StandardOutput.ReadToEnd();
			string error = process.StandardError.ReadToEnd();
			process.WaitForExit();

			Debug.Log(output);
			if (error != "") { 
				Debug.Log(error);
			}

			Debug.Log("Android Plugin Build Finished");
		}

		private static string CombinePaths(params string[] paths) {

			var combined = "";

			foreach (var path in paths) {
				combined = Path.Combine(combined, path);
			}

			return combined;
		}

		private static string GetIntentForFileBrowser(string mimeType) {

			return string.Format(@"
	<intent-filter>
    	<action android:name=""android.intent.action.SEND""/> 
		<action android:name=""android.intent.action.SEND_MULTIPLE""/> 
		<category android:name=""android.intent.category.DEFAULT""/>
		<data android:mimeType=""{0}""/>
	</intent-filter>", mimeType);
		}

	}
}


