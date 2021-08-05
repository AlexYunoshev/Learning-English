using Learning_English.Classes;
using Learning_English.Services;
using Learning_English.Words;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Learning_English
{
    /// <summary>
    /// Логика взаимодействия для Testing.xaml
    /// </summary>
    public partial class Testing : Window
    {
        private bool testingIsStart = false; // начало тестирования
        private bool exit = false; // выход из окна
        private bool wordsByChance = false; // слова вразброс ДА или НЕТ (по умолчанию нет)
        private bool timerIsEnable = false;

        private int allWordsCount; // количество всех вопросов (всех слов)
        private int nowWordNumber = 0; // текущий вопрос (текущее слово)
        private int unitIndex = 0; // 0 = All текущий юнит
        private int wordIndex = 0; // индекс слова в коллекции
        private int correctAnswerCount = 0; // количество правильных ответов = 0

        private TimeSpan timerValue;
        private DispatcherTimer timer;

        private List<Word> EnglishData; // коллекция словаря

 

        private List<int> UnitsData = new List<int>(); // список разделов (Units)
       

        private FileIOService fileIOService;

        private readonly string pathStatistic = $"{Environment.CurrentDirectory}\\StatisticList.json"; // путь файла "статистика"

        Random r = new Random();

        public Testing(BindingList<Word> EnglishData, List<int> UnitsData)
        {
            InitializeComponent();
           
            allWordsCount = EnglishData.Count; // количество всех слов
            this.EnglishData = EnglishData.ToList(); // коллекция слов
            this.UnitsData = UnitsData; // коллекция юнитов
            UpdateComboBox();
            SliderTimerMinutes.ValueChanged += Slider_ValueChanged;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ProgressBar.Minimum = 0;
            ProgressBar.Maximum = allWordsCount;
            ProgressBar.Value = 0;
            TextBlockAllWordsCount.Text = "Пройдено вопросов: " + nowWordNumber.ToString() + "/" + allWordsCount.ToString();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (exit == true)
            {
                StatisticData.AllWordsCount += allWordsCount;
                StatisticData.CorrectWordsCount += correctAnswerCount;
                MessageBox.Show("Правильных ответов: " + correctAnswerCount + "/" + allWordsCount, "Результаты");

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
                }
                RandomIndex.Reset();
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }
            else
            {
                System.Windows.MessageBoxResult result = MessageBox.Show("Вы действительно хотите выйти?\nРезультаты тестирования не сохранятся!", "ВНИМАНИЕ", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    RandomIndex.Reset();
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void ButtonStartEndTesting_Click(object sender, RoutedEventArgs e)
        {
            if (testingIsStart == true)
            {
                exit = IsTheLastQuestion();
                this.Close();
            }

            else
            {
                if (timerIsEnable == true)
                {
                    timerValue = new TimeSpan(0, Convert.ToInt32(SliderTimerMinutes.Value), 0);
                    timer = new DispatcherTimer();
                    timer.Tick += new EventHandler(timer_Tick);
                    timer.Interval = new TimeSpan(0, 0, 1);
                    timer.Start();
                }

                testingIsStart = true;
                ButtonStartEndTesting.Content = "Завершить тест";
                unitIndex = ComboBoxUnitsTesting.SelectedIndex;
                wordsByChance = (bool)CheckBoxWordsByChance.IsChecked;
                TextBoxAnswer.IsEnabled = true;
                TextBoxAnswer.Text = "";
                ButtonGetAnswer.IsEnabled = true;

                ComboBoxUnitsTesting.IsEnabled = false;
                CheckBoxWordsByChance.IsEnabled = false;
                CheckBoxTimerMinutes.IsEnabled = false;
                SliderTimerMinutes.IsEnabled = false;
                

                if (unitIndex != 0)
                {
                    EnglishData = (from k in EnglishData where (Convert.ToInt32(k.Unit) == unitIndex) select k).ToList();
                    allWordsCount = EnglishData.Count;
                }

                if (wordsByChance == true)
                {
                    wordIndex = RandomIndex.GetIndex(allWordsCount);
                }
                TextBlockQuestion.Text = EnglishData[wordIndex].TranslateWord.ToString();
                TextBlockAllWordsCount.Text = "Пройдено вопросов: " + nowWordNumber.ToString() + "/" + allWordsCount.ToString();
                ProgressBar.Maximum = allWordsCount;   
            }
        }

        // кнопка Далее
        private void ButtonNextQuestion_Click(object sender, RoutedEventArgs e)
        {
            TextBoxAnswer.IsReadOnly = false;
            TextBoxAnswer.Foreground = Brushes.Black;
            TextBlockCorrectAnswer.FontWeight = FontWeights.Normal;
            TextBoxAnswer.Text = "";
            TextBlockCorrectAnswer.Text = "Correct answer is: ";
           
            ButtonNextQuestion.IsEnabled = false;
            ButtonGetAnswer.IsEnabled = true;
            wordIndex++;

            if (wordsByChance == true)
            {
                wordIndex = RandomIndex.GetIndex(allWordsCount);
            }

            TextBlockQuestion.Text = EnglishData[wordIndex].TranslateWord.ToString();
        }

        // кнопка Ответить
        private void ButtonGetAnswer_Click(object sender, RoutedEventArgs e)
        {
            TextBoxAnswer.IsReadOnly = true;
            ButtonGetAnswer.IsEnabled = false;
            TextBlockCorrectAnswer.FontWeight = FontWeights.Bold;
            nowWordNumber++;
            ProgressBar.Value = nowWordNumber;
            TextBlockAllWordsCount.Text = "Пройдено вопросов: " + nowWordNumber.ToString() + "/" + allWordsCount.ToString();

            if (ButtonNextQuestion.IsEnabled == false && ProgressBar.Value != ProgressBar.Maximum)
            {
                ButtonNextQuestion.IsEnabled = true; 
            }

            if (string.Equals(EnglishData[wordIndex].EnglishWord, TextBoxAnswer.Text) == true)
            {
                TextBoxAnswer.Foreground = Brushes.Green;
                correctAnswerCount++;
            }
            else
            {
                TextBoxAnswer.Foreground = Brushes.Red;
            }
            TextBlockCorrectAnswer.Text = "Correct answer is: " + EnglishData[wordIndex].EnglishWord;
        }




 
        private void timer_Tick(object sender, EventArgs e)
        {
           
            timerValue = timerValue - TimeSpan.FromSeconds(1);
            TextBoxTimerMinutes.Text = timerValue.ToString(@"mm\:ss");
            if (TimeSpan.Compare(timerValue, new TimeSpan(0,0,0)) == 0)
            {
                exit = true;
                this.Close();
                //MessageBox.Show("Time end");
            }

        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TextBoxTimerMinutes.Text = Convert.ToInt32(SliderTimerMinutes.Value).ToString() + " minutes";
        }

        private void CheckBoxTimerMinutes_Click(object sender, RoutedEventArgs e)
        {
            if (CheckBoxTimerMinutes.IsChecked == true)
            {
                SliderTimerMinutes.IsEnabled = true;
                TextBoxTimerMinutes.IsEnabled = true;
                timerIsEnable = true;



            }
            else
            {
                //SliderTimerMinutes.Value = 0;
                SliderTimerMinutes.IsEnabled = false;
                TextBoxTimerMinutes.IsEnabled = false;
                TextBoxTimerMinutes.Text = Convert.ToInt32(SliderTimerMinutes.Value).ToString() + " minutes";
                timerIsEnable = false;
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

       

        /* 
        Метод проверяет очередность данного вопроса: последний или нет. 
        Если вопрос последний (так же если в тесте всего 1 вопрос), то не должно быть кнопки далее
        Должна быть только кнопка ответить 
        */
        private bool IsTheLastQuestion()
        {
            if (nowWordNumber == allWordsCount)
            {
                ButtonNextQuestion.Visibility = Visibility.Hidden;
                ButtonGetAnswer.Margin = new Thickness(5, 160, 0, 0);
                ButtonGetAnswer.Width = 287;
                return true;
            }
            return false;
        }

       
    }
}