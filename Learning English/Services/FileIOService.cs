using Learning_English.Words;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Learning_English.Classes;

namespace Learning_English.Services
{
    class FileIOService
    {
        private readonly string pathWords; // путь к файлу со словами
        private readonly string pathUnits; // путь к файлу с юнитами
        private readonly string pathStatistic; // путь к файлу статистики

        public FileIOService(string pathWords, string pathUnits, string pathStatistic)
        {
            this.pathWords = pathWords;
            this.pathUnits = pathUnits;
            this.pathStatistic = pathStatistic;
        }

        public FileIOService(string pathStatistic)
        {
            this.pathStatistic = pathStatistic;
        }

        // Метод загрузки слов
        public BindingList<Word> LoadDataWords()
        {
            var fileExists = File.Exists(pathWords);
            if (!fileExists)
            {
                File.CreateText(pathWords).Dispose();
                return new BindingList<Word>();
            }
            using (var reader = File.OpenText(pathWords))
            {
                var fileText = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<BindingList<Word>>(fileText);
            }
        }

        // Метод сохранения слов
        public void SaveDataWords(object English_Data)
        {
            using (StreamWriter writer = File.CreateText(pathWords))
            {
                string output = JsonConvert.SerializeObject(English_Data);
                writer.Write(output);
            }
        }

        // Метод загрузки юнитов
        public List<int> LoadDataUnits()
        {
            var fileExists = File.Exists(pathUnits);
            if (!fileExists)
            {
                File.CreateText(pathUnits).Dispose();
                return new List<int>();
            }
            using (var reader = File.OpenText(pathUnits))
            {
                var fileText = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<List<int>>(fileText);
            }
        }

        // Метод сохранения юнитов
        public void SaveDataUnits(object UnitsData)
        {
            using (StreamWriter writer = File.CreateText(pathUnits))
            {
                string output = JsonConvert.SerializeObject(UnitsData);
                writer.Write(output);
            }
        }

        // Метод загрузки статистики
        public List<int> LoadStatisticData()
        {
            var fileExists = File.Exists(pathStatistic);
            if (!fileExists)
            {
                File.CreateText(pathStatistic).Dispose();
                return new List<int>();
            }
            using (var reader = File.OpenText(pathStatistic))
            {
                var fileText = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<List<int>>(fileText);
            }
        }

        // Метод сохранения статистики
        public void SaveStatisticData(List<int> list)
        {
            using (StreamWriter writer = File.CreateText(pathStatistic))
            {
                string output = JsonConvert.SerializeObject(list);
                writer.Write(output);
            }
        }
    }
}
