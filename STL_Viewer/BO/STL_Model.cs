using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STL_Viewer.BO
{
    public class STL_Model
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }        
        public List<string> Tags { get; set; }
        
        public STL_Model() { }       

        public STL_Model(Guid guid, string name, string fileName, string filePath, string tagsDelimitedString)
        {
            Guid = guid;
            Name = name;
            FileName = fileName;
            FilePath = filePath;            
            Tags = tagsDelimitedString.Split('|').ToList();
            
        }
    }
}
