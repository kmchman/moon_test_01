using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using AL;

public class Logger : AL.Logger{}

public class Util : AL.Util {}

public class Global {

	public static string[] dtList = new string[] {
		"CharData", 
		"HeroData",
        "BuildingData2",
    };

	public enum LoadState
	{
		None,
		SingRouter,
		ResourceSeed,
		ResourceManager,
		DataTable,
		CheckDownload,
		CheckVersion,
		Signup,
		Login,
		Join,
		OK,
	};

	private static Global _inst = new Global();
	public static Global Inst { get { return _inst; } }

//	public static Config cfg = Config.LoadResource("global", "config/global");

	public void Start()
	{
		Debug.Log("public void Start()");
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
		--threadCount;
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

	void Initialize()
	{

	}

	void InitResourceData(Action initSuccess)
	{
		
	}

//	IEnumerator DoInitResourceData(Action InitSuccess, Action InitFailure, bool isReload)
//	{
//		CoroutineHelper ch = new CoroutineHelper();
//
//		string projectCode = cfg.GetString("projectCode", "");
//		string game = cfg.GetString("game", "");
//		string version = cfg.GetString("version", "");
//
//		object result = null;
//
//		yield return null;
//	}

}


public class GlobalManager : MonoBehaviour {

	private static GlobalManager _inst = null;
	public static GlobalManager Inst { get { return _inst; } }

	void Awake()
	{
		_inst = this;
	}

	public void Start() {
		Global.Inst.Start();
	}
}

