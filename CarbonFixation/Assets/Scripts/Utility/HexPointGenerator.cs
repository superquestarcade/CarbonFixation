using UnityEngine;
using System.Collections.Generic;

public static class HexPointGenerator
{
	/// <summary>
	/// Generates points arranged in a hexagonal grid pattern.
	/// </summary>
	/// <param name="rings">Number of rings around the center (0 = just center point)</param>
	/// <param name="spacing">Distance between adjacent points</param>
	/// <param name="randomOffset">Max random displacement applied to each point</param>
	public static List<Vector2> GenerateHexPoints(int rings, float spacing, float randomOffset, System.Random rng)
	{
		List<Vector2> points = new List<Vector2>();

		for (int q = -rings; q <= rings; q++)
		{
			int r1 = Mathf.Max(-rings, -q - rings);
			int r2 = Mathf.Min(rings, -q + rings);

			for (int r = r1; r <= r2; r++)
			{
				// Axial -> world position (flat-top hexagon layout)
				float x = spacing * (1.5f * q);
				float y = spacing * (Mathf.Sqrt(3f) * (r + q / 2f));

				Vector2 point = new Vector2(x, y);

				if (randomOffset > 0f)
				{
					var rngDistance = (float) rng.NextDouble() * randomOffset;
					var randomizedPoint = new Vector2((float) rng.NextDouble(),  (float)rng.NextDouble()).normalized * rngDistance;
					point += randomizedPoint;
				}

				points.Add(point);
			}
		}

		return points;
	}
}