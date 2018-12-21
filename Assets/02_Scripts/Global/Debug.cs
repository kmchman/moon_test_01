#if !UNITY_EDITOR
using UnityEngine;

using System.Collections;
using System.Diagnostics;

using UnityDebug = UnityEngine.Debug;

public class Debug
{
	// break
	[Conditional("UNITY_EDITOR"), Conditional("DONT_MUTE_LOG")]
	public static void Break()																	{ UnityDebug.Break(); }
	
	// log
	[Conditional("UNITY_EDITOR"), Conditional("DONT_MUTE_LOG")]
	public static void Log(object message)														{ UnityDebug.Log(message); }
	[Conditional("UNITY_EDITOR"), Conditional("DONT_MUTE_LOG")]
	public static void Log(object message, Object context)										{ UnityDebug.Log(message, context); }
	[Conditional("UNITY_EDITOR"), Conditional("DONT_MUTE_LOG")]
	public static void LogFormat(string format, params object[] args)							{ UnityDebug.LogFormat(format, args); }
	[Conditional("UNITY_EDITOR"), Conditional("DONT_MUTE_LOG")]
	public static void LogFormat(Object context, string format, params object[] args)			{ UnityDebug.LogFormat(context, format, args); }
	
	// warning
	[Conditional("UNITY_EDITOR"), Conditional("DONT_MUTE_LOG")]
	public static void LogWarning(object message)												{ UnityDebug.LogWarning(message); }
	[Conditional("UNITY_EDITOR"), Conditional("DONT_MUTE_LOG")]
	public static void LogWarning(object message, Object context)								{ UnityDebug.LogWarning(message, context); }
	[Conditional("UNITY_EDITOR"), Conditional("DONT_MUTE_LOG")]
	public static void LogWarningFormat(string format, params object[] args)					{ UnityDebug.LogWarningFormat(format, args); }
	[Conditional("UNITY_EDITOR"), Conditional("DONT_MUTE_LOG")]
	public static void LogWarningFormat(Object context, string format, params object[] args)	{ UnityDebug.LogWarningFormat(context, format, args); }
	
	// error
	[Conditional("UNITY_EDITOR"), Conditional("DONT_MUTE_LOG")]
	public static void LogError(object message)													{ UnityDebug.LogError(message); }
	[Conditional("UNITY_EDITOR"), Conditional("DONT_MUTE_LOG")]
	public static void LogError(object message, Object context)									{ UnityDebug.LogError(message, context); }
	[Conditional("UNITY_EDITOR"), Conditional("DONT_MUTE_LOG")]
	public static void LogErrorFormat(string format, params object[] args)						{ UnityDebug.LogErrorFormat(format, args); }
	[Conditional("UNITY_EDITOR"), Conditional("DONT_MUTE_LOG")]
	public static void LogErrorFormat(Object context, string format, params object[] args)		{ UnityDebug.LogErrorFormat(context, format, args); }
	
	// exception
	[Conditional("UNITY_EDITOR"), Conditional("DONT_MUTE_LOG"), Conditional("DEV_TEST"), Conditional("QA_TEST")]
	public static void LogException(System.Exception exception)									{ UnityDebug.LogException(exception); }
	[Conditional("UNITY_EDITOR"), Conditional("DONT_MUTE_LOG"), Conditional("DEV_TEST"), Conditional("QA_TEST")]
	public static void LogException(System.Exception exception, Object context)					{ UnityDebug.LogException(exception, context); }
}
#endif