using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learning_English.Words
{
    public class Word: INotifyPropertyChanged
    {
        private string englishWord;
        private string translateWord;
        private int unit;
        private int partOfUnit;
        private bool wordQuizState = true;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string EnglishWord
        {
            get { return englishWord; }
            set
            {
                if (englishWord == value)
                    return;
                englishWord = value;
                OnPropertyChanged("EnglishWord");
            }
        }

        public string TranslateWord
        {
            get { return translateWord; }
            set
            {
                if (translateWord == value)
                    return;
                translateWord = value;
                OnPropertyChanged("TranslateWord");
            }
        }

        public int Unit
        {
            get { return unit; }
            set
            {
                if (unit == value)
                    return;
                unit = value;
                OnPropertyChanged("Unit");
            }
        }

        public int PartOfUnit
        {
            get { return partOfUnit; }
            set
            {
                if (partOfUnit == value)
                    return;
                partOfUnit = value;
                OnPropertyChanged("PartOfUnit");
            }
        }

        public bool WordQuizState
        {
            get { return wordQuizState; }
            set
            {
                if (wordQuizState == value)
                    return;
                wordQuizState = value;
                OnPropertyChanged("WordQuizState");
            }
        }
    }
}
