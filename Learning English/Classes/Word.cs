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
        private string unit;
       
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

        public string Unit
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
    }
}
