using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEditor.Android;

namespace Keiwando.NativeFileSO {

	public class NativeFileSOBuild {

		private struct SupportedFileType {
			public string Name;
			public string Extension;
			public bool Owner;
			public string AppleUTI;
			public string MimeType;
		}

		private static readonly SupportedFileType[] supportedFileTypes = new SupportedFileType[] {
			new SupportedFileType {
				Name = "Text File",
				Extension = "txt",
				Owner = false,
				AppleUTI = "public.plain-text",
				MimeType = "text/plain"
			},
			new SupportedFileType {
				Name = "Evolution Save File",
				Extension = "evol",
				Owner = true,
				AppleUTI = "com.keiwando.Evolution.evol",
				MimeType = "text/plain"
			}
		};

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

			var appID = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS);

			var documentTypesArray = rootDict.CreateArray("CFBundleDocumentTypes");

			var exportedTypesArray = rootDict.CreateArray("UTExportedTypeDeclarations");
			var exportedTypesDict = exportedTypesArray.AddDict();

			foreach (var supportedType in supportedFileTypes) {

				var typesDict = documentTypesArray.AddDict();

				typesDict.SetString("CFBundleTypeName", supportedType.Name);
				typesDict.SetString("CFBundleTypeRole", "Viewer");
				typesDict.SetString("LSHandlerRank", supportedType.Owner ? "Owner" : "Default");

				var contentTypesArray = typesDict.CreateArray("LSItemContentTypes");
				contentTypesArray.AddString(supportedType.AppleUTI);

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

		[MenuItem("NativeFileSO/GereateAndroidManifestForOpeningFiles")]
		public static void GenerateAndroidManifestForOpeningFiles() {

			var aarPath = CombinePaths(Application.dataPath, "Plugins", "Android");
			//Directory.CreateDirectory(aarPath);

			Debug.Log(aarPath);

			var manifestPath = Path.Combine(aarPath, "AndroidManifest.xml");
			var manifestContents = GetManifestTemplate();

			var intentFilters = new System.Text.StringBuilder();
			foreach (var supportedFileType in supportedFileTypes) {

				intentFilters.Append(GetIntentForFileBrowser(supportedFileType.Extension));
			}

			manifestContents = manifestContents.Replace("#content#", intentFilters.ToString());

			File.WriteAllText(manifestPath, manifestContents);
		}

		private static string CombinePaths(params string[] paths) {

			var combined = "";

			foreach (var path in paths) {
				combined = Path.Combine(combined, path);
			}

			return combined;
		}

		private static string GetManifestTemplate() {

			return @"
		
	<manifest xmlns:android=""http://schemas.android.com/apk/res/android"">
		<application>
			#content#
		</application>
	</manifest>
	";
		}

		private static string GetIntentForFileBrowser(string extension) {

			return string.Format(@"
	<intent-filter>
    	<action android:name=""android.intent.action.VIEW"" />
		<category android:name=""android.intent.category.DEFAULT""/>
	  	<data android:scheme=""file""/>
		<data android:host=""*""/>
		<data android:mimeType=""*/*""/>
		<data android:pathPattern="".*\\.{0}""/>
	</intent-filter>\n", extension);
		}

	}
}


