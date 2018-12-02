#if (UNITY_EDITOR && !USE_GVS) || DEV_TEST
	#define USE_SINGROUTER
#endif

using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using AL;

public class Logger : AL.Logger {}

public class Util : AL.Util {}

public class Global {
	public enum LoadState
	{
		None,
		CDS,
		Version,
		ResourceSeed,
		ResourceManager,
		DataTable,
		CheckDownload,
		HiveLogin,
		GameLogin,
	};

	private static Global _inst = new Global();
	public static Global Inst { get { return _inst; } }
	
	private LoadState _loadState = LoadState.None;
	public LoadState loadState { get { return _loadState; } set { _loadState = value; evtUpdateState(_loadState); } }

	public static Config cfg = Config.LoadResource("global", "config/global");

	public event Action evtGlobalInitialized = () => {};
	public event Action evtNetworkDisconnected = () => {};
	public event Action<LoadState> evtUpdateState = (state) => {};
	public event Action evtLoadingStart = null;
	public event Action evtLoadingEnd = null;

	public void InvokeNetworkDisconnected() { evtNetworkDisconnected(); }
	public void InvokeLoadingStart() { if(null != evtLoadingStart) evtLoadingStart(); }
	public void InvokeLoadingEnd() { if(null != evtLoadingEnd) evtLoadingEnd(); }

	public void Start() {
		Debug.Log(Logger.Write("Global.Awake"));
		BuiltInTextAdder.inst.AddBuiltInString();
		InitCDS();
	}

	public void FastUpdate() {
		RunFastTask();
	}

	public void Update() {
		RunTask();
		RunLoop();
		RunTimeout();
	}
	
	public void LateUpdate() {
		RunLateTask();
	}

	private void InitCDS() {
		Action<bool> resultAction = (isResult) => {
			if (isResult)
				InitVersion(Initialize);
			else
			{
				UITextEnum textEnum = Application.internetReachability == NetworkReachability.NotReachable ? UITextEnum.BUILTIN_NETWORK_FAIL : UITextEnum.BUILTIN_CDS_FAIL;
				Debug.LogError("InitCDS : " + textEnum);
				string msg = Datatable.Inst.GetUIText(textEnum);
				SystemPopupManager.Inst.ShowOneButtonPopup(msg, Datatable.Inst.GetUIText(UITextEnum.BUILTIN_OK)).SetClickButtonAction(InitCDS);
			}
		};

		loadState = LoadState.CDS;
		CDSManager.InitCDS(resultAction);
	}

	private void InitVersion(Action cb) {
		// OffLineTest
		cb();
		return;

		Action<bool> resultAction = (isResult) => {
			if (isResult)
			{
#if USE_SINGROUTER
				GiantHandler.Inst.Init(SingRouterHandler.Inst.serverURL);
#else
				GiantHandler.Inst.Init(VersionManager.Inst.serverURL);
#endif
				cb();
			}
			else
			{
				// 에러 메세지는 서버 분기랑 같은 걸 사용한다.
				UITextEnum textEnum = Application.internetReachability == NetworkReachability.NotReachable ? UITextEnum.BUILTIN_NETWORK_FAIL : UITextEnum.BUILTIN_CDS_FAIL;
				Debug.LogError("InitVersion : " + textEnum);
				string msg = Datatable.Inst.GetUIText(textEnum);
				SystemPopupManager.Inst.ShowOneButtonPopup(msg, Datatable.Inst.GetUIText(UITextEnum.BUILTIN_OK)).SetClickButtonAction(() => { InitVersion(cb); });
			}
		};

		loadState = LoadState.Version;

#if USE_SINGROUTER
		string projectCode = cfg.GetString("projectCode", "");
		string game = cfg.GetString("game", "");
		string version = cfg.GetString("version", "");

		SingRouterHandler.Inst.doSingServerInfo(projectCode, game, version, (obj) => {
			if(null != obj)
			{
				Debug.LogError("SingRouterHandler.doSingServerInfo : " + obj.ToString());
				resultAction(false);
			}
			else
				resultAction(true);
		});
#else
		VersionManager.Inst.Init(resultAction);
#endif
	}

	private void Initialize() {
#if UNITY_EDITOR || HIDDEN_LOGIN_TEST
		CheckVersion();
#else
		HiveManager.Inst.InitHive((success) => {
			if(success)
				CheckMaintenance();
			else
				Initialize();
		});
#endif
	}

	private void CheckMaintenance() {
		HiveManager.Inst.CheckMaintenance((success) => {
			if(success)
				CheckVersion();
			else
				CheckMaintenance();
		});
	}
	
	private void CheckVersion()
	{
		// 버전 파일을 로딩하는 시점과 버전 체크하는 시점이 다르다.
		if (VersionManager.Inst.Check())
			InitResourceData();
		else
			SystemPopupManager.Inst.ShowOneButtonPopup(Datatable.Inst.GetUIText(UITextEnum.BUILTIN_VERSION_FAIL), Datatable.Inst.GetUIText(UITextEnum.BUILTIN_OK)).SetClickButtonAction(() => { InitVersion(CheckVersion); });
	}

	public void InitResourceData() {
		Debug.Log(Logger.Write("Global.InitResourceData"));

		InitResourceData(() => {
			Debug.Log(Logger.Write("Global.InitResourceData.Done"));
			evtGlobalInitialized();
		}, () => {
			Debug.LogError(Logger.Write("Global.InitResourceData.Failure"));
			InitVersion(CheckVersion);
		}, false);
	}

	public void InitResourceData(Action InitSuccess, Action InitFailure, bool isReload) {
		GlobalManager.Inst.StartCoroutine(DoInitResourceData(InitSuccess, InitFailure, isReload));
	}

	private IEnumerator DoInitResourceData(Action InitSuccess, Action InitFailure, bool isReload) {
		Action<bool> failureAction = (isByServer) => {
			InvokeLoadingEnd();
			UITextEnum textEnum = isByServer ? UITextEnum.BUILTIN_SERVER_FAIL : UITextEnum.BUILTIN_DATA_FAIL;
			Debug.LogError("DoInitResourceData : " + textEnum);
			string msg = Datatable.Inst.GetUIText(textEnum);
			SystemPopupManager.Inst.ShowOneButtonPopup(msg, Datatable.Inst.GetUIText(UITextEnum.BUILTIN_OK)).SetClickButtonAction(() => { if(null != InitFailure) { InitFailure(); } });
			ResourceManager.Inst.ResetResID();
		};
		Action successAction = () => {
			// OffLineTest
			GiantHandler.Inst.OfflineEvt();
			InvokeLoadingEnd();
			if(null != InitSuccess) InitSuccess();
		};

		InvokeLoadingStart();

		object result = null;

		loadState = LoadState.ResourceSeed;
		// OffLineTest
//		yield return GiantHandler.Inst.doServerList().SetFailureAction((error) => { result = error; }).WaitForResponse();

		if(null != result) { failureAction(true); yield break; }
		
		string resourceURL = GiantHandler.Inst.resourceURL;
		string resourceID = GiantHandler.Inst.resourceID;

		PersistentStore.Init(resourceURL);

		loadState = LoadState.ResourceManager;
		
		Util.DelayDone dd = new Util.DelayDone();
		ResourceManager.Inst.Init(resourceURL, resourceID, dd.Wrap<bool, bool>((isFailure, isNewData) => {
			if(isFailure) { dd.SetFailure(isFailure); return; }

			loadState = LoadState.DataTable;
			string defaultLanguage = cfg.GetString("defaultLanguage", Language.EN);
			string language = DeviceSaveManager.Inst.GetLanguage();

			if(isNewData)
			{
				Datatable.Inst.Load(defaultLanguage, language, dd.Wrap<bool>((failure) => { dd.SetFailure(failure); }));
			}
		}));
		
		dd.Done((failure) => {
			if(failure) { failureAction(false); }
			else successAction();
		});
		// OffLineTest
		yield return null;
	}
	
	public void ChangeLanguage(Action cb)
	{
		string language = DeviceSaveManager.Inst.GetLanguage();
		if(Datatable.Inst.currentLanguage == language)
			cb();
		else
		{
			string defaultLanguage = cfg.GetString("defaultLanguage", Language.EN);
			Datatable.Inst.LoadLanguage(defaultLanguage, language, cb);
		}
	}
	
	public event Action evLoop;
	private void RunLoop() {
		if (null != evLoop) evLoop();
	}

	// FastRunTask, RunTask, LateRunTask 중 새로운 Task가 등록되는 경우를 위해 tempTaskList를 번갈아가면서 사용한다.
	private List<Action> tempTaskList = new List<Action>();

	// FastTask
	private List<Action> FastTaskList = new List<Action>();
	public void PushFastTask(Action cb) {
		lock (this) {
			FastTaskList.Add(cb);
		}
	}
	private void RunFastTask() {
		lock (this) {
			if (FastTaskList.Count <= 0) return;
			SwapTask(ref FastTaskList, ref tempTaskList);
		}
		RunTempTask();
	}

	// Normal Task
	private List<Action> TaskList = new List<Action>();
	public void PushTask(Action cb) {
		lock (this) {
			TaskList.Add(cb);
		}
	}
	private void RunTask() {
		lock (this) {
			if (TaskList.Count <= 0) return;
			SwapTask(ref TaskList, ref tempTaskList);
		}
		RunTempTask();
	}

	// Late Task
	private List<Action> LateTaskList = new List<Action>();
	public void PushLateTask(Action cb) {
		lock (this) {
			LateTaskList.Add(cb);
		}
	}
	private void RunLateTask() {
		lock (this) {
			if (LateTaskList.Count <= 0) return;
			SwapTask(ref LateTaskList, ref tempTaskList);
		}
		RunTempTask();
	}

	private void SwapTask(ref List<Action> taskList1, ref List<Action> taskList2) {
		List<Action> tl = taskList1;
		taskList1 = taskList2;
		taskList2 = tl;
	}
	private void RunTempTask() {
		if (tempTaskList.Count <= 0) return;
		for (int i = 0; i < tempTaskList.Count; i++)
			tempTaskList[i]();
		tempTaskList.Clear();
	}

	class TimeoutElement {
		public long time;
		public Action cb;
	}
	private List<TimeoutElement> TimeoutList = new List<TimeoutElement>();
	public void PushTimeout(long delay, Action cb) {
		TimeoutElement e = new TimeoutElement();
		e.time = Util.Millis + delay;
		e.cb = cb;
		lock (this) {
			TimeoutList.Add(e);
		}
	}
	private List<TimeoutElement> tempTimeoutList = new List<TimeoutElement>();
	private void RunTimeout() {
		long time = Util.Millis;
		lock (this) {
			int count = TimeoutList.Count;
			for (int i = 0; i < count; ) {
				TimeoutElement e = TimeoutList[i];
				if (e.time <= time) {
					count--;
					TimeoutList.RemoveAt(i);
					tempTimeoutList.Add(e);
				} else {
					i++;
				}
			}
		}
		tempTimeoutList.ForEach((e) => e.cb());
		tempTimeoutList.Clear();
	}

	static public int threadMaxCount = 1;
	public int threadCount = 0;
	public Queue<Action> threadTasks = new Queue<Action>();
	public void ThreadProc() {
		for (;;) {
			Action task;
			lock (this) {
				if (threadTasks.Count <= 0) break;
				task = threadTasks.Dequeue();
			}
			task();
		}
		lock (this) {
			--threadCount;
		}
	}
	public void PushThreadTask(Action cb) {
		lock (this) {
			threadTasks.Enqueue(cb);
			if (threadCount < threadMaxCount) {
				++threadCount;
				Thread th = new Thread(new ThreadStart(ThreadProc));
				th.Priority = System.Threading.ThreadPriority.Lowest;
				th.Start();
			}
		}
	}
	public void InstantiateThread(Action cb)
	{
		Thread th = new Thread(new ThreadStart(cb));
		th.Start();
	}
};

public class GlobalManager : MonoBehaviour {
	private static GlobalManager _inst = null;
	public static GlobalManager Inst { get { return _inst; } }

	public void Awake() {
		this.useGUILayout = false;
		_inst = this;
	}
	public void Start() {
		Global.Inst.Start();
	}
	public void Update() {
		Global.Inst.FastUpdate();
		Invoke("GUpdate", 0);
	}
	public void LateUpdate() {
		Global.Inst.LateUpdate();
	}
	public void GUpdate() {
		Global.Inst.Update();
	}
};
