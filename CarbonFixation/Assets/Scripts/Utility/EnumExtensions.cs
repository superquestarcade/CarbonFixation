using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utility
{
	public static class EnumExtensions
	{
		public static T[] AllTypes<T>() where T : Enum
		{
			var allNeeds = Enum.GetValues(typeof(T)) as T[];
			Debug.Assert(allNeeds != null, nameof(allNeeds) + " != null");
			return allNeeds;
		}

		public static T[] ReturnSelectedTypes<T>(this T _needTypes) where T : Enum
		{
			var enumValues = Enum.GetValues(typeof(T)); // Todo: Performance heavy
			var selectedElements = new T[enumValues.Length];
			var count = 0;
			foreach (T value in enumValues)
			{
				if (!_needTypes.HasFlag(value)) continue;
				selectedElements[count] = value;
				count++;
			}
			Array.Resize(ref selectedElements, count);
			return selectedElements;
		}
	}
}