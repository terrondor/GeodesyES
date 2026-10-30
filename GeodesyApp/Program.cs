using GeodesyApp;
using System;
using System.Globalization;
using System.Net.NetworkInformation;


string connectionString = "Server=.\\SQLEXPRESS;Database=GeodesyRegistry;Trusted_Connection=True;TrustServerCertificate=true;";
DatabaseHelper dbHelper = new DatabaseHelper(connectionString);

static void PrintMenu()
{
    Console.WriteLine("\n=== РЕЕСТР ИСПОЛНИТЕЛЬНЫХ СХЕМ ===\n");
    Console.WriteLine("1. Показать все схемы");
    Console.WriteLine("2. Добавить новую схему");
    Console.WriteLine("3. Изменить статус схемы");
    Console.WriteLine("4. Удалить схему");
    Console.WriteLine("5. Выход");
    Console.Write("Выберите действие: ");
}

static string ReadString(string prompt)
{
    Console.Write(prompt);
    return Console.ReadLine()!;
}



static void PrintScheme(List<Scheme> schemes)
{

    foreach(Scheme s in schemes)
    {
        Console.WriteLine($"ID: {s.Id} | Название: {s.Name} | Вид: {s.View} | Объем: {s.Volume} | Статус: {s.Status}");
    }
}

static int ReadInt(string prompt)
{
    int result;
    while(true)
    {
        Console.Write(prompt);
        string input = Console.ReadLine()!;

        if(int.TryParse(input, out result))
        {
            return result;  // возвращаем число и выходим из метода
        }

        Console.WriteLine("Неверный формат. Введите целое число.");
    }
}

static decimal ReadDecimal(string prompt)
{
    decimal result;
    while(true)
    {
        Console.Write(prompt);
        string input = Console.ReadLine()!;

        if(decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
        {
            return result;
        }

        Console.WriteLine("Неверный формат числа. Попробуйте ещё раз.");
    }
}

while(true)
{
    PrintMenu();

    string choise = Console.ReadLine()!;

    if ( choise == "1")
    {
        List<Scheme> schemes = dbHelper.GetAllSchemes();
        PrintScheme(schemes);
    }
    else if (choise == "2")
    {
        string name = ReadString("Введите название схемы: ");
        string view = ReadString("Введите Вид схемы: ");
        
        decimal volume = ReadDecimal("Введите объём схемы: ");
        string status = ReadString("Введите cтатус схемы: ");

        Scheme newScheme = new Scheme();
        newScheme.Name = name;
        newScheme.View = view;
        newScheme.Volume = volume;
        newScheme.Status = status;

        bool success = dbHelper.AddScheme(newScheme);

        if(success)
        {
            Console.WriteLine("Схема успешно добавлена!");
        }
        else
        {
            Console.WriteLine("Ошибка при добавлении схемы.");
        }
    }
    else if (choise == "3") 
    {
        // 1. Показываем все схемы (чтобы пользователь видел ID)
        Console.WriteLine("\n=== ВЫБЕРИТЕ СХЕМУ ДЛЯ ИЗМЕНЕНИЯ СТАТУСА ===");
        List<Scheme> schemes = dbHelper.GetAllSchemes();
        PrintScheme(schemes);

        // 2. Спрашиваем ID схемы
        int id = ReadInt("Введите ID схемы: ");

        // 3. Спрашиваем новый статус
        string newStatus = ReadString("Введите новый cтатус схемы: ");

        // 4. Вызываем метод обновления
        bool success = dbHelper.UpdateStatus(id, newStatus);

        // 5. Показываем результат
        if(success)
        {
            Console.WriteLine("Статус успешно изменён!");
        }
        else
        {
            Console.WriteLine("Ошибка при изменении статуса.");
        }
    }
    else if(choise == "4")
    {
        // 1. Показываем все схемы (чтобы пользователь видел ID)
        Console.WriteLine("\n=== ВЫБЕРИТЕ СХЕМУ ДЛЯ УДАЛЕНИЯ ===");
        List<Scheme> schemes = dbHelper.GetAllSchemes();
        PrintScheme(schemes);
        // 2. Спрашиваем ID схемы
        int id = ReadInt("Введите ID схемы: ");

        string confirm = ReadString("Вы уверены ? (да / нет): ");
        
        if (confirm.ToLower() != "да") { Console.WriteLine("Отмена"); continue; }

        bool success = dbHelper.DeleteScheme(id);
        if(success)
        {
            Console.WriteLine("Схема успешно удалена!");
        }
        else
        {
            Console.WriteLine("Ошибка при удалении схемы.");
        }
    }
    else if(choise == "5")
    {
        break;
    }
    else
    {
        Console.WriteLine("Неверный выбор");
        
    }
    
}



Console.ReadKey();