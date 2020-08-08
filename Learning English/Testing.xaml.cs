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
using System.Windows.Shapes;

namespace Learning_English
{
    /// <summary>
    /// Логика взаимодействия для Testing.xaml
    /// </summary>
    public partial class Testing : Window
    {
        
        private bool testingStart = false; // начало тестирования
        private bool testingFinal = false; // в тесте пройдены все слова
        private bool exit = false; // выход из окна
        private bool wordsByChance = false; // слова вразброс
        private bool getAnswer = false; // дал ли пользователь ответ на вопрос?

        private int time = 0; // ограничение времени (по умолчанию 0, т.е. выключено)
        private int allWordsCount;
        private int nowWordNumber = 1;
        private int unit = 0; // 0 = All
        private int byChanceVariable = 0;
   
        private BindingList<Word> EnglishData; // слова

        private List<Word> EnglishDataFiltered = new List<Word>();
        private List<int> UnitsData = new List<int>(); // список разделов (Units)
        private List<int> IndexOfWords = new List<int>(); // использованные индексы слов
        


        public Testing(BindingList<Word> EnglishData, List<int> UnitsData)
        {
            InitializeComponent();
            allWordsCount = EnglishData.Count;
            this.EnglishData = EnglishData;
            this.UnitsData = UnitsData;
            UpdateComboBox();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlockAllWordsCount.Text = nowWordNumber.ToString() + "/" + allWordsCount.ToString();
            ProgressBar.Minimum = 0;
            ProgressBar.Maximum = allWordsCount;
            ProgressBar.Value = 1;
            ProgressBar.Visibility = Visibility.Hidden;
            TextBlockAllWordsCount.Visibility = Visibility.Hidden;
            ButtonGetAnswer.Visibility = Visibility.Hidden;
            ButtonNextQuestion.Visibility = Visibility.Hidden;
            ButtonNextQuestion.Margin = new Thickness(154, 160, 0, 0);
            this.Height = 245;
            TextBoxAnswer.IsReadOnly = true;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (exit == false)
            {
                System.Windows.MessageBoxResult result = MessageBox.Show("Вы действительно хотите выйти?\nРезультаты тестирования не сохранятся!", "ВНИМАНИЕ", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            time = Convert.ToInt32(SliderTimerMinutes.Value);
            TextBoxTimerMinutes.Text = time.ToString() + " minutes";
        }



        private void CheckBoxTimerMinutes_Click(object sender, RoutedEventArgs e)
        {
            if (CheckBoxTimerMinutes.IsChecked == true)
            {
                SliderTimerMinutes.IsEnabled = true;
                TextBoxTimerMinutes.IsEnabled = true;
            }
            else
            {
                SliderTimerMinutes.IsEnabled = false;
                TextBoxTimerMinutes.IsEnabled = false;
                time = 0;
                SliderTimerMinutes.Value = 0;
                TextBoxTimerMinutes.Text = time.ToString() + " minutes";
            }
        }

       

        private void ButtonStartEndTesting_Click(object sender, RoutedEventArgs e)
        {
            if (testingStart == false)
            {
                testingStart = true;
                TextBoxAnswer.IsReadOnly = false;

                this.Height = 295;

                ButtonStartEndTesting.Visibility = Visibility.Hidden;
                ButtonStartEndTesting.Content = "Завершить тест";
                ProgressBar.Visibility = Visibility.Visible;
                TextBlockAllWordsCount.Visibility = Visibility.Visible;
                ButtonNextQuestion.Visibility = Visibility.Visible;
                ButtonGetAnswer.Visibility = Visibility.Visible;
                unit = ComboBoxUnitsTesting.SelectedIndex;

                ComboBoxUnitsTesting.IsEnabled = false;
                CheckBoxWordsByChance.IsEnabled = false;
                CheckBoxTimerMinutes.IsEnabled = false;
                SliderTimerMinutes.IsEnabled = false;
                TextBoxTimerMinutes.IsEnabled = false;
                ButtonNextQuestion.IsEnabled = false;
                TextBoxAnswer.Text = "";

                if (unit > 0)
                {
                    int a = 0;
                    allWordsCount = 0;
                    foreach (Word i in EnglishData)
                    {
                        if (Convert.ToInt32(EnglishData[a].unit) == unit)
                        {
                            allWordsCount++;
                        }
                        a++;
                    }
                }

                TextBlockAllWordsCount.Text = nowWordNumber.ToString() + "/" + allWordsCount.ToString();
                ProgressBar.Maximum = allWordsCount;

                if (unit != 0)
                {
                    foreach (Word n in EnglishData)
                    {
                        EnglishDataFiltered = (from k in EnglishData where (Convert.ToInt32(k.unit) == unit) select k).ToList();
                    }

                    TextBlockQuestion.Text = EnglishDataFiltered[byChanceVariable].translateWord.ToString();
                }
                else
                    TextBlockQuestion.Text = EnglishData[byChanceVariable].translateWord.ToString();




            }

            if (testingFinal == true)
            {
                System.Windows.MessageBoxResult result = MessageBox.Show("Вы действительно хотите завершить тест?", "ВНИМАНИЕ", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    exit = true;
                    this.Close();
                }
            }
        }

       

        private void UpdateComboBox()
        {
            UnitsData.Sort();
            ComboBoxUnitsTesting.Items.Clear();
            ComboBoxUnitsTesting.Items.Add("All");
            int a = 0;
            foreach (int i in UnitsData)
            {
                ComboBoxUnitsTesting.Items.Add(UnitsData[a].ToString());
                a++;
            }
            ComboBoxUnitsTesting.SelectedIndex = 0;
        }

        private void CheckBoxWordsByChance_Click(object sender, RoutedEventArgs e)
        {
            if (CheckBoxWordsByChance.IsChecked == true)
                wordsByChance = true;
            else
                wordsByChance = false;
        }

        private void ButtonNextQuestion_Click(object sender, RoutedEventArgs e)
        {
            TextBoxAnswer.IsReadOnly = false;
            nowWordNumber++;
            ProgressBar.Value = nowWordNumber;
            TextBlockAllWordsCount.Text = nowWordNumber.ToString() + "/" + allWordsCount.ToString();
            ButtonNextQuestion.IsEnabled = false;
            ButtonGetAnswer.IsEnabled = true;
            byChanceVariable++;
            if (unit != 0)
            {
                TextBlockQuestion.Text = EnglishDataFiltered[byChanceVariable].translateWord.ToString();
            }
            else
                TextBlockQuestion.Text = EnglishData[byChanceVariable].translateWord.ToString();

            if (nowWordNumber == allWordsCount)
            {
                ButtonNextQuestion.Visibility = Visibility.Hidden;
                ButtonGetAnswer.Margin = new Thickness(5, 160, 0, 0);
                ButtonGetAnswer.Width = 287;
            }
        }

        private void ButtonGetAnswer_Click(object sender, RoutedEventArgs e)
        {
            if (ButtonNextQuestion.IsEnabled == false && ProgressBar.Value != ProgressBar.Maximum)
            {
                ButtonNextQuestion.IsEnabled = true;
                ButtonGetAnswer.IsEnabled = false;
            }
            TextBoxAnswer.IsReadOnly = true;

            if (nowWordNumber == allWordsCount)
            {
                testingFinal = true;
                ButtonStartEndTesting.Visibility = Visibility.Visible;
                ProgressBar.Visibility = Visibility.Hidden;
                ButtonGetAnswer.IsEnabled = false;
                ButtonGetAnswer.Visibility = Visibility.Hidden;
                TextBlockAllWordsCount.Visibility = Visibility.Hidden;
                this.Height = 245;
            }
        }

        
    }
}
