using UnityEngine;

namespace Utility
{
	public static class LayermaskUtils
	{
		public static bool IsInLayerMask(this LayerMask _layerMask, int _layer)
		{
			return (_layerMask.value & (1 << _layer)) != 0;
		}
	}
}