using Learning_English.Classes;
using Learning_English.Services;
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
        private bool wordsByChance = false; // слова вразброс ДА или НЕТ (по умолчанию нет)

        private int time = 0; // ограничение времени (по умолчанию 0, т.е. выключено)
        private int allWordsCount; // количество всех вопросов (всех слов)
        private int nowWordNumber = 1; // текущий вопрос (текущее слово)
        private int unit = 0; // 0 = All текущий юнит
        private int wordIndex = 0; // индекс слова в коллекции
        private int correctAnswerCount = 0; // количество правильных ответов = 0

        private BindingList<Word> EnglishData; // коллекция словаря

        private List<Word> EnglishDataFiltered = new List<Word>(); // коллекция отфильтрованного словаря

        private List<int> UnitsData = new List<int>(); // список разделов (Units)
        private List<int> IndexOfWords = new List<int>(); // коллекция использованных индексов слов (если включены слова вразброс)

        private FileIOService fileIOService;

        private readonly string pathStatistic = $"{Environment.CurrentDirectory}\\StatisticList.json"; // путь файла "статистика"

        Random r = new Random();

        public Testing(BindingList<Word> EnglishData, List<int> UnitsData)
        {
            InitializeComponent();
            allWordsCount = EnglishData.Count; // количество всех слов
            this.EnglishData = EnglishData; // коллекция слов
            this.UnitsData = UnitsData; // коллекция юнитов
            UpdateComboBox(); 
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Height = 245;

            ProgressBar.Minimum = 0;
            ProgressBar.Maximum = allWordsCount;
            ProgressBar.Value = 1;
            ProgressBar.Visibility = Visibility.Hidden;

            TextBlockAllWordsCount.Text = nowWordNumber.ToString() + "/" + allWordsCount.ToString();
            TextBlockAllWordsCount.Visibility = Visibility.Hidden;

            TextBoxAnswer.IsReadOnly = true;

            ButtonGetAnswer.Visibility = Visibility.Hidden;

            ButtonNextQuestion.Visibility = Visibility.Hidden;
            ButtonNextQuestion.Margin = new Thickness(154, 160, 0, 0);
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
                StatisticData.AllWordsCount += allWordsCount;
                StatisticData.CorrectWordsCount += correctAnswerCount;
                MessageBox.Show("Правильных ответов " + correctAnswerCount + "/" + allWordsCount, "Результаты");

                fileIOService = new FileIOService(pathStatistic);

                try
                {
                    List<int> list = new List<int>();
                    list.Add(StatisticData.AllWordsCount);
                    list.Add(StatisticData.CorrectWordsCount);
                    fileIOService.SaveStatisticData(list);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Close();
                }

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
                time = 0;

                SliderTimerMinutes.Value = 0;
                SliderTimerMinutes.IsEnabled = false;

                TextBoxTimerMinutes.IsEnabled = false;
                TextBoxTimerMinutes.Text = time.ToString() + " minutes";
            }
        }

        private void ButtonStartEndTesting_Click(object sender, RoutedEventArgs e)
        {
            if (testingStart == false)
            {
                this.Height = 295;

                testingStart = true;

                unit = ComboBoxUnitsTesting.SelectedIndex;
                wordsByChance = (bool)CheckBoxWordsByChance.IsChecked;

                TextBoxAnswer.IsReadOnly = false;
                TextBoxAnswer.Text = "";

                TextBlockAllWordsCount.Visibility = Visibility.Visible;

                ButtonStartEndTesting.Visibility = Visibility.Hidden;
                ButtonStartEndTesting.Content = "Завершить тест";

                ButtonNextQuestion.Visibility = Visibility.Visible;

                ButtonGetAnswer.Visibility = Visibility.Visible;

                ProgressBar.Visibility = Visibility.Visible;

                ComboBoxUnitsTesting.IsEnabled = false;
                CheckBoxWordsByChance.IsEnabled = false;
                CheckBoxTimerMinutes.IsEnabled = false;
                SliderTimerMinutes.IsEnabled = false;
                TextBoxTimerMinutes.IsEnabled = false;
                ButtonNextQuestion.IsEnabled = false;
              
                if (unit != 0)
                {
                    foreach (Word n in EnglishData)
                    {
                        EnglishDataFiltered = (from k in EnglishData where (Convert.ToInt32(k.Unit) == unit) select k).ToList();
                    }

                    allWordsCount = EnglishDataFiltered.Count;

                    TextBlockQuestion.Text = EnglishDataFiltered[wordIndex].TranslateWord.ToString();
                }
                else
                {
                    TextBlockQuestion.Text = EnglishData[wordIndex].TranslateWord.ToString();
                }
                    
                TextBlockAllWordsCount.Text = nowWordNumber.ToString() + "/" + allWordsCount.ToString();
                ProgressBar.Maximum = allWordsCount;
            }

            if (testingFinal == true)
            {
                exit = true;
                this.Close();
            }

            IsTheLastQuestion();
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
            TextBoxAnswer.Foreground = Brushes.Black;
            TextBoxAnswer.Text = "";
            TextBlockCorrectAnswer.Text = "Correct answer: ";
            TextBlockCorrectAnswer.FontWeight = FontWeights.Normal;
            nowWordNumber++;
            ProgressBar.Value = nowWordNumber;
            TextBlockAllWordsCount.Text = nowWordNumber.ToString() + "/" + allWordsCount.ToString();
            ButtonNextQuestion.IsEnabled = false;
            ButtonGetAnswer.IsEnabled = true;
            wordIndex++;
            if (unit != 0)
            {
                TextBlockQuestion.Text = EnglishDataFiltered[wordIndex].TranslateWord.ToString();
            }
            else
                TextBlockQuestion.Text = EnglishData[wordIndex].TranslateWord.ToString();

            IsTheLastQuestion();
        }

        /* 
        Метод проверяет очередность данного вопроса: последний или нет. 
        Если вопрос последний (так же если в тесте всего 1 вопрос), то не должно быть кнопки далее
        Должна быть только кнопка ответить 
        */
        private void IsTheLastQuestion()
        {
            if (nowWordNumber == allWordsCount)
            {
                ButtonNextQuestion.Visibility = Visibility.Hidden;
                ButtonGetAnswer.Margin = new Thickness(5, 160, 0, 0);
                ButtonGetAnswer.Width = 287;
            }
        }

        // кнопка Ответить
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
            TextBlockCorrectAnswer.FontWeight = FontWeights.Bold;
            if (unit != 0)
            {
                if (string.Equals(EnglishDataFiltered[wordIndex].EnglishWord, TextBoxAnswer.Text) == true)
                {
                    TextBoxAnswer.Foreground = Brushes.Green;
                    correctAnswerCount++;
                    
                }
                else
                {
                    TextBoxAnswer.Foreground = Brushes.Red;
                }
                TextBlockCorrectAnswer.Text = "Correct answer: " + EnglishDataFiltered[wordIndex].EnglishWord;
            }
            else
            {
                if (string.Equals(EnglishData[wordIndex].EnglishWord, TextBoxAnswer.Text) == true)
                {
                    TextBoxAnswer.Foreground = Brushes.Green;
                    correctAnswerCount++;
                }
                else
                {
                    TextBoxAnswer.Foreground = Brushes.Red;
                }
                TextBlockCorrectAnswer.Text = "Correct answer: " + EnglishData[wordIndex].EnglishWord;
            }
        }

        

        private void WWW_Click(object sender, RoutedEventArgs e)
        {

            int value = RandomIndex.GetRandomIndex(allWordsCount);
            bool next = false;

            if (IndexOfWords.Count == 0)
            {
                IndexOfWords.Add(value);
                www.Text += value.ToString() + " ";
            }
            else
            {
                while (next == false)
                {
                    if (IndexOfWords.IndexOf(value) == -1)
                    {
                        IndexOfWords.Add(value);
                        www.Text += value.ToString()+ " ";
                        next = true;
                    }
                    else
                    {
                        value = RandomIndex.GetRandomIndex(allWordsCount);
                    }
                }

                
            }


            //Random rnd = new Random();

            //int value = rnd.Next(0, allWordsCount - 1);
            //www.Text += value.ToString() + " ";
            //www.Text += RandomIndex.GetRandomIndex(allWordsCount).ToString() + " ";
        }
    }
}
