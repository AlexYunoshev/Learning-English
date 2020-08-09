﻿using System;
using System.Collections.Generic;
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
using Learning_English.Classes;

namespace Learning_English
{
    /// <summary>
    /// Interaction logic for Statistic.xaml
    /// </summary>
    public partial class Statistic : Window
    {
        public Statistic()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlockAllWordsCount.Text = "Всего вопросов: " + StatisticData.AllWordsCount;
            TextBlockCorrectWordsCount.Text = "Правильных ответов: " + StatisticData.CorrectWordsCount;
            TextBlockIncorrectWordsCount.Text = "Неправильных ответов: " + (StatisticData.AllWordsCount - StatisticData.CorrectWordsCount);
        }
    }
}
