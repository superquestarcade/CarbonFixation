using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
	public static class MathNL
	{
		public static int[] RandomUniqueFew(int _totalCount, int _getSome, System.Random _rng)
		{
			if (_getSome > _totalCount)
			{
				Debug.LogError("RandomUniqueFew cannot return more than the requested total");
				return Array.Empty<int>();
			}
			
			var indexList = new List<int>();
			for(var c=0;c<_totalCount;c++)
				indexList.Add(c);

			if (_getSome == _totalCount)
			{
				Debug.LogWarning("RandomUniqueFew requested same as total values, this method is designed to select less than total");
				return indexList.ToArray();
			}

			var removeCount = _totalCount - _getSome;

			while (removeCount > 0)
			{
				indexList.RemoveAt(_rng.Next(indexList.Count));
				removeCount--;
			}
			
			return indexList.ToArray();
		}
	}
}