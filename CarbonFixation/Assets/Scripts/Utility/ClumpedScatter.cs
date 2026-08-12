using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates a set of points on a normalized [0,1] x [0,1] plane that are randomly
/// scattered but naturally clump together, using Perlin noise as a probability field.
/// Useful for decoration placement (grass tufts, rocks, props) where pure uniform
/// random looks too even and pure clustering looks too artificial.
/// </summary>
public static class ClumpedScatter
{
    /// <summary>
    /// Generates clumped-random points on a normalized 0-1 plane.
    /// </summary>
    /// <param name="count">How many points to generate.</param>
    /// <param name="noiseScale">
    /// Frequency of the underlying Perlin noise. Lower = bigger, fewer clumps.
    /// Higher = smaller, more numerous clumps. Try 2-6 as a starting range.
    /// </param>
    /// <param name="clumpiness">
    /// Exponent applied to the noise value before thresholding. 1 = mild clumping.
    /// Higher values (3-6) push points into tighter, sparser clumps with more empty space.
    /// </param>
    /// <param name="minDistance">
    /// Optional minimum spacing between accepted points (in normalized 0-1 units).
    /// Prevents points from stacking directly on top of each other inside a clump.
    /// Set to 0 to disable.
    /// </param>
    /// <param name="random">Randomizer, allows using same randomizer to generate multiple results</param>
    /// <param name="maxAttemptsPerPoint">
    /// Safety cap on rejection-sampling attempts per point, to avoid an infinite loop
    /// if count is too high for the given clumpiness/minDistance settings.
    /// </param>
    public static List<Vector2> Generate(
        int count,
        System.Random random,
        float noiseScale = 3f,
        float clumpiness = 2f,
        float minDistance = 0f,
        int maxAttemptsPerPoint = 50)
    {

        // Random offset so different seeds actually sample different regions
        // of Perlin space instead of all starting at the same origin pattern.
        float offsetX = (float)random.NextDouble() * 10000f;
        float offsetY = (float)random.NextDouble() * 10000f;

        var points = new List<Vector2>(count);
        int maxTotalAttempts = count * maxAttemptsPerPoint;
        int attempts = 0;

        while (points.Count < count && attempts < maxTotalAttempts)
        {
            attempts++;

            float x = (float)random.NextDouble();
            float y = (float)random.NextDouble();

            float density = Mathf.PerlinNoise(x * noiseScale + offsetX, y * noiseScale + offsetY);
            density = Mathf.Pow(density, clumpiness);

            // Roll against the density field - high density = likely accept,
            // low density = likely reject. This is what creates the clumping.
            if ((float)random.NextDouble() > density)
                continue;

            if (minDistance > 0f && IsTooClose(points, x, y, minDistance))
                continue;

            points.Add(new Vector2(x, y));
        }

        return points;
    }

    private static bool IsTooClose(List<Vector2> points, float x, float y, float minDistance)
    {
        float minDistSqr = minDistance * minDistance;
        var candidate = new Vector2(x, y);

        for (int i = 0; i < points.Count; i++)
        {
            if ((points[i] - candidate).sqrMagnitude < minDistSqr)
                return true;
        }
        return false;
    }
}