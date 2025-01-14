using UnityEngine;

public static class ChancesHelper
{
    private const float ITERATIONS_GIVEOUT_MULT = 2;
    private const float BASE_GIVEOUT_CHANCE = 95;
    private const float CAT_GIVEOUT_CHANCE_SUBSTRACT = 8;
    private const float MIN_GIVEOUT_CHANCE = 20;
    private const float D_BASE_CHANCE = 20;
    private const float C_BASE_CHANCE = -0.5f;
    private const float B_BASE_CHANCE = -0.15f;
    private const float ITERATIONS_D_MULT = 3;
    private const float ITERATIONS_C_MULT = 0.8f;
    private const float ITERATIONS_B_MULT = 0.05f;

    public static float CalculateGiveoutChance(int currentIteration, int catCount)
    {
        return Mathf.Max(MIN_GIVEOUT_CHANCE, BASE_GIVEOUT_CHANCE - CAT_GIVEOUT_CHANCE_SUBSTRACT * catCount) + currentIteration * ITERATIONS_GIVEOUT_MULT;
    }

    public static float CalculateDChance(int currentIteration)
    {
        return Mathf.Max(0, D_BASE_CHANCE + currentIteration * ITERATIONS_D_MULT);
    }

    public static float CalculateCChance(int currentIteration)
    {
        return Mathf.Max(0, C_BASE_CHANCE + currentIteration * ITERATIONS_C_MULT);
    }

    public static float CalculateBChance(int currentIteration)
    {
        return Mathf.Max(0, B_BASE_CHANCE + currentIteration * ITERATIONS_B_MULT);
    }

    public static float CalculateEChance(int currentIteration)
    {
        return 100 - (CalculateDChance(currentIteration) - CalculateCChance(currentIteration) - CalculateBChance(currentIteration)) - (CalculateCChance(currentIteration) - CalculateBChance(currentIteration)) - CalculateBChance(currentIteration);
    }
}
