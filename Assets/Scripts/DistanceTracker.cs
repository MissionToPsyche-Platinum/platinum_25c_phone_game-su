using UnityEngine;

/// <summary>
/// Computes the "real life" distance travelled for the current run, shown on the
/// Mission Report.
///
/// Each checkpoint/stage is anchored to a real cumulative distance along NASA's
/// Psyche route (Earth -> Moon -> Mars gravity assist -> asteroid 16 Psyche ->
/// beyond). The run's score - which is what gates checkpoints - is mapped onto
/// those milestones with piecewise-linear interpolation, so reaching a checkpoint
/// reads as exactly that real leg of the trip.
/// </summary>
public class DistanceTracker : MonoBehaviour
{
    // Cumulative real-world distance (km) logged when each checkpoint is reached,
    // index-aligned with CheckpointSpawnScript.checkpointScores:
    //   0 Earth, 1 Moon, 2 Mars, 3 Psyche, 4 Jupiter, 5 Saturn, 6 Uranus,
    //   7 Neptune, 8 Proxima Centauri, 9 Alpha Centauri A, 10 Alpha Centauri B.
    // Inner legs follow the real Psyche mission (~3.6 billion km to the asteroid,
    // via a Mars gravity assist); outer/interstellar legs are extrapolated.
    // Tune these in the inspector.
    [SerializeField]
    private double[] stageDistancesKm =
    {
        0.0,                  // Earth - launch
        384400.0,             // Moon
        1_500_000_000.0,      // Mars gravity assist
        3_600_000_000.0,      // asteroid 16 Psyche - mission target
        7_200_000_000.0,      // Jupiter
        13_000_000_000.0,     // Saturn
        27_000_000_000.0,     // Uranus
        43_000_000_000.0,     // Neptune
        40_000_000_000_000.0, // Proxima Centauri (~4.24 light-years)
        41_300_000_000_000.0, // Alpha Centauri A
        41_340_000_000_000.0  // Alpha Centauri B
    };

    // Used only if checkpoint score data can't be found: flat km per score point.
    [SerializeField] private double fallbackKmPerScorePoint = 120000.0;

    private ScoreIncrement scoreIncrement;
    private CheckpointSpawnScript checkpointSpawner;

    /// <summary>Distance travelled so far this run, in kilometers.</summary>
    public double DistanceKm => ComputeDistanceKm();

    private void Start()
    {
        scoreIncrement = FindFirstObjectByType<ScoreIncrement>();
        checkpointSpawner = FindFirstObjectByType<CheckpointSpawnScript>();
    }

    private double ComputeDistanceKm()
    {
        if (scoreIncrement == null)
            scoreIncrement = FindFirstObjectByType<ScoreIncrement>();
        if (checkpointSpawner == null)
            checkpointSpawner = FindFirstObjectByType<CheckpointSpawnScript>();

        float score = scoreIncrement != null ? scoreIncrement.GetCurrentScore() : 0f;
        return JourneyDistanceForScore(score);
    }

    // Maps a score onto the journey milestones with piecewise-linear interpolation.
    private double JourneyDistanceForScore(float score)
    {
        int[] scores = checkpointSpawner != null ? checkpointSpawner.checkpointScores : null;

        if (scores == null || scores.Length < 2 || stageDistancesKm.Length < 2)
        {
            // No usable checkpoint data - fall back to a flat rate.
            return score * fallbackKmPerScorePoint;
        }

        int n = Mathf.Min(scores.Length, stageDistancesKm.Length);

        if (score <= scores[0])
            return stageDistancesKm[0];

        for (int i = 0; i < n - 1; i++)
        {
            if (score < scores[i + 1])
            {
                double span = scores[i + 1] - scores[i];
                double t = span > 0.0 ? (score - scores[i]) / span : 0.0;
                return Lerp(stageDistancesKm[i], stageDistancesKm[i + 1], t);
            }
        }

        // Past the final checkpoint (endless run): keep going at the final leg's rate.
        double lastSpan = scores[n - 1] - scores[n - 2];
        double lastRate = lastSpan > 0.0
            ? (stageDistancesKm[n - 1] - stageDistancesKm[n - 2]) / lastSpan
            : 0.0;
        return stageDistancesKm[n - 1] + (score - scores[n - 1]) * lastRate;
    }

    private static double Lerp(double a, double b, double t)
    {
        return a + (b - a) * t;
    }
}
