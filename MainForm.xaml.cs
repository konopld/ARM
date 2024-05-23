using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace ARM
{
    /// <summary>
    /// Interaction logic for MainForm.xaml
    /// </summary>
    public partial class MainForm : Window
    {
        private readonly DatabaseService _dbService;
        private readonly UserService _userService;

        // конструктор, що приймає DatabaseService та UserService як залежності
        public MainForm(DatabaseService dbService, UserService userService)
        {
            InitializeComponent();
            // перевірка на null для dbService та userService
            _dbService = dbService ?? throw new ArgumentNullException(nameof(dbService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));

            _dbService.FetchData(dataGrid);
        }

        // метод, який виконується при натисканні на кнопку завантаження
        private void downloadButton_Click(object sender, RoutedEventArgs e)
        {
            // створюємо діалог збереження файлу
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Microsoft Word Files (*.docx)|*.docx|All files (*.*)|*.*", // фільтр для вибору типу файлу
                DefaultExt = "docx", // стандартне розширення файлу
                FileName = "exportedData.docx" // стандартне ім'я файлу
            };

            // показуємо діалогове вікно та перевіряємо, чи натиснуто кнопку "ОК"
            if (saveFileDialog.ShowDialog() == true)
            {
                // викликаємо метод для експорту даних у MS Word файл
                GenerateReport(dataGrid, saveFileDialog.FileName);
            }
        }

        // метод для експорту даних з DataGrid у MS Word файл (UC2 Generate Report)
        private void GenerateReport(DataGrid dataGrid, string filePath)
        {
            // створюємо новий документ
            using var doc = DocX.Create(filePath);

            // додаємо заголовок до документа
            var title = doc.InsertParagraph("DataGrid Export").FontSize(20).Bold();
            title.Alignment = Alignment.center;

            // додаємо таблицю
            int columnCount = dataGrid.Columns.Count;
            int rowCount = dataGrid.Items.Count;

            var table = doc.AddTable(rowCount + 1, columnCount);
            table.Design = TableDesign.TableGrid;

            // отримуємо заголовки стовпців
            for (int colIndex = 0; colIndex < columnCount; colIndex++)
            {
                table.Rows[0].Cells[colIndex].Paragraphs[0].Append(dataGrid.Columns[colIndex].Header.ToString()).Bold();
            }

            // проходимо через всі рядки у DataGrid
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                var item = dataGrid.Items[rowIndex];
                if (item is DataRowView row)
                {
                    // якщо рядок є DataRowView, отримуємо значення полів
                    for (int colIndex = 0; colIndex < columnCount; colIndex++)
                    {
                        table.Rows[rowIndex + 1].Cells[colIndex].Paragraphs[0].Append(row.Row.ItemArray[colIndex].ToString());
                    }
                }
                else if (item != null)
                {
                    // для інших типів об'єктів отримуємо властивості та їх значення
                    var properties = item.GetType().GetProperties();
                    for (int colIndex = 0; colIndex < columnCount; colIndex++)
                    {
                        try
                        {
                            var propValue = properties[colIndex].GetValue(item)?.ToString();
                            table.Rows[rowIndex + 1].Cells[colIndex].Paragraphs[0].Append(propValue);
                        }
                        catch (System.IndexOutOfRangeException)
                        {
                            break;
                        }
                    }
                }
            }

            // додаємо таблицю до документа
            doc.InsertTable(table);

            // зберігаємо документ
            doc.Save();
        }
    }
}
