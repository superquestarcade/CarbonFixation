using UnityEngine;

namespace Utility
{
	public static class WrapValues
	{
		public static float Wrap(this float _value, float _max)
		{
			return ((_value % _max) + _max) % _max;
		}
		
		public static int Wrap(this int _value, int _max)
		{
			return ((_value % _max) + _max) % _max;
		}

		public static Vector2 Wrap(this Vector2 _value, float _max)
		{
			return new Vector2(_value.x.Wrap(_max), _value.y.Wrap(_max));
		}
	}
}