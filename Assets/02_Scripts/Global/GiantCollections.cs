using UnityEngine;

using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using AL;

public interface IGiantNetElement
{
	bool UpdateValues(IDictionary value, bool isClear);
}

public class GiantNetElement : MemberToString, IGiantNetElement
{
	public virtual void OnNew() {}
	public virtual void OnUpdate() {}

	public virtual void OnSetKey(object key)
	{
		if(!SetKey(key))
			Debug.LogError(Util.LogFormat("Error GiantNetElement.SetKey ", key, GetType().Name));
	}

	public virtual bool UpdateValues(IDictionary value, bool isClear)
	{
		bool isDirty = false;

		FieldInfo field = null;
		FieldInfo[] fields = NetCollectionsUtil.GetFieldInfos(GetType());

		for (int i = 0; i < fields.Length; i++)
		{
			field = fields[i];

			if (value.Contains(field.Name))
				isDirty |= NetCollectionsUtil.SetField(field, this, value[field.Name], isClear);
			else if (isClear)
				isDirty |= NetCollectionsUtil.SetField(field, this, null, isClear);
		}

		OnUpdate();

		return isDirty;
	}

	protected virtual bool SetKey(object key) { return true; }
}

public interface IGiantList : IList, IGiantNetElement
{
	// TODO : GiantNetElement 클래스의 맴버 변수 타입중 GiantList를 사용 할 예정이라면 아래 함수를 구현해야 할듯
	// 그후 추가로 NetCollectionsUtil의 SetField에서 처리 가능하게 구현
 	//public void UpdateValues(IList value, bool isClear);
}

public class GiantList<ValueType> : List<ValueType>, IGiantList
{
	private static Hashtable _TempHashtable = new Hashtable();

	public Action<ValueType> newDelegate = null;
	public Action<ValueType, bool> updateDelegate = null;

	private void InvokeNewDelegate(ValueType value)
	{
		if (value is GiantNetElement)
			(value as GiantNetElement).OnNew();

		if (newDelegate != null)
			newDelegate(value);
	}

	private void InvokeUpdateDelegate(ValueType value, bool isRemoveValue)
	{
		if (value != null && isRemoveValue)
		{
			if (value is GiantNetElement)
				(value as GiantNetElement).OnUpdate();
		}

		if (updateDelegate != null)
			updateDelegate(value, isRemoveValue);
	}

	public virtual bool UpdateValues(IDictionary value, bool isClear)
	{
		_TempHashtable.Clear();

		var enumerator = value.GetEnumerator();
		while (enumerator.MoveNext())
			_TempHashtable.Add(enumerator.Key.ToString(), enumerator.Value);

		if (isClear) 
		{
			var enumerator2 = GetEnumerator();
			while (enumerator2.MoveNext())
				if(!_TempHashtable.Contains(enumerator2.Current.ToString()))
					_TempHashtable[enumerator2.Current.ToString()] = null;
		}

		bool isDirty = false;

		enumerator = _TempHashtable.GetEnumerator();
		while (enumerator.MoveNext())
		{
			ValueType objectValue = (ValueType)Util.CastValue(typeof(ValueType), enumerator.Key);
			bool isRemoveValue = (enumerator.Value == null);

			if (UpdateValue(objectValue, isRemoveValue, isClear))
			{
				isDirty = true;
			}
			else if (!isRemoveValue)
			{
				AddValue(objectValue);
				isDirty = true;
			}
		}

		return isDirty;
	}

	private bool UpdateValue(ValueType value, bool isRemoveValue, bool isClear)
	{
		ValueType objectValue = (ValueType)Util.CastValue(typeof(ValueType), value);
		int index = IndexOf(objectValue);

		if (index < 0)
			return isRemoveValue;

		if (isRemoveValue)
		{
			RemoveAt(index);
		}
		else
		{
			Type objectType = typeof(ValueType);
			if (objectType.IsValueType)
				this[index] = objectValue;
			else	
				NetCollectionsUtil.SetFields(this[index], value, isClear);
		}

		InvokeUpdateDelegate(value, isRemoveValue);

		return true;
	}

	private void AddValue(ValueType value)
	{
		Add(value);
		InvokeNewDelegate(value);
		InvokeUpdateDelegate(value, false);
	}

	public override string ToString()
	{
		return Util.JsonEncode2(this);
	}
}

public interface IGiantDictionary : IDictionary, IGiantNetElement
{
}

public class GiantDictionary<KeyType, ValueType> : Dictionary<KeyType, ValueType>, IGiantDictionary
{
	private static Hashtable _TempHashtable = new Hashtable();

	public Action<KeyType, ValueType> newDelegate = null;
	public Action<KeyType, ValueType> updateDelegate = null;

	private void InvokeNewDelegate(KeyType key, ValueType value)
	{
		if (value is GiantNetElement)
			(value as GiantNetElement).OnNew();
		
		if (newDelegate != null)
			newDelegate(key, value);
	}

	private void InvokeUpdateDelegate(KeyType key, ValueType value)
	{
		if (value != null)
			if (value is GiantNetElement)
				(value as GiantNetElement).OnUpdate();

		if (updateDelegate != null)
			updateDelegate(key, value);
	}

	public new void Clear()
	{
		List<KeyType> keys = new List<KeyType>();

		var enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			keys.Add(enumerator.Current.Key);
		}

		for (int i = 0; i < keys.Count; i++)
		{
			UpdateValue(keys[i], null);
		}

		base.Clear();
	}

	public virtual bool UpdateValues(IDictionary value, bool isClear)
	{
		_TempHashtable.Clear();

		var enumerator = value.GetEnumerator();
		while (enumerator.MoveNext())
			_TempHashtable.Add(enumerator.Key.ToString(), enumerator.Value);

		if (isClear) 
		{
			enumerator = GetEnumerator();
			while (enumerator.MoveNext())
				if(!_TempHashtable.Contains(enumerator.Key.ToString()))
					_TempHashtable[enumerator.Key.ToString()] = null;
		}

		enumerator = _TempHashtable.GetEnumerator();
		while (enumerator.MoveNext())
			UpdateValue(enumerator.Key, enumerator.Value, isClear);

		return _TempHashtable.Count > 0;
	}

	public void UpdateValue(object keyObject, object valueObject) { UpdateValue(keyObject, valueObject, false); }
	public void UpdateValue(object keyObject, object valueObject, bool isClear)
	{
		Type keyType = typeof(KeyType);
		KeyType key;
		if (keyType != typeof(string))
		{
			key = (KeyType)keyType.InvokeMember(
				"Parse",
				BindingFlags.Default | BindingFlags.InvokeMethod,
				null,
				null,
				new object[] { keyObject.ToString() });
		}
		else
		{
			key = (KeyType)keyObject;
		}

		if (null == valueObject)
		{
			Remove(key);
			InvokeUpdateDelegate(key, default(ValueType));
			return;
		}

		Type valueType = typeof(ValueType);
		ValueType value;

		bool isNew = !ContainsKey(key);

		if (valueType.IsValueType)
		{
			value = (ValueType)Util.CastValue(valueType, valueObject);
		}
		else
		{
			if (ContainsKey(key))
			{
				value = this[key];
			}
			else
			{
				MethodInfo info = valueType.GetMethod("Create");
				if (null != info)
					value = this[key] = (ValueType)info.Invoke(null, new object[] { valueObject });
				else
					value = this[key] = (ValueType)valueType.GetConstructor(new Type[] {}).Invoke(null);

				if (value is GiantNetElement)
					(value as GiantNetElement).OnSetKey(key);
			}
				
			NetCollectionsUtil.SetFields(value, valueObject, isClear);
		}

		this[key] = value;

		if(isNew)
			InvokeNewDelegate(key, value);
		InvokeUpdateDelegate(key, value);
	}

	public override string ToString()
	{
		return Util.JsonEncode2(this);
	}
}

public class NetCollectionsUtil
{
	private static Dictionary<string, FieldInfo[]> _FieldInfoDictionary = new Dictionary<string, FieldInfo[]>();

	private static Type TypeOfIGNetDictionary = typeof(IGiantDictionary);
	private static Type TypeOfGiantNetElement = typeof(GiantNetElement);

	public static FieldInfo[] GetFieldInfos(Type type)
	{
		if (!_FieldInfoDictionary.ContainsKey(type.FullName)) 
			_FieldInfoDictionary[type.FullName] = type.GetFields();

		return _FieldInfoDictionary[type.FullName];
	}

	public static void SetFields<ValueType>(ValueType obj, object value, bool isClear)
	{
		IDictionary valueDictionary = (IDictionary)value;
		Type valueType = typeof(ValueType);

		FieldInfo[] fields = GetFieldInfos(valueType);

		for (int i = 0; i < fields.Length; i++)
		{
			FieldInfo field = fields[i];

			if (!valueDictionary.Contains(field.Name))
				continue;

			SetField(field, obj, valueDictionary[field.Name], isClear);
		}
	}

	public static bool SetField(FieldInfo field, object obj, object value, bool isClear)
	{
		Type fieldType = field.FieldType;

		try
		{
			if (value == null)
			{
				if (TypeOfIGNetDictionary.IsAssignableFrom(fieldType))
					((IGiantDictionary)field.GetValue(obj)).Clear();
				else
					field.SetValue(obj, null);
			}
			else if (TypeOfIGNetDictionary.IsAssignableFrom(fieldType))
			{
				IGiantDictionary dictionary = ((IGiantDictionary)field.GetValue(obj));
				if (dictionary == null)
				{
					dictionary = (IGiantDictionary)Activator.CreateInstance(fieldType);
					field.SetValue(obj, dictionary);
				}

				dictionary.UpdateValues((IDictionary)value, isClear);
			}
			else if (TypeOfGiantNetElement.IsAssignableFrom(fieldType))
			{
				bool isNew = false;
				GiantNetElement element = ((GiantNetElement)field.GetValue(obj));
				if (element == null)
				{
					element = (GiantNetElement)Activator.CreateInstance(fieldType);
					field.SetValue(obj, element);
					isNew = true;
				}

				element.UpdateValues((IDictionary)value, isClear);

				if (isNew)
					element.OnNew();
			}
			else
			{
				object fieldValue = Util.CastValue(fieldType, value);
				field.SetValue(obj, fieldValue);
			}

			return true;
		}
		catch (Exception exception)
		{
			Debug.LogError(Util.LogFormat("GNetDictionary.SetFields", exception, field.Name, field.FieldType, value, value.GetType(), obj));
		}

		return false;
	}
}
