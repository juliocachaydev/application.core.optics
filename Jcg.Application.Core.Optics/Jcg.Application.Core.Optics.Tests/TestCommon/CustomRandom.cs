namespace Jcg.Application.Core.Optics.Tests.TestCommon;

public static class CustomRandom
{
    public static int RandomInt(int min = 0, int max = 100)
    {
        var rnd = new Random();
        return rnd.Next(min, max + 1);
    }

    public static decimal RandomDecimal(decimal min = 0m, decimal max = 100m)
    {
        var rnd = new Random();
        var next = (decimal)rnd.NextDouble();
        return min + next * (max - min);
    }

    public static string RandomString(int minLenght = 10, int maxLength = 50)
    {
        var rnd = new Random();
        var length = rnd.Next(minLenght, maxLength + 1);
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[rnd.Next(s.Length)]).ToArray());
    }
}