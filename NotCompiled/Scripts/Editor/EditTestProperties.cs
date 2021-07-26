using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;
using System;
using System.Reflection;

[CustomEditor(typeof(TestProperties))]
public class EditTestProperties : Editor {

	public override void OnInspectorGUI ()
	{
		var iterator = serializedObject.GetIterator();
		SerializedProperty prop;
		while(iterator.NextVisible(true))
		{
			var parent = GetParent(iterator);
			if(parent!=null)
				Debug.Log(iterator.propertyPath + " parent " + parent);
			else
				Debug.Log(iterator.propertyPath + " has no parent");
		}
		
		
		
		
	}
	
	public object GetParent(SerializedProperty prop)
	{
		var path = prop.propertyPath.Replace(".Array.data[", "[");
		object obj = prop.serializedObject.targetObject;
		var elements = path.Split('.');
		foreach(var element in path.Split('.').Take(elements.Length-1))
		{
			if(element.Contains("["))
			{
				var elementName = element.Substring(0, element.IndexOf("["));
				var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[","").Replace("]",""));
				obj = GetValue(obj, elementName, index);
			}
			else
			{
				obj = GetValue(obj, element);
			}
		}
		return obj;
	}
	
	public object GetValue(object source, string name)
	{
		if(source == null)
			return null;
		var type = source.GetType();
		var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
		if(f == null)
		{
			var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
			if(p == null)
				return null;
			return p.GetValue(source, null);
		}
		return f.GetValue(source);
	}
	
	public object GetValue(object source, string name, int index)
	{
		var enumerable = GetValue(source, name) as IEnumerable;
		var enm = enumerable.GetEnumerator();
		while(index-- >= 0)
			enm.MoveNext();
		return enm.Current;
	}
	
}
