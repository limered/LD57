namespace depths_ld57.Score;

public static class ScoreStore
{
    public static int DirtParticlesLeft { private get; set; }
    public static int DirtParticlesMax { private get; set; }
    public static float PercentLeft => (float)DirtParticlesLeft / DirtParticlesMax;
}