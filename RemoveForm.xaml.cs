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
    /// Interaction logic for RemoveForm.xaml
    /// </summary>
    public partial class RemoveForm : Window
    {
        private DataGrid _dataGrid;
        private readonly DatabaseService _dbService;

        public RemoveForm(DataGrid dataGrid, DatabaseService dbService)
        {
            InitializeComponent();

            _dataGrid = dataGrid;
            _dbService = dbService ?? throw new ArgumentNullException(nameof(dbService));

            // отримуємо актуальні id employees та заповнюємо ними idComboBox
            PopulateIdComboBox();
        }


        // отримання IDs employees для початкового idComboBox
        public void PopulateIdComboBox()
        {
            idComboBox.Items.Clear();
            foreach (var item in _dataGrid.Items)
            {
                if (item is DataRowView row)
                {
                    idComboBox.Items.Add(row[0].ToString());
                }
            }
        }


        private void removeRecordButton_Click(object sender, RoutedEventArgs e)
        {
            if (idComboBox.SelectedIndex == -1)
            {
                // якщо не обрано жодного id в idComboBox
                MessageBox.Show("You haven't selected a record", "No Record", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            if (idComboBox.SelectedItem != null)
            {
                string selectedId = idComboBox.SelectedItem.ToString();

                // видаляємо запис у БД і оновлюємо dataGrid
                MessageBoxResult areYouSure = MessageBox.Show($"Are you sure that you want to remove record with id {selectedId}?", "Confirming", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (areYouSure == MessageBoxResult.Yes)
                {
                    if (_dbService.DeleteRecordById(selectedId))
                    {
                        MessageBox.Show("Employee record removed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        _dbService.FetchData(_dataGrid);
                    }
                    else
                    {
                        MessageBox.Show("Error with removing");
                    }
                }
            }
            else
            {
                MessageBox.Show("Error with removing");
            }
        }

        private void idComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (idComboBox.SelectedItem != null)
            {
                string selectedId = idComboBox.SelectedItem.ToString();
                FillTextBoxes(selectedId);
            }
        }

        // заповнюємо текстбокси в залежності від обраного id в idComboBox
        private void FillTextBoxes(string id)
        {
            var data = GetRecordDataById(id);

            if (data != null)
            {
                surnameTextBox.Text = data["surname"].ToString();
                departmentTextBox.Text = data["department"].ToString();
                birthYearTextBox.Text = data["birth_year"].ToString();
                employmentYearTextBox.Text = data["employment_year"].ToString();
                positionTextBox.Text = data["position"].ToString();
                aDegreeTextBox.Text = data["academic_degree"].ToString();
                aTitleTextBox.Text = data["academic_title"].ToString();
            }
        }

        private DataRow? GetRecordDataById(string id)
        {
            return _dbService.GetRecordById(id);
        }
    }
}
