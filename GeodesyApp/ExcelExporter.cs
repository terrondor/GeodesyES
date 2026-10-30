using System;
using System.Collections.Generic;
using System.Text;
using ClosedXML;
using ClosedXML.Excel;

namespace GeodesyApp
{
    public class ExcelExporter
    {
        public void ExportToExcel(List<Scheme> schemes, string filePath)
        {
            //Создаём workbook(всю книгу)
            using(var workbook = new XLWorkbook())
            {
                //Добавляем лист
                var worksheet = workbook.Worksheets.Add("Реестр схем");

                //Заголовки (первая строка)
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Название";
                worksheet.Cell(1, 3).Value = "Вид";
                worksheet.Cell(1, 4).Value = "Обьем";
                worksheet.Cell(1, 5).Value = "Статус";

                //Данные (начиная со 2-й строки)
                int row = 2;
                foreach(var scheme in schemes)
                {
                    worksheet.Cell(row, 1).Value = scheme.Id;
                    worksheet.Cell(row, 2).Value = scheme.Name;
                    worksheet.Cell(row, 3).Value = scheme.View;
                    worksheet.Cell(row, 4).Value = scheme.Volume;
                    worksheet.Cell(row, 5).Value = scheme.Status;

                    //Красим только ячейку статуса
                    string status = scheme.Status;
                    XLColor color;

                    if(status == "Подписана")
                        color = XLColor.LightGreen;
                    else if(status == "На подписании")
                        color = XLColor.LightYellow;
                    else if(status == "Готова к подписанию")
                        color = XLColor.LightBlue;
                    else
                        color = XLColor.White;

                    worksheet.Cell(row, 5).Style.Fill.BackgroundColor = color;

                    row++;
                }

                // Форматируем заголовки (жирные, серый фон, по центру)
                for(int col = 1; col <= 5; col++)
                {
                    var headerCell = worksheet.Cell(1, col);
                    headerCell.Style.Font.Bold = true;
                    headerCell.Style.Fill.BackgroundColor = XLColor.LightGray;
                    headerCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                // Границы для всего диапазона (заголовки + данные)
                var dataRange = worksheet.Range(1, 1, row - 1, 5);
                dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                // Автоширина колонок
                worksheet.Columns(1, 5).AdjustToContents();

                // Сохраняем файл
                workbook.SaveAs(filePath);
            }

        }
        private void ApplyColorCoding(IXLWorksheet worksheet, int totalRows)
        {
            // Проходим по всем строкам данных (начиная со 2-й)
            for (int row = 2; row <= totalRows; row++)
            {
                // Получаем статус из 5-й колонки
                string status = worksheet.Cell(row, 5).Value.ToString();

                // Определяем цвет в зависимости от статуса
                XLColor color;

                if (status == "Подписана")
                {
                    color = XLColor.LightGreen;
                }
                else if (status == "На подписании")
                {
                    color = XLColor.LightYellow;
                }
                else if(status == "Готова к подписанию")
                {
                    color = XLColor.LightBlue;
                }
                else
                {
                    color = XLColor.White;
                }
                // Применяем цвет ко всем 5 ячейкам строки
                for (int col = 1; col <= 5; col++)
                {
                    worksheet.Cell(row, col).Style.Fill.BackgroundColor = color;
                }
            }
        }
    }
}
