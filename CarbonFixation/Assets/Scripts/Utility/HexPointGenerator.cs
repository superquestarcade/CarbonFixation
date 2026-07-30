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
	
	public static Vector2Int[] GenerateHexOuterPoints(Vector2Int _origin, float _spacing, Vector2 _area, out Vector2[] _worldPositions)
	{
		// Basis vectors of the triangular lattice containing both hex centers and hex vertices.
		// Lattice point (a, b) -> world position = a*u + b*v
		Vector2 u = new Vector2(_spacing, 0f);
		Vector2 v = new Vector2(_spacing * 0.5f, _spacing * Mathf.Sqrt(3f) * 0.5f);

		// How far out (in lattice steps) to search to cover the area, plus margin.
		int aRange = Mathf.CeilToInt(_area.x / _spacing);
		int bRange = Mathf.CeilToInt(_area.y / (_spacing * Mathf.Sqrt(3f) * 0.5f));

		List<Vector2Int> coords = new List<Vector2Int>();
		List<Vector2> positions = new List<Vector2>();
		Vector2 halfArea = _area * 0.5f;

		for (int a = -aRange; a <= aRange; a++)
		{
			for (int b = -bRange; b <= bRange; b++)
			{
				// residue 0 = hex CENTER point -> skip it, we only want the 6 outer corners
				int residue = ((a - b) % 3 + 3) % 3;
				if (residue == 0)
					continue;

				Vector2 worldPos = a * u + b * v;

				if (Mathf.Abs(worldPos.x) > halfArea.x || Mathf.Abs(worldPos.y) > halfArea.y)
					continue;

				coords.Add(new Vector2Int(a, b) + _origin);
				positions.Add(worldPos);
			}
		}

		_worldPositions = positions.ToArray();
		return coords.ToArray();
	}

	public static Vector2 HexToWorldPos(this Vector2Int _value, float _spacing)
	{
		Vector2 u = new Vector2(_spacing, 0f);
		Vector2 v = new Vector2(_spacing * 0.5f, _spacing * Mathf.Sqrt(3f) * 0.5f);

		return _value.x * u + _value.y * v;
	}
	
	public static Vector2Int[] FilterHexPointsByNoise(
		Vector2Int[] _coords,
		Vector2[] _positions,
		float _noiseScale,
		float _threshold,
		out Vector2[] _filteredPositions,
		Vector2 _noiseOffset = default)
	{
		List<Vector2Int> keptCoords = new List<Vector2Int>();
		List<Vector2> keptPositions = new List<Vector2>();

		for (int i = 0; i < _coords.Length; i++)
		{
			// Offset sample point away from (0,0) to avoid Perlin's mirror symmetry around the origin
			float sampleX = (_positions[i].x + _noiseOffset.x) * _noiseScale + 1000f;
			float sampleY = (_positions[i].y + _noiseOffset.y) * _noiseScale + 1000f;

			float noiseValue = Mathf.PerlinNoise(sampleX, sampleY);
			
			// Debug.Log($"FilterHexPointsByNoise position: {_positions[i]}, noiseValue: {noiseValue}");

			if (noiseValue >= _threshold)
			{
				keptCoords.Add(_coords[i]);
				keptPositions.Add(_positions[i]);
			}
		}

		_filteredPositions = keptPositions.ToArray();
		return keptCoords.ToArray();
	}

	public static Vector2Int WorldToNearestHexVertex(float _spacing, Vector2 _worldPosition)
	{
		float sqrt3 = Mathf.Sqrt(3f);

		Vector2 u = new Vector2(_spacing, 0f);
		Vector2 v = new Vector2(_spacing * 0.5f, _spacing * sqrt3 * 0.5f);

		// Inverse of the (u, v) basis matrix, applied to _worldPosition
		float aFrac = _worldPosition.x / _spacing - _worldPosition.y / (_spacing * sqrt3);
		float bFrac = 2f * _worldPosition.y / (_spacing * sqrt3);
		float cFrac = -aFrac - bFrac; // redundant third coord, a + b + c = 0

		int ra = Mathf.RoundToInt(aFrac);
		int rb = Mathf.RoundToInt(bFrac);
		int rc = Mathf.RoundToInt(cFrac);

		float da = Mathf.Abs(ra - aFrac);
		float db = Mathf.Abs(rb - bFrac);
		float dc = Mathf.Abs(rc - cFrac);

		if (da > db && da > dc)
			ra = -rb - rc;
		else if (db > dc)
			rb = -ra - rc;

		int residue = ((ra - rb) % 3 + 3) % 3;

		if (residue != 0)
			return new Vector2Int(ra, rb);

		// Landed on a center: pick the nearest of its 6 surrounding vertex points instead
		Vector2Int[] neighborOffsets =
		{
			new Vector2Int(1, 0), new Vector2Int(-1, 0),
			new Vector2Int(0, 1), new Vector2Int(0, -1),
			new Vector2Int(1, -1), new Vector2Int(-1, 1)
		};

		Vector2Int bestCoord = default;
		float bestSqrDist = float.MaxValue;

		foreach (Vector2Int offset in neighborOffsets)
		{
			int na = ra + offset.x;
			int nb = rb + offset.y;
			Vector2 candidatePos = na * u + nb * v;
			float sqrDist = (candidatePos - _worldPosition).sqrMagnitude;

			if (sqrDist < bestSqrDist)
			{
				bestSqrDist = sqrDist;
				bestCoord = new Vector2Int(na, nb);
			}
		}

		return bestCoord;
	}

	public static Vector2Int[] GetNeighbours(this Vector2Int _value, Vector2Int[] _hexCoords)
	{
		var returnCoords = new List<Vector2Int>();

		int ca = _value.x;
		int cb = _value.y;
		int cc = -ca - cb;

		foreach (var hexCoord in _hexCoords)
		{
			if (hexCoord == _value) continue;

			int na = hexCoord.x;
			int nb = hexCoord.y;
			int nc = -na - nb;

			int da = Mathf.Abs(na - ca);
			int db = Mathf.Abs(nb - cb);
			int dc = Mathf.Abs(nc - cc);

			// True lattice neighbors differ by exactly 1 step in cube space
			if (Mathf.Max(da, Mathf.Max(db, dc)) != 1) continue;

			returnCoords.Add(hexCoord);
		}

		return returnCoords.ToArray();
	}
}