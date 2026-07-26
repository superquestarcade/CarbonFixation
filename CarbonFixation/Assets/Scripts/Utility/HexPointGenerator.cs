using UnityEngine;
using System.Collections.Generic;

public static class HexPointGenerator
{
	/// <summary>
	/// Generates points arranged in a hexagonal grid pattern.
	/// </summary>
	/// <param name="rings">Number of rings around the center (0 = just center point)</param>
	/// <param name="spacing">Distance between adjacent points</param>
	public static List<Vector2> GenerateHexPoints(int rings, float spacing)
	{
		List<Vector2> points = new List<Vector2>();
		float adjustedSpacing = spacing / Mathf.Sqrt(3f);

		for (int q = -rings; q <= rings; q++)
		{
			int r1 = Mathf.Max(-rings, -q - rings);
			int r2 = Mathf.Min(rings, -q + rings);

			for (int r = r1; r <= r2; r++)
			{
				// Skip every 3rd point (the "hex center" sub-lattice)
				// to turn the triangular lattice into a honeycomb pattern
				int mod = ((q - r) % 3 + 3) % 3; // safe mod for negative values
				if (mod == 0) continue;
				
				// Axial -> world position (flat-top hexagon layout)
				float x = adjustedSpacing * (1.5f * q);
				float y = adjustedSpacing * (Mathf.Sqrt(3f) * (r + q / 2f));

				Vector2 point = new Vector2(x, y);

				points.Add(point);
			}
		}

		return points;
	}
	
	// Todo: These formula below might return triangle grid coordinates as they were written before fixing above
	
	/// <summary>
    /// Converts a world-space point to the nearest hex grid position
    /// (flat-top layout, matching GenerateHexPoints).
    /// </summary>
    public static Vector2 WorldToNearestHexPoint(Vector2 worldPos, float spacing)
    {
	    float adjustedSpacing = spacing / Mathf.Sqrt(3f);
	    
        // Invert the axial -> world formulas to get fractional axial coords
        float qFrac = worldPos.x / (1.5f * adjustedSpacing);
        float rFrac = (worldPos.y / (adjustedSpacing * Mathf.Sqrt(3f))) - (qFrac / 2f);

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
        float x = adjustedSpacing * (1.5f * q);
        float y = adjustedSpacing * (Mathf.Sqrt(3f) * (r + q / 2f));

        return new Vector2(x, y);
    }

    /// <summary>
    /// Same as above, but also returns the axial (q, r) coordinates
    /// in case you need them for lookups, neighbor checks, etc.
    /// </summary>
    public static Vector2Int WorldToNearestHexCoord(Vector2 worldPos, float spacing)
    {
	    float adjustedSpacing = spacing / Mathf.Sqrt(3f);
	    
        float qFrac = worldPos.x / (1.5f * adjustedSpacing);
        float rFrac = (worldPos.y / (adjustedSpacing * Mathf.Sqrt(3f))) - (qFrac / 2f);

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