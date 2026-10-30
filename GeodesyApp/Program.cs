using ClosedXML.Excel;
using GeodesyApp;
using System;
using System.IO;
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
    Console.WriteLine("5. Выгрузить в Excel");
    Console.WriteLine("6. Выход");
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
        // Форматируем объём: если NULL — показываем прочерк
        string volumeText = s.Volume.HasValue ? s.Volume.Value.ToString() : "—";

        Console.WriteLine($"ID: {s.Id} | Название: {s.Name} | Оси: {s.Axes} | Отметка: {s.Mark} | Обьём: {volumeText} | Статус: {s.Status}");
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
    else if(choise == "2")
    {
        string name = ReadString("Введите название схемы: ");
        string axes = ReadString("Введите оси (например, 2-5/Д-Ж): ");
        string mark = ReadString("Введите отметку (например, -1.700 или -1.700, -2.800): ");

        // Объём — опциональный
        decimal? volume = null;
        Console.Write("Введите объём (или нажмите Enter для пропуска): ");
        string volumeInput = Console.ReadLine()!;

        if(!string.IsNullOrEmpty(volumeInput))
        {
            if(decimal.TryParse(volumeInput, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal vol))
            {
                volume = vol;
            }
            else
            {
                Console.WriteLine("Неверный формат числа. Объём не будет сохранён.");
            }
        }

        string status = ReadString("Введите статус схемы: ");

        Scheme newScheme = new Scheme();
        newScheme.Name = name;
        newScheme.Axes = axes;
        newScheme.Mark = mark;
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
    else if (choise == "5")
    {
        // 1. Получаем все схемы
        List<Scheme> schemes = dbHelper.GetAllSchemes();

        // 2. Формируем путь
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string filePath = Path.Combine(desktopPath, "РеестрСхем.xlsx");

        // 3. Создаём workbook
        ExcelExporter exporter = new ExcelExporter();
        XLWorkbook workbook = exporter.CreateExcelWorkbook(schemes);

        // 4. Сохраняем с проверками
        exporter.SaveWorkbook(workbook, filePath);
    }
    else if(choise == "6")
    {
        break;
    }
    else
    {
        Console.WriteLine("Неверный выбор");
        
    }
    
}



Console.ReadKey();