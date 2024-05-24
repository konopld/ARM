using System;
using System.Collections.Generic;
using System.Data;
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

namespace ARM
{
    /// <summary>
    /// Interaction logic for AddForm.xaml
    /// </summary>
    public partial class AddForm : Window
    {
        private DataGrid _dataGrid;
        private readonly DatabaseService _dbService;

        public AddForm(DataGrid dataGrid, DatabaseService dbService)
        {
            InitializeComponent();

            _dataGrid = dataGrid;
            _dbService = dbService ?? throw new ArgumentNullException(nameof(dbService));
        }

        private void addNewRecordButton_Click(object sender, RoutedEventArgs e)
        {
            // інформація нового запису
            var newData = new Dictionary<string, string>
            {
                { "Surname", surnameTextBox.Text },
                { "Department", departmentTextBox.Text },
                { "BirthYear", birthYearTextBox.Text },
                { "EmploymentYear", employmentYearTextBox.Text },
                { "Position", positionTextBox.Text },
                { "AcademicDegree", aDegreeTextBox.Text },
                { "AcademicTitle", aTitleTextBox.Text }
            };


            // додаємо новий запис до БД і оновлюємо dataGrid
            try
            {
                if (_dbService.AddRecord(newData))
                {
                    MessageBox.Show("Employee record added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                
                _dbService.FetchData(_dataGrid);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error with adding new record");
            }
        }
    }
}
