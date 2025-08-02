namespace Jcg.Application.Optics.Tests.TestingCommon;

public static class RandomHelper
{
    public static int GenInt(int min = 1, int max = 100)
    {
        return new Random().Next(min, max);
    }
    
  public static decimal GenDecimal(decimal min = 1, decimal max = 100)
    {
        var random = new Random();
        var next = (decimal)random.NextDouble();
        return min + (next * (max - min));
    }
  
    public static string GenString(int minLength = 10, int maxLength = 20)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        int length = random.Next(minLength, maxLength + 1);
        var result = new char[length];
        for (int i = 0; i < length; i++)
        {
            result[i] = chars[random.Next(chars.Length)];
        }
        return new string(result);
    }
}