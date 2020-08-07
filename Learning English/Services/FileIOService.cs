using Learning_English.Words;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learning_English.Services
{
    class FileIOService
    {
        private readonly string pathWords;
        private readonly string pathUnits;


        public FileIOService(string pathWords, string pathUnits)
        {
            this.pathWords = pathWords;
            this.pathUnits = pathUnits;
        }

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




        public void SaveDataUnits(object UnitsData)
        {
            using (StreamWriter writer = File.CreateText(pathUnits))
            {
                string output = JsonConvert.SerializeObject(UnitsData);
                writer.Write(output);
            }
        }








        public void SaveDataWords(object English_Data)
        {
            using (StreamWriter writer = File.CreateText(pathWords))
            {
                string output = JsonConvert.SerializeObject(English_Data);
                writer.Write(output);
            }
        }
    }
}
