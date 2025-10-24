namespace Techcore_Internship.Module_2;

public static class Task337_10_ValueTask
{
    public static async Task Run()
    {
        Console.WriteLine("\nTask337.10 - ValueTask\n");

        var cacheService = new CacheService();

        Console.WriteLine("1-я Попытка получить пользователя с ID = 1");
        var result1 = await cacheService.GetDataAsync(1);
        Console.WriteLine($"Результат: {result1}\n");

        Console.WriteLine("2-я Попытка получить пользователя с ID = 1");
        var result2 = await cacheService.GetDataAsync(1);
        Console.WriteLine($"Результат: {result2}\n");

        Console.WriteLine("1-я Попытка получить пользователя с ID = 2");
        var result3 = await cacheService.GetDataAsync(2);
        Console.WriteLine($"Результат: {result3} \n");
    }
}
public class CacheService
{
    private readonly Dictionary<int, string> _cache = new Dictionary<int, string>();

    public async ValueTask<string> GetDataAsync(int id)
    {
        if (_cache.TryGetValue(id, out var data))
        {
            Console.WriteLine($"Данные для ID {id} найдены в кэше");
            return data; 
        }

        Console.WriteLine($"Данные для ID {id} не найдены в кэше, загружаем...");
        await Task.Delay(3000); 

        data = $"Данные пользователя с ID = {id}";

        _cache[id] = data;
        Console.WriteLine($"Данные для ID {id} загружены и сохранены в кэш");

        return data;
    }
}