using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Saving_InfoLog_ClassLibrary
{
    public class SavingMethod
    {
        private const string jsonFilePathLDB = "LogDB.json";

        public List<string> GetAllInfoLog()
        {

            List<string> log = ReadJson(jsonFilePathLDB);
            return log;
        }

        public void SaveInfoLog(List<string> log)
        {
            WriteJson(jsonFilePathLDB, log);
        }

        private void WriteJson(string filePath, List<string> log)
        {
            string json = JsonConvert.SerializeObject(log);

            using (StreamWriter sw = new StreamWriter(filePath, true))
            {
                sw.Write(json);
            }
        }

        private List<string> ReadJson(string filePath)
        {
            if (!File.Exists(jsonFilePathLDB))
            {
                return new List<string>();
            }
            using (StreamReader sr = new StreamReader(filePath))
            {
                string json = sr.ReadToEnd();

                return JsonConvert.DeserializeObject<List<string>>(json);
            }
        }
    }
}
