using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STL_Viewer.BO
{
    public class ModelDB
    {
        public string RootFilePath { get; set; }
        public string Name { get; set; }
        public List<STL_Model> Models { get; set; }
        public string SaveToPath { get; set; }
    }
}
