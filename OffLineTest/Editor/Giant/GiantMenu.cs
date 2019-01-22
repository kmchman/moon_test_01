using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.U2D;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using AL;

using Process = System.Diagnostics.Process;
using ProcessStartInfo = System.Diagnostics.ProcessStartInfo;

public partial class GiantMenu
{
	const string ResServerUrl = "http://singres.singta.co:10101/giant/";
	const string AssetBundleFolderName = "AssetBundles";
	const string StreamingAssetsPath = "Assets/StreamingAssets/";
	const string UIPrefabPath = "Assets/03_Prefabs/UI";
	const string UIImagePath = "05_Images";
	const string UIDummyPath = "98_NewUIArt";
	const string UIEffectImagePath = "09_Effects/UI/Texture";

	[MenuItem("Giant/Build Editor", false, 0)]
	static void BuiltEditTool()
	{
		GiantBuildEditor editor = EditorWindow.GetWindow<GiantBuildEditor>(true, "Build Editor");
#if UNITY_ANDROID
		editor.maxSize = new Vector2(650f, 650f);
#elif UNITY_IOS
		editor.maxSize = new Vector2(400f, 630f);
#endif
		editor.minSize = editor.maxSize;
		editor.Show();
	}
		
	[MenuItem("Giant/Define Editor", false, 1)]
	static void ShowDefineEditor()
	{
		GiantDefineEditor editor = EditorWindow.GetWindow<GiantDefineEditor>(true, "Define Editor");
		editor.maxSize = new Vector2(200f, 400f);
		editor.minSize = editor.maxSize;
		editor.Show();
	}

	[MenuItem("GameObject/Apply Changes To All Prefab", false, 100)]
	static public void ApplyChangesToAllPrefab()
	{
		GameObject[] gameObjects = Selection.gameObjects;
		foreach (var gameObject in gameObjects)
		{
			Object prefab = PrefabUtility.GetPrefabParent(gameObject);
			if (prefab != null)
				PrefabUtility.ReplacePrefab(PrefabUtility.FindPrefabRoot(gameObject), prefab, ReplacePrefabOptions.ConnectToPrefab);
		}
	}

	[MenuItem("Giant/Open Giant Setting\t\t&g", false, 200)]
	static void OpenGiantSetting()
	{
		GiantSetting.Open();
	}

#if UNITY_EDITOR_OSX || UNITY_EDITOR_WIN
	[MenuItem("Giant/Open Persistent Folder\t\t&f", false, 200)]
	static void OpenPersistentFolder()
	{
#if UNITY_EDITOR_OSX
		string macPath = Application.persistentDataPath;
		if (!macPath.StartsWith("/"))
			macPath = "/" + macPath;
		if (!macPath.EndsWith("/"))
			macPath = macPath + "/";
		macPath = "'" + macPath + "'";
		Process.Start("open", macPath);
#else
		string winPath = Application.persistentDataPath;
		if (!winPath.EndsWith("\\"))
			winPath = winPath + "\\";
		winPath = winPath.Replace('/', '\\');
		winPath = "\"" + winPath + "\"";
		Process.Start("explorer.exe", winPath);
#endif
	}
#endif

	[MenuItem("Giant/Request GetUser", false, 300)]
	static void GiantGetUser()
	{
		if (!Application.isPlaying)
			return;
		GiantHandler.Inst.doGetUser().SetSuccessAction(() => { Debug.LogWarning("Success Get User"); }).SetFailureAction((error) => { Debug.LogError(error); });
	}

	[MenuItem("Giant/Request GetPlant", false, 301)]
	static void GiantGetPlant()
	{
		if (!Application.isPlaying)
			return;
		GiantHandler.Inst.doGetBase().SetSuccessAction(() => { Debug.LogWarning("Success Get Base"); }).SetFailureAction((error) => { Debug.LogError(error); });
	}

	[MenuItem("Giant/ReloadDt", false, 302)]
	static void GiantReloadDt()
	{
		if (!Application.isPlaying)
			return;
		Global.Inst.InitResourceData();
	}

	[MenuItem("Giant/Generate Code/Datatable", false, 400)]
	static void GenerateDatatable()
	{
		CodeGenerator.GenDatatable();
	}

	[MenuItem("Giant/Generate Code/EnumTypes", false, 401)]
	static void GenerateEnumTypes()
	{
		CodeGenerator.GenEnum();
	}

	[MenuItem("Giant/Generate Code/Built In Text Enum", false, 402)]
	static void GenerateBuiltInTextEnum()
	{
		BuiltInTextAdder.inst.MakeBuiltInTextEnum();
	}

	[MenuItem("Giant/Select Prefab Folder/Character")]
	static void SelectPrefab_Character()
	{
		SelectPrefab("Assets/03_Prefabs/Character", true);
	}
	
	[MenuItem("Giant/Select Prefab Folder/Effect")]
	static void SelectPrefab_Effect()
	{
		SelectPrefab("Assets/03_Prefabs/Effects", true);
	}

	[MenuItem("Giant/Select Prefab Folder/UI_Root")]
	static void SelectPrefab_UI_Root()
	{
		SelectPrefab("Assets/03_Prefabs/UI/0_UIRoot", true);
	}

	[MenuItem("Giant/Select Prefab Folder/UI_Panel")]
	static void SelectPrefab_UI_Panel()
	{
		SelectPrefab("Assets/03_Prefabs/UI/1_Panel", true);
	}

	[MenuItem("Giant/Select Prefab Folder/UI_Item")]
	static void SelectPrefab_UI_Item()
	{
		SelectPrefab("Assets/03_Prefabs/UI/9_Item", true);
	}

	[MenuItem("Giant/Select GameObject/Characters")]
	static void SelectGameObject_Characters()
	{
		SelectGameObject("Scene/3DView/Characters", true);
	}

	[MenuItem("Giant/Select GameObject/Main Camera")]
	static void SelectGameObject_MainCamera()
	{
		Selection.activeObject = Camera.main;
	}

	[MenuItem("Giant/Build AssetBundles")]
	static void BuildAssetBundles()
	{
#if UNITY_ANDROID
		BuildAssetBundles(BuildTarget.Android);
#elif UNITY_IOS
		BuildAssetBundles(BuildTarget.iOS);
#endif
	}

	[MenuItem("Giant/Build AssetBundles Twice")]
	static void BuildAssetBundles_Twice()
	{
		if (EditorUtility.DisplayDialog("경고", "에셋번들 빌드를 2번실행 합니다. 진행 하시겠습니까?", "예", "아니오"))
		{
			BuildAssetBundles();
			BuildAssetBundles();
			GiantBuildEditor.SendSlackMessage();
		}
	}
		
	[MenuItem("Giant/Git Reset", false)]
	static void GitReset()
	{
		if (EditorUtility.DisplayDialog("Git Reset", "Git을 초기화 합니다.", "확인", "취소"))
			Process.Start("git", "reset --hard HEAD");
	}

	[MenuItem("Giant/PreBuildProcess")]
	static public void PreBuildProcess()
	{
		// 씬에 변경 사항이 있는 경우 씬을 저장할 지를 먼저 물어보고 true 가 반환되면 아래를 진행한다.
		// "Don't Save" 를 누르면 진행하지만, 저장되지 않은 변경된 정보는 날라가니 주의하기 바란다.
		if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			return;

		STText uiText;
		STImage uiImage;
		STRawImage uiRawImage;
		GameObject prefabObj;
		bool isResult = true;

		// 프로젝트 내부에 복사된 오브젝트 폴더를 삭제한다.
		string streamingAssetObjPath = PersistentStore.StreamingAssetObjDir;
		if (streamingAssetObjPath.EndsWith("/"))
			streamingAssetObjPath = streamingAssetObjPath.Substring(0, streamingAssetObjPath.Length - 1);
		if (Directory.Exists(streamingAssetObjPath))
			Directory.Delete(streamingAssetObjPath, true);
		Directory.CreateDirectory(streamingAssetObjPath);

		LoadResourceSeed();

		// 프로젝트 외부(giantres)의 폴더에서 빌드에 포함할 파일을 프로젝트 내부로 해시 파일 이름으로 복사한다.
		// 해시 파일 이름이 리소스 서버와 다르면 리소스 서버의 파일을 다운로드한다.
		string assetBundleResourceFolder = GiantSetting.Inst.assetBundleResourceFolder;
		if (string.IsNullOrEmpty(assetBundleResourceFolder))
		{
			Debug.LogError("Check the AssetBundle Resource Folder in GiantSetting.");
			return;
		}

		int resourceFolderLength = assetBundleResourceFolder.Length + 1;
		string [] files = Directory.GetFiles(assetBundleResourceFolder, "*", SearchOption.AllDirectories);
		// OffLineTest

		MoonTestResourceData resourceData = new MoonTestResourceData();
		resourceData.fileDic = new Dictionary<string, string>();

		for (int i = 0; i < files.Length; ++i)
		{
			string path = files[i].Substring(resourceFolderLength);
			if (!CheckPreBuildFile(path))
				continue;
			byte [] bytes = File.ReadAllBytes(files[i]);
			if (Path.GetExtension(path).Equals(".json"))
				bytes = Util.ALEEncode(bytes, ComponentAce.Compression.Libs.zlib.zlibConst.Z_DEFAULT_COMPRESSION);
			string hash = PersistentStore.GetHash(bytes);

		// OffLineTest
//			string id = ResourceManager.Inst.GetID(path);
//			if (!hash.Equals(id))
//				PersistentStore.Inst.LoadObject(id, (data) => { bytes = data; });

			if(bytes != null)
				File.WriteAllBytes(Path.Combine(PersistentStore.StreamingAssetObjDir, hash), bytes);
			
			if (!resourceData.fileDic.ContainsKey(hash))
				resourceData.fileDic.Add(hash, path);
			else
				Debug.Log("alreay exist");
		}
			
		// OffLineTest

		string filePath = Path.Combine(PersistentStore.StreamingAssetObjDir, "MoonTest");
		string json = Util.JsonEncode2(resourceData);
		byte[] jsonBytes =  AL.Util.Utf8Encode(json);
		jsonBytes = AL.Util.ALEEncode(jsonBytes);
		AL.Util.WriteFile(filePath, jsonBytes);

//		{
//			string filePath = AL.PersistentStore.GetPath(fileName);
//			string json = AL.Util.JsonEncode2(data);
//			byte[] bytes =  AL.Util.Utf8Encode(json);
//#if !UNITY_EDITOR
//			bytes = AL.Util.ALEEncode(bytes);
//#endif
//			AL.Util.WriteFile(filePath, bytes, isiCloudAndiTunesBackup);
//		}

		return;
		AssetDatabase.Refresh();

		// 다른 플랫폼 폴더 삭제
		AssetDatabase.DeleteAsset(StreamingAssetsPath + Constant.OtherPlatformPath);

		AssetDatabase.RemoveUnusedAssetBundleNames();

		List<int> textureInstanceIds = new List<int>();
		List<string> assetBundleAssetList = new List<string>();
		string [] assetBundleNames = AssetDatabase.GetAllAssetBundleNames();
		for (int i = 0; i < assetBundleNames.Length; ++i)
		{
			string [] assets = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleNames[i]);
			assetBundleAssetList.AddRange(assets.ToList());
			for (int j = 0; j < assets.Length; ++j)
			{
				SpriteAtlas spriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(assets[j]);
				if (spriteAtlas == null)
					continue;
				Sprite [] sprites = new Sprite[spriteAtlas.spriteCount];
				spriteAtlas.GetSprites(sprites);
				for (int k = 0; k < spriteAtlas.spriteCount; ++k)
				{
					Texture2D texture = sprites[k].texture;
					if (texture == null)
					{
						Debug.LogError("Error");
						return;
					}
					textureInstanceIds.Add(texture.GetInstanceID());
				}
			}
		}

		// 프리팹에 있는 STText 에 문자열이 추가되어 있으면 문자열을 삭제해준다.
		List<STText> textList = STPrefabUtil.FindAllComponents<STText>();
		for (int i = 0; i < textList.Count; ++i)
		{
			uiText = textList[i];
			prefabObj = uiText.transform.root.gameObject;

			if (!CheckPreBuildText(prefabObj, uiText))
				continue;
			
			uiText.text = string.Empty;
			EditorUtility.SetDirty(prefabObj);
		}

		// 프리팹에 있는 STImage 의 Source Image 에 에셋번들의 에셋이 설정되어 있으면 Source Image 를 삭제해준다.
		List<STImage> imageList = STPrefabUtil.FindAllComponents<STImage>();
		for (int i = 0; i < imageList.Count; ++i)
		{
			uiImage = imageList[i];
			prefabObj = uiImage.transform.root.gameObject;

			Sprite sprite = uiImage.sprite;
			if (!CheckPreBuildImage(prefabObj, null, sprite))
				continue;

			ExecutePreBuildImage(sprite, sprite.texture.GetInstanceID(), assetBundleAssetList, textureInstanceIds, (assetPath) => {
				Debug.LogFormat(prefabObj, "{0} : {1}", prefabObj.name, assetPath);
				uiImage.sprite = null;
				EditorUtility.SetDirty(prefabObj);
			}, (assetPath) => {
				Debug.LogError(assetPath, uiImage.transform.root.gameObject);
				isResult = false;
			});
		}

		// 프리팹에 있는 STRawImage 의 Texture 에 에셋번들의 에셋이 설정되어 있으면 Texture 를 삭제해준다.
		List<STRawImage> rawImageList = STPrefabUtil.FindAllComponents<STRawImage>();
		for (int i = 0; i < rawImageList.Count; ++i)
		{
			uiRawImage = rawImageList[i];
			prefabObj = uiRawImage.transform.root.gameObject;

			Texture texture = uiRawImage.texture;
			if (!CheckPreBuildImage(prefabObj, null, texture))
				continue;

			ExecutePreBuildImage(texture, texture.GetInstanceID(), assetBundleAssetList, textureInstanceIds, (assetPath) => {
				Debug.LogFormat(prefabObj, "{0} : {1}", prefabObj.name, assetPath);
				uiRawImage.texture = null;
				EditorUtility.SetDirty(prefabObj);
			}, (assetPath) => {
				Debug.LogError(assetPath, uiRawImage.transform.root.gameObject);
				isResult = false;
			});
		}

		// 변경된 프리팹을 먼저 저장한다.
		AssetDatabase.SaveAssets();

		// 빌드에 포함되는 모든 씬을 연다.
		List<string> curScenes = STSceneUtil.GetCurrentScenes();
		STSceneUtil.OpenBuildScenes();

		// 빌드에 포함되는 모든 씬에 있는 STText 에 문자열이 추가되어 있으면 문자열을 삭제해준다.
		textList = STSceneUtil.FindAllComponents<STText>();
		for (int i = 0; i < textList.Count; ++i)
		{
			uiText = textList[i];
			if (!CheckPreBuildText(null, uiText))
				continue;

			uiText.text = string.Empty;
			EditorSceneManager.MarkSceneDirty(uiText.gameObject.scene);
		}

		// 빌드에 포함되는 모든 씬에 있는 STImage 의 Source Image 에 에셋번들의 에셋이 설정되어 있으면 Source Image 를 삭제해준다.
		imageList = STSceneUtil.FindAllComponents<STImage>();
		for (int i = 0; i < imageList.Count; ++i)
		{
			uiImage = imageList[i];

			Sprite sprite = uiImage.sprite;
			if (!CheckPreBuildImage(null, null, sprite))
				continue;

			ExecutePreBuildImage(sprite, sprite.texture.GetInstanceID(), assetBundleAssetList, textureInstanceIds, (assetPath) => {
				Debug.LogFormat("{0} : {1}", uiImage.gameObject.scene.name, assetPath);
				uiImage.sprite = null;
				EditorSceneManager.MarkSceneDirty(uiImage.gameObject.scene);
			}, (assetPath) => {
				Debug.LogError(assetPath);
				isResult = false;
			});
		}

		// 빌드에 포함되는 모든 씬에 있는 STRawImage 의 Texture 에 에셋번들의 에셋이 설정되어 있으면 Texture 를 삭제해준다.
		rawImageList = STSceneUtil.FindAllComponents<STRawImage>();
		for (int i = 0; i < rawImageList.Count; ++i)
		{
			uiRawImage = rawImageList[i];

			Texture texture = uiRawImage.texture;
			if (!CheckPreBuildImage(null, null, texture))
				continue;

			ExecutePreBuildImage(texture, texture.GetInstanceID(), assetBundleAssetList, textureInstanceIds, (assetPath) => {
				Debug.LogFormat("{0} : {1}", uiRawImage.gameObject.scene.name, assetPath);
				uiRawImage.texture = null;
				EditorSceneManager.MarkSceneDirty(uiRawImage.gameObject.scene);
			}, (assetPath) => {
				Debug.LogError(assetPath);
				isResult = false;
			});
		}

		// 변경한 씬을 저장한다.
		EditorSceneManager.SaveOpenScenes();

		// 메뉴를 실행하기 전에 열려있던 씬으로 복구해준다.
		STSceneUtil.RestoreCurrentScenes(curScenes);

		Debug.Log(isResult ? "Success!" : "Fail!");
	}

	[MenuItem("Giant/PreBuildProcess (DryRun)")]
	static void PreBuildProcessDryRun()
	{
		// 씬에 변경 사항이 있는 경우 씬을 저장할 지를 먼저 물어보고 true 가 반환되면 아래를 진행한다.
		// "Don't Save" 를 누르면 진행하지만, 저장되지 않은 변경된 정보는 날라가니 주의하기 바란다.
		if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			return;

		STImage uiImage;
		STRawImage uiRawImage;
		GameObject prefabObj;
		bool isResult = true;

		AssetDatabase.RemoveUnusedAssetBundleNames();

		List<int> textureInstanceIds = new List<int>();
		List<string> assetBundleAssetList = new List<string>();
		string [] assetBundleNames = AssetDatabase.GetAllAssetBundleNames();
		for (int i = 0; i < assetBundleNames.Length; ++i)
		{
			string [] assets = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleNames[i]);
			assetBundleAssetList.AddRange(assets.ToList());
			for (int j = 0; j < assets.Length; ++j)
			{
				SpriteAtlas spriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(assets[j]);
				if (spriteAtlas == null)
					continue;
				Sprite [] sprites = new Sprite[spriteAtlas.spriteCount];
				spriteAtlas.GetSprites(sprites);
				for (int k = 0; k < spriteAtlas.spriteCount; ++k)
				{
					Texture2D texture = sprites[k].texture;
					if (texture == null)
					{
						Debug.LogError("Error");
						return;
					}
					textureInstanceIds.Add(texture.GetInstanceID());
				}
			}
		}

		// 프리팹에 있는 STImage 의 Source Image 에 에셋번들의 에셋이 설정되어 있으면 Source Image 를 삭제해준다.
		List<STImage> imageList = STPrefabUtil.FindAllComponents<STImage>();
		for (int i = 0; i < imageList.Count; ++i)
		{
			uiImage = imageList[i];
			prefabObj = uiImage.transform.root.gameObject;

			Sprite sprite = uiImage.sprite;
			if (!CheckPreBuildImage(prefabObj, null, sprite))
				continue;

			ExecutePreBuildImage(sprite, sprite.texture.GetInstanceID(), assetBundleAssetList, textureInstanceIds, (assetPath) => {
				Debug.LogFormat(prefabObj, "{0} : {1}", prefabObj.name, assetPath);
			}, (assetPath) => {
				Debug.LogError(assetPath, uiImage.transform.root.gameObject);
				isResult = false;
			});
		}

		// 프리팹에 있는 STRawImage 의 Texture 에 에셋번들의 에셋이 설정되어 있으면 Texture 를 삭제해준다.
		List<STRawImage> rawImageList = STPrefabUtil.FindAllComponents<STRawImage>();
		for (int i = 0; i < rawImageList.Count; ++i)
		{
			uiRawImage = rawImageList[i];
			prefabObj = uiRawImage.transform.root.gameObject;

			Texture texture = uiRawImage.texture;
			if (!CheckPreBuildImage(prefabObj, null, texture))
				continue;

			ExecutePreBuildImage(texture, texture.GetInstanceID(), assetBundleAssetList, textureInstanceIds, (assetPath) => {
				Debug.LogFormat(prefabObj, "{0} : {1}", prefabObj.name, assetPath);
			}, (assetPath) => {
				Debug.LogError(assetPath, uiRawImage.transform.root.gameObject);
				isResult = false;
			});
		}

		// 빌드에 포함되는 모든 씬을 연다.
		List<string> curScenes = STSceneUtil.GetCurrentScenes();
		STSceneUtil.OpenBuildScenes();

		// 빌드에 포함되는 모든 씬에 있는 STImage 의 Source Image 에 에셋번들의 에셋이 설정되어 있으면 Source Image 를 삭제해준다.
		imageList = STSceneUtil.FindAllComponents<STImage>();
		for (int i = 0; i < imageList.Count; ++i)
		{
			uiImage = imageList[i];

			Sprite sprite = uiImage.sprite;
			if (!CheckPreBuildImage(null, uiImage.gameObject, sprite))
				continue;

			ExecutePreBuildImage(sprite, sprite.texture.GetInstanceID(), assetBundleAssetList, textureInstanceIds, (assetPath) => {
				Debug.LogFormat("{0} : {1}", uiImage.gameObject.scene.name, assetPath);
			}, (assetPath) => {
				Debug.LogError(assetPath);
				isResult = false;
			});
		}

		// 빌드에 포함되는 모든 씬에 있는 STRawImage 의 Texture 에 에셋번들의 에셋이 설정되어 있으면 Texture 를 삭제해준다.
		rawImageList = STSceneUtil.FindAllComponents<STRawImage>();
		for (int i = 0; i < rawImageList.Count; ++i)
		{
			uiRawImage = rawImageList[i];

			Texture texture = uiRawImage.texture;
			if (!CheckPreBuildImage(null, uiRawImage.gameObject, texture))
				continue;

			ExecutePreBuildImage(texture, texture.GetInstanceID(), assetBundleAssetList, textureInstanceIds, (assetPath) => {
				Debug.LogFormat("{0} : {1}", uiRawImage.gameObject.scene.name, assetPath);
			}, (assetPath) => {
				Debug.LogError(assetPath);
				isResult = false;
			});
		}

		// 메뉴를 실행하기 전에 열려있던 씬으로 복구해준다.
		STSceneUtil.RestoreCurrentScenes(curScenes);

		Debug.Log(isResult ? "Success!" : "Fail!");
	}

	static void LoadResourceSeed()
	{
		// OfflineTest
//		string version = GetResourceGitBranchName();
//		string url = Util.StringFormat("{0}seed/{1}", ResServerUrl, version);
//		Util.SyncWWW(url, (www) => {
//			if (www == null || !string.IsNullOrEmpty(www.error))
//			{
//				Debug.LogError("Error : LoadResourceSeed");
//				return;
//			}
//
//			byte[] data = Util.ALEDecode(www.bytes);
//			string seed = Util.Utf8Decode(data);
//
//			PersistentStore.Init(ResServerUrl);
//			PersistentStore.Inst.LoadJsonFromID(seed, (obj) => {
//				ResourceManager.Inst.LoadedResourceSeed(seed, obj);
//			});
//		});

		string seedName = "MoonTest";
		PersistentStore.Inst.LoadJsonFromID(seedName, (obj) => {
			ResourceManager.Inst.LoadedResourceSeed(seedName, obj);
		});
	}

	static string GetResourceGitBranchName()
	{
		string args = "branch -l";
		ProcessStartInfo startInfo = new ProcessStartInfo("git", args);
		startInfo.WorkingDirectory = GiantSetting.Inst.assetBundleResourceFolder;
		startInfo.UseShellExecute = false;
		startInfo.RedirectStandardOutput = true;

		Process process = new Process();
		process.StartInfo = startInfo;
		process.Start();
		process.WaitForExit();

		StreamReader streamReader = process.StandardOutput;
		string result = streamReader.ReadToEnd();
		string [] splits = result.Split('\n');

		for (int i = 0; i < splits.Length; ++i)
		{
			if (splits[i].StartsWith("*"))
				return splits[i].Substring(1).Trim();
		}
		return null;
	}

	static bool CheckPreBuildFile(string path)
	{
		// OffLineTest
		if (Path.GetFileName(path).StartsWith("."))
			return false;

		if (path.StartsWith("dt"))
		{
			string fileName = Path.GetFileNameWithoutExtension(path);
			if (fileName.StartsWith("ml.") || fileName.StartsWith("pw."))
				return true;
			return fileName.Equals("giantdatas");
		}
		return path.StartsWith("ios") || path.StartsWith("raw") || path.StartsWith("dt/pw.nick") || path.StartsWith("raw/json");

#if UNITY_IOS
		if (Path.GetFileName(path).StartsWith("."))
			return false;
		return path.StartsWith("ios") || path.StartsWith("raw") || path.StartsWith("dt/pw.nick");


		if (path.StartsWith("dt"))
		{
			string fileName = Path.GetFileNameWithoutExtension(path);
			if (fileName.StartsWith("ml.") || fileName.StartsWith("pw."))
				return true;
			return fileName.Equals("giantdatas");
		}
		return path.StartsWith("raw/json");

#else
		return path.Equals("android/bgm_title");
#endif
	}

	static bool CheckPreBuildText(GameObject prefabObj, STText uiText)
	{
		if (string.IsNullOrEmpty(uiText.text) || uiText.isUnchangeable || (prefabObj != null && AssetDatabase.GetAssetPath(prefabObj).Contains(UIDummyPath)))
			return false;
		return true;
	}
	
	static bool CheckPreBuildImage(GameObject prefabObj, GameObject gameObj, Object asset)
	{
		if (asset == null || !AssetDatabase.IsForeignAsset(asset))
			return false;
		if (prefabObj != null && AssetDatabase.GetAssetPath(prefabObj).Contains(UIDummyPath))
			return false;
		if (gameObj != null && PrefabUtility.GetPrefabParent(gameObj) != null)
			return false;
		return true;
	}
	
	static void ExecutePreBuildImage(Object asset, int idInstance, List<string> assetBundleAssetList, List<int> textureInstanceIds, System.Action<string> resetAction, System.Action<string> errorAction)
	{
		string assetPath = AssetDatabase.GetAssetPath(asset);
		int index = assetBundleAssetList.IndexOf(assetPath);
		if (index < 0)
			index = textureInstanceIds.IndexOf(idInstance);

		if (index >= 0)
			resetAction(assetPath);
		else if (!assetPath.Contains(UIImagePath) && !assetPath.Contains(UIEffectImagePath))
			errorAction(assetPath);
	}
	
	[MenuItem("Giant/Print GameObject Path", false, 500)]
	static void PrintGameObjectPath()
	{
		GameObject gameObject = Selection.activeGameObject;
		if (gameObject == null)
		{
			Debug.LogError("Not Selected GameObject");
			return;
		}

		Debug.LogWarning(Giant.Util.GetGameObjectPath(gameObject));
	}

	[MenuItem("Giant/Capture ScreenShot", false, 600)]
	static void CaptureScreenShot()
	{
		string fileName = Util.StringFormat("ScreenShot_{0}.png", System.DateTime.Now.ToString("yyyyMMdd_HHmmss"));
		ScreenCapture.CaptureScreenshot(fileName);
	}

#if UNITY_EDITOR_OSX
	[MenuItem("Giant/Capture GameView To Clipboard", false, 600)]
	static void GameViewToClipboard()
	{
		System.Reflection.Assembly assembly = typeof(UnityEditor.EditorWindow).Assembly;
		System.Type type = assembly.GetType("UnityEditor.GameView");
		EditorWindow gameview = EditorWindow.GetWindow(type);
		Rect rect = gameview.position;
		string str = Util.StringFormat("-cR{0},{1},{2},{3}", rect.xMin, rect.yMin + 17, rect.size.x, rect.size.y - 17);
		Process.Start("screencapture", str);
	}

	[MenuItem("Giant/Capture ClippingArea To Clipboard", false, 600)]
	static void ClipingAreaToClipboard()
	{
		Process.Start("screencapture", "-ic");
	}
#endif

	[MenuItem("Giant/Extract Shaders From Prefabs", false, 800)]
	static void ExtractShadersFromPrefab()
	{
		AssetDatabase.RemoveUnusedAssetBundleNames();

		Dictionary<int, string> shaders = new Dictionary<int, string>();
		string [] assetbundleNames = AssetDatabase.GetAllAssetBundleNames();
		for (int i = 0; i < assetbundleNames.Length; ++i)
		{
			string [] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(assetbundleNames[i]);
			for (int j = 0; j < assetPaths.Length; ++j)
			{
				string ext = assetPaths[j].Substring(assetPaths[j].LastIndexOf('.')).ToLower();
				if (ext.EndsWith(".prefab"))
				{
					ExtractShadersFromGameObject(AssetDatabase.LoadAssetAtPath(assetPaths[j], typeof(GameObject)) as GameObject, shaders);
				}
			}
		}

		int index = 0;
		var enumerator = shaders.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Debug.LogFormat(Shader.Find(enumerator.Current.Value), "{0} ({1}) : {2}", index, enumerator.Current.Key, enumerator.Current.Value);
			++index;
		}
	}

	[MenuItem("Giant/Extract Shaders From Current Scene", false, 800)]
	static void ExtractShadersFromScene()
	{
		Dictionary<int, string> shaders = new Dictionary<int, string>();

		ExtractShadersFromCurrentScene(shaders);

		int index = 0;
		var enumerator = shaders.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Debug.LogFormat(Shader.Find(enumerator.Current.Value), "{0} ({1}) : {2}", index, enumerator.Current.Key, enumerator.Current.Value);
			++index;
		}
	}

	[MenuItem("Giant/Extract Shaders From All Game Scenes", false, 800)]
	static void ExtractShadersFromAllGameScenes()
	{
		const string GAME_SCENE_PATH = "Assets/01_Scenes/GameScene";

		// 씬에 변경 사항이 있는 경우 씬을 저장할 지를 먼저 물어보고 true 가 반환되면 아래를 진행한다.
		// "Don't Save" 를 누르면 진행하지만, 저장되지 않은 변경된 정보는 날라가니 주의하기 바란다.
		if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			return;

		List<string> curScenes = STSceneUtil.GetCurrentScenes();
		EditorSceneManager.SaveOpenScenes();

		Dictionary<int, string> allShaders = new Dictionary<int, string>();
		Dictionary<int, string> shaders = new Dictionary<int, string>();
		StringBuilder stringBuilder = new StringBuilder();
		int index;

		string [] guids = AssetDatabase.FindAssets("t:Scene", new string[] { GAME_SCENE_PATH });
		for (int i = 0; i < guids.Length; ++i)
		{
			string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
			EditorSceneManager.OpenScene(assetPath);

			shaders.Clear();
			ExtractShadersFromCurrentScene(shaders);

			index = 0;
			stringBuilder.Remove(0, stringBuilder.Length);
			stringBuilder.AppendLine(assetPath);

			var enumerator = shaders.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (!allShaders.ContainsKey(enumerator.Current.Key))
					allShaders.Add(enumerator.Current.Key, enumerator.Current.Value);

				stringBuilder.AppendLine();
				stringBuilder.AppendFormat("{0} ({1}) : {2}", index, enumerator.Current.Key, enumerator.Current.Value);
				++index;
			}

			Debug.Log(stringBuilder.ToString());
		}
		
		stringBuilder.Remove(0, stringBuilder.Length);
		stringBuilder.AppendLine("All Shader List");

		index = 0;
		var enumerator2 = allShaders.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat("{0} ({1}) : {2}", index, enumerator2.Current.Key, enumerator2.Current.Value);
			++index;
		}

		Debug.Log(stringBuilder.ToString());

		STSceneUtil.RestoreCurrentScenes(curScenes);
	}

	[MenuItem("Assets/Giant/Find Referencies In Scene(Image)\t\t&r", false, 100)]
	static void FindReferenciesInSceneByImage()
	{
		if (Selection.activeObject == null)
			return;

		Object [] selectionObjs = Selection.objects;
		for (int i = 0; i < selectionObjs.Length; ++i)
		{
			System.Type type = selectionObjs[i].GetType();
			if (!type.Equals(typeof(Texture)) && !type.IsSubclassOf(typeof(Texture)))
			{
				Debug.LogErrorFormat(selectionObjs[i], "{0} is not Texture or Sprite", selectionObjs[i].name);
				return;
			}
		}

		int [] ids = new int [selectionObjs.Length];
		for (int i = 0; i < ids.Length; ++i)
		{
			ids[i] = selectionObjs[i].GetInstanceID();
		}

		Canvas [] canvases = Object.FindObjectsOfType<Canvas>();
		List<Object> objs = new List<Object>();
		for (int i = 0; i < canvases.Length; ++i)
		{
			Image [] images = canvases[i].GetComponentsInChildren<Image>(true);
			for (int j = 0; j < images.Length; ++j)
			{
				if (images[j].sprite == null)
					continue;
				int id = images[j].sprite.texture.GetInstanceID();
				if (System.Array.FindIndex(ids, (value) => { return id == value; }) < 0)
					continue;
				Debug.Log(images[j].name, images[j].gameObject);
				objs.Add(images[j].gameObject);
			}
		}

		if (objs.Count > 0)
			Selection.objects = objs.ToArray();
	}

	[MenuItem("Assets/Giant/Find Dependencies In Scene", false, 101)]
	static void FindDependenciesInScene()
	{
		Object[] objects = Selection.objects;
		if (objects == null || objects.Length <= 0)
			return;

		EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
		Dictionary<EditorBuildSettingsScene, string[]> allDependenciesInScenes = new Dictionary<EditorBuildSettingsScene, string[]>();
		foreach (var scene in scenes)
		{
			string[] dependenciesPath = AssetDatabase.GetDependencies(new string[]{ scene.path });
			allDependenciesInScenes[scene] = dependenciesPath;
		}

		foreach (var oneObject in objects)
		{
			string objectPath = AssetDatabase.GetAssetPath(oneObject);

			List<string> pathList = new List<string>();

			foreach(var sceneDependencies in allDependenciesInScenes)
			{
				if(System.Array.IndexOf(sceneDependencies.Value, objectPath) > 0)
					pathList.Add(sceneDependencies.Key.path);
			}

			string result = oneObject.name + " => ";

			if (pathList.Count == 0)
			{
				result += "\nNone";
			}
			else
			{
				for(int i = 0; i < pathList.Count; i++)
				{
					result += Util.StringFormat("\n {0}. {1}", i, pathList[i]);
				}
			}

			Debug.LogWarning(result);
		}
	}

	[MenuItem("Assets/Giant/Find Referencies In Prefab", false, 102)]
	static void FindReferenciesInPrefab()
	{
		Object obj = Selection.activeObject;
		if (obj == null)
			return;

		string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(obj));
		if (guid == null)
			return;
		
		string[] allPrefabPath = STPrefabUtil.FindAllPrefabPath();

		foreach (var path in allPrefabPath)
		{
			string text = Util.ReadText(path);

			if (text != null && text.IndexOf(guid) >= 0)
				Debug.LogWarning(path, AssetDatabase.LoadAssetAtPath<GameObject>(path));
		}

		Debug.LogWarning("Done");
	}

	[MenuItem("Giant/Find Null Image Component In Scene", false, 900)]
	static void FindNullImageInScene()
	{
		List<Object> objs = new List<Object>();
		List<Image> images = STSceneUtil.FindAllComponents<Image>();
		for (int i = 0; i < images.Count; ++i)
		{
			if (images[i].sprite != null)
				continue;
			Mask mask = images[i].GetComponent<Mask>();
			if (mask != null && !mask.showMaskGraphic)
				continue;
			Debug.Log(images[i].name, images[i].gameObject);
			objs.Add(images[i].gameObject);
		}

		if (objs.Count > 0)
			Selection.objects = objs.ToArray();
	}

	[MenuItem("Giant/Find Null Image Component In Prefab", false, 900)]
	static void FindNullImageInPrefab()
	{
		List<Object> objs = new List<Object>();
		List<Image> images = STPrefabUtil.FindAllComponents<Image>();
		GameObject prefabObj;
		for (int i = 0; i < images.Count; ++i)
		{
			if (images[i].sprite != null)
				continue;
			Mask mask = images[i].GetComponent<Mask>();
			if (mask != null && !mask.showMaskGraphic)
				continue;
			prefabObj = images[i].transform.root.gameObject;
			Debug.Log(Giant.Util.GetGameObjectPath(images[i].gameObject), prefabObj);
			objs.Add(prefabObj);
		}
		
		if (objs.Count > 0)
			Selection.objects = objs.ToArray();
	}
	
	[MenuItem("Giant/Find Null TextStyler Component In Scene", false, 900)]
	static void FindNullStylerInScene()
	{
		List<Object> objs = new List<Object>();
		List<STText> stText = STSceneUtil.FindAllComponents<STText>();
		for (int i = 0; i < stText.Count; ++i)
		{
			if (stText[i].textStyler != null)
				continue;

			Debug.Log(stText[i].name, stText[i].gameObject);
			objs.Add(stText[i].gameObject);
		}

		if (objs.Count > 0)
			Selection.objects = objs.ToArray();
	}
	
	[MenuItem("Giant/Find Null UITextSetter", false, 900)]
	static void FindNoneTextSetter()
	{
		// 씬에 변경 사항이 있는 경우 씬을 저장할 지를 먼저 물어보고 true 가 반환되면 아래를 진행한다.
		// "Don't Save" 를 누르면 진행하지만, 저장되지 않은 변경된 정보는 날라가니 주의하기 바란다.
		if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			return;
		
		const string delimiter = "@";

		STText uiText;
		GameObject prefabObj;

		Debug.LogWarning("::::::::: Find In Prefab :::::::::");
		
		List<STText> textList = STPrefabUtil.FindAllComponents<STText>();
		for (int i = 0; i < textList.Count; ++i)
		{
			uiText = textList[i];
			prefabObj = uiText.transform.root.gameObject;

			if (AssetDatabase.GetAssetPath(prefabObj).Contains(UIDummyPath))
				continue;
			
			if (!uiText.text.StartsWith(delimiter) || uiText.GetComponent<UITextSetter>() != null)
				continue;
			
			Debug.LogWarning(Giant.Util.GetGameObjectPath(uiText.gameObject) + " : " + uiText.text, prefabObj);
		}

		Debug.LogWarning("::::::::: Find In Build Scenes :::::::::");
		
		List<string> curScenes = STSceneUtil.GetCurrentScenes();
		STSceneUtil.OpenBuildScenes();

		textList = STSceneUtil.FindAllComponents<STText>();
		for (int i = 0; i < textList.Count; ++i)
		{
			uiText = textList[i];
			if (!uiText.text.StartsWith(delimiter) || uiText.GetComponent<UITextSetter>() != null)
				continue;
			
			if (PrefabUtility.GetPrefabParent(uiText.gameObject) != null)
				continue;
			
			Debug.LogWarning(Giant.Util.GetGameObjectPath(uiText.gameObject) + " : " + uiText.text);
		}
		
		// 메뉴를 실행하기 전에 열려있던 씬으로 복구해준다.
		STSceneUtil.RestoreCurrentScenes(curScenes);
	}

	[MenuItem("Giant/Find Null ReuseIdentifier STScrollRectItem In Prefab", false, 900)]
	static void FindNullReuseIdentifierSTScrollRectItemInPrefab()
	{
		string [] guids = AssetDatabase.FindAssets("t:Prefab", new string[] { UIPrefabPath });
		for (int i = 0; i < guids.Length; ++i)
		{
			string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
			GameObject obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
			if (obj == null)
				continue;
			STScrollRectItem item = obj.GetComponent<STScrollRectItem>();
			if (item == null)
				continue;
			if (string.IsNullOrEmpty(item.reuseIdentifierName))
				Debug.LogWarning(assetPath, obj);
		}
	}

	[MenuItem("Giant/Find All Remove Object In Prefab", false, 900)]
	static void FindAllRemoveObjectInPrefab()
	{
		string [] guids = AssetDatabase.FindAssets("t:Prefab", new string[] { UIPrefabPath });
		for (int i = 0; i < guids.Length; ++i)
		{
			string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
			GameObject obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
			if (obj == null)
				continue;
			if (IsExistRemoveObject(obj.transform))
				Debug.LogWarning(assetPath, obj);
		}
	}

	[MenuItem("Giant/Find All UITextEnum In Prefab", false, 900)]
	static void FindAllUITextEnumInPrefab()
	{
		FindAllUITextEnumInPrefab(false);
	}
	
	[MenuItem("Giant/Find All UITextEnum In Prefab (ColorTag)", false, 900)]
	static void FindAllUITextEnumInPrefab_ColorTag()
	{
		FindAllUITextEnumInPrefab(true);
	}
	
	[MenuItem("Giant/Find All UITextEnum In Build Scene", false, 900)]
	static void FindAllUITextEnumInScene()
	{
		FindAllUITextEnumInScene(false);
	}
	
	[MenuItem("Giant/Find All UITextEnum In Build Scene (ColorTag)", false, 900)]
	static void FindAllUITextEnumInScene_ColorTag()
	{
		FindAllUITextEnumInScene(true);
	}
	
	[MenuItem("Giant/Find All Useless CanvasRenderer In Prefab", false, 900)]
	static void FindAllUselessCanvasRendererInPrefab()
	{
		string [] guids = AssetDatabase.FindAssets("t:Prefab", new string[] { UIPrefabPath });
		for (int i = 0; i < guids.Length; ++i)
		{
			string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
			GameObject obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
			if (obj == null)
				continue;
			if (IsExistUselessCanvasRendererObject(obj.transform))
				Debug.LogWarning(assetPath, obj);
		}
	}

	[MenuItem("Giant/Remove Useless CanvasRenderer Object", false, 900)]
	static void RemoveUselessCanvasRendererObject()
	{
		// 프리팹에 있는 CanvasRenderer 에 Graphic 컴포넌트가 없으면 CanvasRenderer 를 삭제해준다.
		GameObject prefabObj;
		CanvasRenderer canvasRenderer;
		List<CanvasRenderer> canvasRendererList = STPrefabUtil.FindAllComponents<CanvasRenderer>();
		for (int i = 0; i < canvasRendererList.Count; ++i)
		{
			canvasRenderer = canvasRendererList[i];

			if (canvasRenderer.GetComponent<Graphic>() == null)
			{
				prefabObj = canvasRenderer.transform.root.gameObject;
				Debug.LogWarning(Giant.Util.GetGameObjectPath(canvasRenderer.transform.gameObject));

				Object.DestroyImmediate(canvasRenderer, true);

				EditorUtility.SetDirty(prefabObj);
			}
		}
	}

	static void SelectPrefab(string assetPath, bool isFoldOut)
	{
		Object obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));
		if (isFoldOut)
		{
			Object childObj = null;
			string [] files = Directory.GetFiles(assetPath);
			for (int i = 0; i < files.Length; ++i)
			{
				childObj = AssetDatabase.LoadAssetAtPath(files[i], typeof(Object));
				if (childObj != null)
				{
					obj = childObj;
					break;
				}
			}
		}
		Selection.activeObject = obj;
	}

	static void SelectGameObject(string gameObjectName, bool isFoldOut)
	{
		GameObject obj = GameObject.Find(gameObjectName);
		if (obj != null && isFoldOut)
		{
			Transform transform = obj.transform;
			if (transform.childCount > 0)
				obj = transform.GetChild(0).gameObject;
		}
		Selection.activeObject = obj;
	}

	static void BuildAssetBundles(BuildTarget buildTarget)
	{
		// AssetBundleDatabase
		AssetBundleManager.MakeAssetBundleDatabase();
		SaveAssetBundleDatabase();

		// 에셋번들 빌드
		string folderName = Path.Combine(AssetBundleFolderName, buildTarget.ToString());
		if (!Directory.Exists(folderName))
			Directory.CreateDirectory(folderName);
		BuildPipeline.BuildAssetBundles(folderName, BuildAssetBundleOptions.None, buildTarget);

		// 폰트 이동 (AssetBundleFolder -> Assets/StreamingAssets/Android or iOS 폴더로 이동)
		string streamingAssetsPlatformPath = StreamingAssetsPath + Constant.PlatformPath;
		string [] files = Directory.GetFiles(streamingAssetsPlatformPath);
		for (int i = 0; i < files.Length; ++i)
		{
			if (Path.GetExtension(files[i]).Equals(".meta"))
				continue;
			File.Delete(files[i]);
		}

		files = Directory.GetFiles(folderName);
		for (int i = 0; i < files.Length; ++i)
		{
			if (!files[i].Contains(Constant.FontPathPrefix))
				continue;
			if (Path.GetExtension(files[i]).Equals(".manifest"))
			{
				File.Delete(files[i]);
				continue;
			}
			File.Move(files[i], streamingAssetsPlatformPath + "/" + Path.GetFileName(files[i]));
		}

		// 프로젝트 외부(giant-assetbundle)로 폴더 이동
		string assetBundleDestFolder = GiantSetting.Inst.assetBundleDestFolder;
		if (string.IsNullOrEmpty(assetBundleDestFolder))
		{
			Debug.LogWarning("Check the AssetBundle Destination Folder in GiantSetting.");
			return;
		}
		string destBuildTargetFolder = Path.Combine(assetBundleDestFolder, buildTarget.ToString());
		if (!Directory.Exists(assetBundleDestFolder))
			Directory.CreateDirectory(assetBundleDestFolder);
		if (Directory.Exists(destBuildTargetFolder))
			Directory.Delete(destBuildTargetFolder, true);
		Directory.Move(folderName, destBuildTargetFolder);

		// 프로젝트 외부(giantres)로 파일 복사 (.manifest 파일 제외)
		string assetBundleResourceFolder = GiantSetting.Inst.assetBundleResourceFolder;
		if (string.IsNullOrEmpty(assetBundleResourceFolder))
		{
			Debug.LogWarning("Check the AssetBundle Resource Folder in GiantSetting.");
			return;
		}
		string resBuildTargetFolder = Path.Combine(assetBundleResourceFolder, buildTarget.ToString().ToLower());
		if (Directory.Exists(resBuildTargetFolder))
			Directory.Delete(resBuildTargetFolder, true);
		Directory.CreateDirectory(resBuildTargetFolder);
		files = Directory.GetFiles(destBuildTargetFolder);
		int length = destBuildTargetFolder.Length;
		if (!destBuildTargetFolder.EndsWith("/"))
			length += 1;
		for (int i = 0; i < files.Length; ++i)
		{
			if (Path.GetExtension(files[i]).ToLower().Equals(".manifest"))
				continue;
			string resFileName = Path.Combine(resBuildTargetFolder, files[i].Remove(0, length));
			File.Copy(files[i], resFileName);
		}
	}

	static void SaveAssetBundleDatabase()
	{
		AssetBundleDatabase assetBundleDatabase = AssetBundleManager.assetBundleDatabase;
		if (!Directory.Exists(AssetBundleFolderName))
			Directory.CreateDirectory(AssetBundleFolderName);
		string fileName = Path.Combine(AssetBundleFolderName, Constant.AssetBundleDatabase);
		SaveFile(fileName, assetBundleDatabase);

		// 프로젝트 외부(giantres)로 파일 복사
		string assetBundleResourceFolder = GiantSetting.Inst.assetBundleResourceFolder;
		if (string.IsNullOrEmpty(assetBundleResourceFolder))
		{
			Debug.LogWarning("Check the AssetBundle Resource Folder in GiantSetting.");
			return;
		}
		string resFileName = Path.Combine(assetBundleResourceFolder, Path.Combine("raw/json", Constant.AssetBundleDatabase));
		if (File.Exists(resFileName))
			File.Delete(resFileName);
		File.Copy(fileName, resFileName);
		Debug.Log("Success!");
	}

	static void ExtractShadersFromCurrentScene(Dictionary<int, string> shaders)
	{
		// GameObject
		GameObject [] rootObjs = SceneManager.GetActiveScene().GetRootGameObjects();
		for (int i = 0; i < rootObjs.Length; ++i)
		{
			ExtractShadersFromGameObject(rootObjs[i], shaders);
		}

		// Skybox
		if (RenderSettings.skybox != null)
		{
			Shader shader = RenderSettings.skybox.shader;
			if (!shaders.ContainsKey(shader.GetInstanceID()))
			{
				shaders.Add(shader.GetInstanceID(), shader.name);
			}
		}
	}

	static void ExtractShadersFromGameObject(GameObject obj, Dictionary<int, string> shaders)
	{
		Renderer [] renderers = obj.GetComponentsInChildren<Renderer>(true);
		for (int i = 0; i < renderers.Length; ++i)
		{
			Material [] materials = renderers[i].sharedMaterials;
			for (int j = 0; j < materials.Length; ++j)
			{
				if (materials[j] == null)
					continue;
				Shader shader = materials[j].shader;
				if (shader == null || shaders.ContainsKey(shader.GetInstanceID()))
					continue;
				shaders.Add(shader.GetInstanceID(), shader.name);
			}
		}
	}

	static bool IsExistRemoveObject(Transform transform)
	{
		if (transform.name.Contains("_Remove"))
			return true;
		for (int i = 0; i < transform.childCount; ++i)
		{
			if (IsExistRemoveObject(transform.GetChild(i)))
				return true;
		}
		return false;
	}

	static bool IsExistUselessCanvasRendererObject(Transform transform)
	{
		CanvasRenderer canvasRenderer = transform.GetComponent<CanvasRenderer>();
		Graphic graphic = transform.GetComponent<Graphic>();
		if (canvasRenderer != null && graphic == null)
			return true;
		for (int i = 0; i < transform.childCount; ++i)
		{
			if (IsExistUselessCanvasRendererObject(transform.GetChild(i)))
				return true;
		}
		return false;
	}

	static void FindAllUITextEnumInPrefab(bool isWithColorTag)
	{
		List<int> idList = new List<int>();
		UITextSetter uiTextSetter;
		GameObject prefabObj;
		int textEnumInt;

		List<UITextSetter> list = STPrefabUtil.FindAllComponents<UITextSetter>();
		for (int i = 0; i < list.Count; ++i)
		{
			uiTextSetter = list[i];
			prefabObj = uiTextSetter.transform.root.gameObject;

			if (AssetDatabase.GetAssetPath(prefabObj).Contains(UIDummyPath))
				continue;

			textEnumInt = uiTextSetter.textEnumInt;
			if (idList.IndexOf(textEnumInt) < 0)
				idList.Add(textEnumInt);
		}

		idList.Sort();

		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Prefab : " + idList.Count);
		for (int i = 0; i < idList.Count; ++i)
		{
			if (isWithColorTag && IsTextEnumScriptScope(idList[i]))
				stringBuilder.AppendFormat("<color=red>{0}</color>, ", idList[i]);
			else
				stringBuilder.AppendFormat("{0}, ", idList[i]);
		}

		Debug.LogWarning(stringBuilder.ToString());
	}
	
	static void FindAllUITextEnumInScene(bool isWithColorTag)
	{
		if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			return;

		List<int> idList = new List<int>();
		UITextSetter uiTextSetter;
		int textEnumInt;

		List<string> curScenes = STSceneUtil.GetCurrentScenes();
		STSceneUtil.OpenBuildScenes();

		List<UITextSetter> list = STSceneUtil.FindAllComponents<UITextSetter>();
		for (int i = 0; i < list.Count; ++i)
		{
			uiTextSetter = list[i];

			if (PrefabUtility.GetPrefabParent(uiTextSetter.gameObject) != null)
				continue;

			textEnumInt = uiTextSetter.textEnumInt;
			if (idList.IndexOf(textEnumInt) < 0)
				idList.Add(textEnumInt);
		}

		idList.Sort();

		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Scene : " + idList.Count);
		for (int i = 0; i < idList.Count; ++i)
		{
			if (isWithColorTag && IsTextEnumScriptScope(idList[i]))
				stringBuilder.AppendFormat("<color=red>{0}</color>, ", idList[i]);
			else
				stringBuilder.AppendFormat("{0}, ", idList[i]);
		}

		Debug.LogWarning(stringBuilder.ToString());

		STSceneUtil.RestoreCurrentScenes(curScenes);
	}
	
	static bool IsTextEnumScriptScope(int textEnum)
	{
		return (textEnum / 100) % 10 >= 5 && textEnum < (int)UITextEnum.POPUP_UNLOCK_CONTENTS;
	}
}
