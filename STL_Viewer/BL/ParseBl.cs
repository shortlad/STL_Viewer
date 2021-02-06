using FlatFiles;
using FlatFiles.TypeMapping;
using STL_Viewer.BO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace STL_Viewer.BL
{    
    public static class ParseBl
    {
        public static ModelDB ScanForNewFiles(ModelDB modelDB)
        {
            var stlList = new List<string>();
            var folders = Directory.GetDirectories(modelDB.RootFilePath, "*", SearchOption.AllDirectories);

            foreach(var folder in folders)
            {
                var files = Directory.GetFiles(folder, "*.stl");
                stlList.AddRange(files);
            }

            foreach(var file in stlList)
            {
                if(modelDB.Models.FirstOrDefault(x => x.FilePath == file) == null)
                {
                    var model = new STL_Model() {
                        Guid = Guid.NewGuid(),
                        FileName = Path.GetFileName(file),
                        FilePath = file,
                        Name = Path.GetFileName(file).Split('.')[0]
                    };
                    modelDB.Models.Add(model);
                }
            }            
            WriteToFile(modelDB.SaveToPath, modelDB);
            return modelDB;
        }
        private static dynamic ReadFromFile(string filePath)
        {            
            using (var reader = new StreamReader(File.OpenRead(filePath)))
            {             
                return JsonConvert.DeserializeObject(reader.ReadToEnd().ToString());
            }         
        }

        public static void WriteToFile(string filePath, object obj)
        {            
            using (var writer = new StreamWriter(File.Open(filePath, FileMode.OpenOrCreate)))
            {
                writer.Write(JsonConvert.SerializeObject(obj));
            };
        }        
    }
}
