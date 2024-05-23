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
    /// Interaction logic for EditForm.xaml
    /// </summary>
    public partial class EditForm : Window
    {
        private readonly List<TextBox> textBoxes;
        private DataGrid _dataGrid;
        private readonly DatabaseService _dbService;

        public EditForm(DataGrid dataGrid, DatabaseService dbService)
        {
            InitializeComponent();

            _dataGrid = dataGrid;
            _dbService = dbService ?? throw new ArgumentNullException(nameof(dbService));
            textBoxes = FindAllTextBoxes(wrapPanel);

            // початкова деактивація текстбоксів (перед вибором id запису)
            ChangeTextBoxesStatus(false);

            // отримуємо актуальні id employees та заповнюємо ними idComboBox
            PopulateIdComboBox();
        }

        // допоміжний метод для знайдення всіх текстбоксів на формі
        public static List<TextBox> FindAllTextBoxes(DependencyObject parent)
        {
            List<TextBox> textBoxes = [];

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                if (child is TextBox textBox)
                {
                    textBoxes.Add(textBox);
                }
                else
                {
                    textBoxes.AddRange(FindAllTextBoxes(child));
                }
            }

            return textBoxes;
        }

        // допоміжний метод для активації/деактивації текстбоксів
        public void ChangeTextBoxesStatus(bool isEnabled)
        {
            foreach (TextBox textBox in textBoxes)
            {
                textBox.IsEnabled = isEnabled;
            }
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


        private void saveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            if (idComboBox.SelectedIndex == -1)
            {
                // якщо не обрано жодного id в idComboBox
                MessageBox.Show("You haven't selected a record", "No Record", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            // оновлена інформація про запис
            var updatedData = new Dictionary<string, string>
            {
                { "Surname", surnameTextBox.Text },
                { "Department", departmentTextBox.Text },
                { "BirthYear", birthYearTextBox.Text },
                { "EmploymentYear", employmentYearTextBox.Text },
                { "Position", positionTextBox.Text },
                { "AcademicDegree", aDegreeTextBox.Text },
                { "AcademicTitle", aTitleTextBox.Text }
            };

            if (idComboBox.SelectedItem != null)
            {
                string selectedId = idComboBox.SelectedItem.ToString();

                // оновлюємо змінні у базі данних і оновлюємо dataGrid
                _dbService.UpdateRecordById(selectedId, updatedData);
                _dbService.FetchData(_dataGrid);

                MessageBox.Show("Employee record changed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Error with saving changes");
            }
        }

        private void idComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (idComboBox.SelectedItem != null)
            {
                string selectedId = idComboBox.SelectedItem.ToString();
                FillTextBoxes(selectedId);
                ChangeTextBoxesStatus(true);
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
