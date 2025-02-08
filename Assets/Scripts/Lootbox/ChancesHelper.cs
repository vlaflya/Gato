using System;
using UnityEngine;

public static class ChancesHelper
{
    private const float ITERATIONS_GIVEOUT_MULT = 3;
    private const float BASE_GIVEOUT_CHANCE = 75;
    private const float CAT_GIVEOUT_CHANCE_SUBSTRACT = 15;
    private const float MIN_GIVEOUT_CHANCE = 10;
    private const float D_BASE_CHANCE = 8.571428571f;
    private const float C_BASE_CHANCE = -17.64705882f;
    private const float B_BASE_CHANCE = -2.8f;
    private const float A_BASE_CHANCE = -3.2f;
    private const float S_BASE_CHANCE = -2.38f;
    private const float D_ITERATIONS_MULT = 3.80952381f;
    private const float C_ITERATIONS_MULT = 3.235294118f;
    private const float B_ITERATIONS_MULT = 0.5333333333f;
    private const float A_ITERATIONS_MULT = 0.2166666667f;
    private const float S_ITERATIONS_MULT = 0.12f;
    private const float D_MIN = 20;
    private const float C_MIN = 5;
    private const float B_MIN = 2;
    private const float A_MIN = 0.05f;
    private const float S_MIN = 0.02f;
    private const int D_ITERATION_START = 3;
    private const int C_ITERATION_START = 7;
    private const int B_ITERATION_START = 9;
    private const int A_ITERATION_START = 15;
    private const int S_ITERATION_START = 20;


    public static float CalculateGiveoutChance(int currentIteration, int catCount)
    {
        return Mathf.Max(MIN_GIVEOUT_CHANCE, Mathf.Min(100, BASE_GIVEOUT_CHANCE - CAT_GIVEOUT_CHANCE_SUBSTRACT * catCount)) + currentIteration * ITERATIONS_GIVEOUT_MULT;
    }

    public static float CalculateDChance(int currentIteration)
    {
        if (currentIteration < D_ITERATION_START)
            return 0;
        return Mathf.Max(D_MIN, D_BASE_CHANCE + currentIteration * D_ITERATIONS_MULT);
    }

    public static float CalculateCChance(int currentIteration)
    {
        if (currentIteration < C_ITERATION_START)
            return 0;
        return Mathf.Max(C_MIN, C_BASE_CHANCE + currentIteration * C_ITERATIONS_MULT);
    }

    public static float CalculateBChance(int currentIteration)
    {
        if (currentIteration < B_ITERATION_START)
            return 0;
        return Mathf.Max(B_MIN, B_BASE_CHANCE + currentIteration * B_ITERATIONS_MULT);
    }

    public static float CalculateAChance(int currentIteration)
    {
        if (currentIteration < A_ITERATION_START)
            return 0;
        return Mathf.Max(A_MIN, A_BASE_CHANCE + currentIteration * A_ITERATIONS_MULT);
    }

    public static float CalculateSChance(int currentIteration)
    {
        if (currentIteration < S_ITERATION_START)
            return 0;
        return Mathf.Max(S_MIN, S_BASE_CHANCE + currentIteration * S_ITERATIONS_MULT);
    }
}
