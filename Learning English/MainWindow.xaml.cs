using Learning_English.Services;
using Learning_English.Classes;
using Learning_English.Words;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Learning_English
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string pathWords = $"{Environment.CurrentDirectory}\\WordsList.json";
        private readonly string pathStatistics = $"{Environment.CurrentDirectory}\\StatisticsList.json";
        private BindingList<Word> EnglishData;
        private List<int> UnitsData = new List<int>();
        private FileIOService fileIOService;

        public MainWindow()
        {
            InitializeComponent();
            fileIOService = new FileIOService(pathWords, pathStatistics);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                EnglishData = fileIOService.LoadDataWords();
                UnitsData = EnglishData.Select(x => x.Unit).Distinct().ToList(); 
                List<int> statisticData = fileIOService.LoadStatisticData();
                StatisticData.AllWordsCount = statisticData[0];
                StatisticData.CorrectWordsCount = statisticData[1];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Close();
            }

            UpdateComboBox(); // обновить элементы комбобокс (юниты) //
            dgEnglish.ItemsSource = EnglishData; // таблица словника берет информацию из биндинг листа "англ данные"
            EnglishData.ListChanged += English_Data_ListChanged;
            //dgEnglish.CanUserSortColumns = true;
        }

        private void English_Data_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemAdded || 
                e.ListChangedType == ListChangedType.ItemDeleted || 
                e.ListChangedType == ListChangedType.ItemChanged)
            {
                try
                {
                   fileIOService.SaveDataWords(sender); // сохранение таблицы слов //
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Close();
                }
            }

            UpdateUnitsData();
            UpdateComboBox();
        }

        private void Button_Start_Click(object sender, RoutedEventArgs e)
        {
            Testing testing = new Testing(EnglishData, UnitsData);
            testing.Show();
            this.Close();
        }

        private void Button_Statistic_Click(object sender, RoutedEventArgs e)
        {
            Statistic statistic = new Statistic();
            statistic.Show();
            this.Close();
        }

        private void UpdateDG()
        {
            int index = ComboBoxUnits.SelectedIndex;

            if (index == 0)
                dgEnglish.ItemsSource = EnglishData;
            else 
                dgEnglish.ItemsSource = from k in EnglishData where Convert.ToInt32(k.Unit) == index select k;
        }

        private void UpdateComboBox()
        {
            UnitsData.Sort();
            ComboBoxUnits.Items.Clear();
            ComboBoxUnits.Items.Add("All");
            int a = 0;
            foreach (int i in UnitsData)
            {
                ComboBoxUnits.Items.Add(UnitsData[a].ToString());
                a++;
            }
            ComboBoxUnits.SelectedIndex = 0;
        }

        private void UpdateUnitsData()
        {
            UnitsData.Clear();
            int a = 0;
            foreach (Word i in EnglishData)
            {
                int var = Convert.ToInt32(EnglishData[a].Unit);
                if (UnitsData.IndexOf(var) == -1 && var > 0)
                {
                    UnitsData.Add(var);
                }
                a++;
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                fileIOService.SaveDataWords(EnglishData); // сохранение таблицы слов //
            }
            catch {}
        }

        private void ComboBoxUnits_DropDownClosed(object sender, EventArgs e)
        {
            UpdateDG();
        }
    }
}
