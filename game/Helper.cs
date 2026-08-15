
using System;
using System.Collections.Generic;

public static class Helper
{
    public static T PickRandom<T>(List<T> input)
    {
        var random = new Random();
        var randomElement = input[random.Next(input.Count)];
        return randomElement;
    }
}


