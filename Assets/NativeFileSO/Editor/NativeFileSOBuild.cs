using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using Keiwando.NativeFileSO;

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

		if (SupportedFilePreferences.supportedFileTypes.Length == 0) {
			return;
		}

		Debug.Log("NativeFileSO: Adding associated file types");

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
			typesDict.SetString("LSHandlerRank", supportedType.Owner ? "Owner" : "Alternate");

			var contentTypesArray = typesDict.CreateArray("LSItemContentTypes");
			foreach (var uti in supportedType.AppleUTI.Split('|')) {
				contentTypesArray.AddString(uti);
			}

			if (supportedType.Owner) {

				var exportedTypesDict = exportedTypesArray.AddDict();
				// Export the File Type

				if (!string.IsNullOrEmpty(supportedType.AppleConformsToUTI)) { 
					var conformsToArray = exportedTypesDict.CreateArray("UTTypeConformsTo");
					foreach (var conformanceUTI in supportedType.AppleConformsToUTI.Split('|')) {
						conformsToArray.AddString(conformanceUTI);
					}
				}

				exportedTypesDict.SetString("UTTypeDescription", supportedType.Name);
				exportedTypesDict.SetString("UTTypeIdentifier", supportedType.AppleUTI.Split('|')[0]);

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

	[MenuItem("NativeFileSO/UpdateAndroidPluginFileAssociations")]
	public static void UpdateAndroidPlugin() {

		var pluginFolder = CombinePaths(Application.dataPath, "Plugins", 
		                                "NativeFileSO", "Android");
		var aarPath = CombinePaths(pluginFolder, "NativeFileSO.aar");

		var manifestName = "AndroidManifest.xml";
		var manifestPath = CombinePaths(pluginFolder, manifestName);

		// DEBUG:
		//byte[] beforeData = File.ReadAllBytes(aarPath);

		ZipStorer zip = ZipStorer.Open(aarPath, FileAccess.ReadWrite);

		var centralDir = zip.ReadCentralDir();
		var manifest = centralDir.Find(x => Path.GetFileName(x.FilenameInZip)
		                               == manifestName);

		zip.ExtractFile(manifest, manifestPath);
		UpdateManifestAssociations(manifestPath);

		//ZipStorer.RemoveEntries(ref zip, new List<ZipStorer.ZipFileEntry>() { });

		ZipStorer.RemoveEntries(ref zip, new List<ZipStorer.ZipFileEntry>() { 
			manifest });
		zip.AddFile(ZipStorer.Compression.Deflate, manifestPath, manifest.FilenameInZip, "");

		zip.Close();

		File.Delete(manifestPath);

		//byte[] afterData = File.ReadAllBytes(aarPath);
		//TestByteCompare(beforeData, afterData, 47270);
		//WriteByteCompare(beforeData, afterData);

		Debug.Log("NativeFileSO: Finished updating the Android plugin");
	}

//	[MenuItem("NativeFileSO/RebuildAndroidPlugin")]
//	public static void GenerateAndroidManifestForOpeningFiles() {

//		//var aarPath = CombinePaths(Application.dataPath, "Plugins", "NativeFileSO",
//		//                           "Android", 

//		var pluginManifestPath = CombinePaths(Application.dataPath, "..",
//											  "Plugin-Projects",
//											  "Android", "NativeFileSO",
//											  "lib_nativefileso", "src", "main",
//												 "AndroidManifest.xml");

//		UpdateManifestAssociations(pluginManifestPath);

//		// Run the Gradle Build script
//		var pInfo = new System.Diagnostics.ProcessStartInfo();
//#if UNITY_EDITOR_OSX
//		var gradlewName = "gradlew";
//		//gradlewName = "test.sh"; 
//#else
//		var gradlewName = "gradlew.bat";
//#endif

	//	pInfo.FileName = CombinePaths(".", Application.dataPath, "..", 
	//	                                "Plugin-Projects", "Android",
	//	                                "NativeFileSO", gradlewName);
	//	pInfo.WorkingDirectory = CombinePaths(pInfo.FileName, "..");
		
	//	pInfo.UseShellExecute = false;
	//	pInfo.RedirectStandardOutput = true;
	//	pInfo.RedirectStandardError = true;
	//	pInfo.CreateNoWindow = false;

	//	pInfo.Arguments = "lib_nativefileso:build";
	//	//pInfo.Arguments = pInfo.FileName;
	//	//pInfo.FileName = "sh";

	//	Debug.Log("Building Android Plugin");

	//	System.Diagnostics.Process process = System.Diagnostics.Process.Start(pInfo);
	//	string output = process.StandardOutput.ReadToEnd();
	//	string error = process.StandardError.ReadToEnd();
	//	process.WaitForExit();

	//	Debug.Log(output);
	//	if (error != "") { 
	//		Debug.Log(error);
	//	}

	//	Debug.Log("Android Plugin Build Finished");
	//}

	private static void UpdateManifestAssociations(string manifestPath) { 
	
		var manifestContents = File.ReadAllText(manifestPath);

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


		File.WriteAllText(manifestPath, manifestContents);
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

	private static void TestByteCompare(byte[] data1, byte[] data2, int startIndex) {

		Debug.Log(string.Format("data1 length: {0}\ndata2 length: {1}", data1.Length, data2.Length));

		if (startIndex >= data1.Length)
			return;

		for (int i = startIndex; i < System.Math.Max(data1.Length, data2.Length); i++) {
			int b1 = i < data1.Length ? data1[i] : -1;
			int b2 = i < data2.Length ? data2[i] : -1;
			Debug.Log(string.Format("{0}\t{1}\t{2}", i, b1, b2));
		}
	}

	private static void WriteByteCompare(byte[] data1, byte[] data2) {

		var logPath = "/Users/Keiwan/Desktop/Test/ByteComparison.txt";
		//File.Create(logPath);

		var sb = new System.Text.StringBuilder();
		for (int i = 0; i < System.Math.Max(data1.Length, data2.Length); i++) { 
			int b1 = i < data1.Length ? data1[i] : -1;
			int b2 = i < data2.Length ? data2[i] : -1;

			if (b1 != b2) {
				sb.Append(string.Format("{0}\t{1}\t{2}\n", i, b1, b2));
			}
		}

		File.WriteAllText(logPath, sb.ToString());
	}

	[MenuItem("Debug/DeflateTest")]
	private static void DeflateTest() { 
		// DEBUG:
		var testMemoryStream = new MemoryStream();
		//Stream testStream = testMemoryStream;
		Stream testStream = new DeflateStream(testMemoryStream, CompressionMode.Compress, true);
		testStream.Write(new byte[] { 0 }, 0, 1);
		var testOutput = testMemoryStream.ToArray();
		testStream.Flush();
		testStream.Close();
		testStream.Dispose();
		var testBytes = testMemoryStream.ToArray();
		var sb = new System.Text.StringBuilder();
		for (int i = 0; i < testBytes.Length; i++) {
			sb.Append((int)testBytes[i]);
			sb.Append(" ");
		}
		Debug.Log(string.Format("Buffer: {0}", sb));
	}
}


