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
					Debug.Log($"GenerateHexPoints ({r},{q}) rngDistance: {rngDistance}, randomizedPoint: {randomizedPoint}");
					point += randomizedPoint;
				}

				points.Add(point);
			}
		}

		return points;
	}
	
	/// <summary>
    /// Converts a world-space point to the nearest hex grid position
    /// (flat-top layout, matching GenerateHexPoints).
    /// </summary>
    public static Vector2 WorldToNearestHexPoint(Vector2 worldPos, float spacing)
    {
        // Invert the axial -> world formulas to get fractional axial coords
        float qFrac = worldPos.x / (1.5f * spacing);
        float rFrac = (worldPos.y / (spacing * Mathf.Sqrt(3f))) - (qFrac / 2f);

        // Convert to cube coordinates for correct rounding
        float xFrac = qFrac;
        float zFrac = rFrac;
        float yFrac = -xFrac - zFrac;

        float xRound = Mathf.Round(xFrac);
        float yRound = Mathf.Round(yFrac);
        float zRound = Mathf.Round(zFrac);

        float xDiff = Mathf.Abs(xRound - xFrac);
        float yDiff = Mathf.Abs(yRound - yFrac);
        float zDiff = Mathf.Abs(zRound - zFrac);

        // Fix the coordinate with the largest rounding error
        // so x + y + z == 0 is preserved
        if (xDiff > yDiff && xDiff > zDiff)
            xRound = -yRound - zRound;
        else if (yDiff > zDiff)
            yRound = -xRound - zRound;
        else
            zRound = -xRound - yRound;

        int q = (int)xRound;
        int r = (int)zRound;

        // Axial -> world position (same formula as generator)
        float x = spacing * (1.5f * q);
        float y = spacing * (Mathf.Sqrt(3f) * (r + q / 2f));

        return new Vector2(x, y);
    }

    /// <summary>
    /// Same as above, but also returns the axial (q, r) coordinates
    /// in case you need them for lookups, neighbor checks, etc.
    /// </summary>
    public static Vector2Int WorldToNearestHexCoord(Vector2 worldPos, float spacing)
    {
        float qFrac = worldPos.x / (1.5f * spacing);
        float rFrac = (worldPos.y / (spacing * Mathf.Sqrt(3f))) - (qFrac / 2f);

        float xFrac = qFrac;
        float zFrac = rFrac;
        float yFrac = -xFrac - zFrac;

        float xRound = Mathf.Round(xFrac);
        float yRound = Mathf.Round(yFrac);
        float zRound = Mathf.Round(zFrac);

        float xDiff = Mathf.Abs(xRound - xFrac);
        float yDiff = Mathf.Abs(yRound - yFrac);
        float zDiff = Mathf.Abs(zRound - zFrac);

        if (xDiff > yDiff && xDiff > zDiff)
            xRound = -yRound - zRound;
        else if (yDiff > zDiff)
            yRound = -xRound - zRound;
        else
            zRound = -xRound - yRound;

        return new Vector2Int((int)xRound, (int)zRound);
    }
}