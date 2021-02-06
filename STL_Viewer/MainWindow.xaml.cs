using HelixToolkit.Wpf;
using System;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Configuration;
using STL_Viewer.BO;
using System.IO;
using STL_Viewer.BL;
using Newtonsoft.Json;

namespace STL_Viewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {   
        private static readonly string modelDBPath = ConfigurationManager.AppSettings["ModelDB"].ToString();
        private ModelDB _modelDB;
        public MainWindow()
        {
            InitializeComponent();
            //ParseBl.ParseTest();
            ModelDB modelDB = new ModelDB();
            if (File.Exists(modelDBPath))
            {
                using (var reader = new StreamReader(File.OpenRead(modelDBPath)))
                {
                    modelDB =JsonConvert.DeserializeObject<ModelDB>(reader.ReadToEnd().ToString());                    
                }
                
            }
            else
            {
                var dialog = new Microsoft.Win32.OpenFileDialog();
                dialog.CheckFileExists = false;
                dialog.CheckPathExists = true;
                dialog.FileName = "Select A Folder";
                if (dialog.ShowDialog() == true)
                {
                    modelDB.Name = "Model 1";
                    modelDB.RootFilePath = Path.GetDirectoryName(dialog.FileName);
                    modelDB.Models = new List<STL_Model>();
                    modelDB.SaveToPath = modelDBPath;
                    using (var writer = new StreamWriter(File.Open(modelDBPath, FileMode.OpenOrCreate)))
                    {
                        writer.Write(JsonConvert.SerializeObject(modelDB));
                    }
                }
                else
                {
                    return;
                }
            }

            foreach (var model in modelDB.Models)
            {
                fileList.Items.Add(new FileListItem() { Name = model.Name, Model = model });
            }
            _modelDB = modelDB;
        }        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _modelDB = ParseBl.ScanForNewFiles(_modelDB);
            UpdateFileList(_modelDB.Models);
        }

        private void UpdateFileList(List<STL_Model> models)
        {
            fileList.Items.Clear();

            foreach (var model in models)
            {
                fileList.Items.Add(new FileListItem() { Name = model.Name, Model = model });
            }
        }

        private void Create3dViewPort(string filePath)
        {
            var device3D = new ModelVisual3D
            {
                Content = Display3d(filePath)
            };
            viewPort.Children.Clear();
            viewPort.Children.Add(new DefaultLights());
            viewPort.Children.Add(device3D);
            viewPort.ZoomExtents();
        }

        private Model3D Display3d(string model)
        {
            Model3D device = null;
            try
            {
                viewPort.RotateGesture = new MouseGesture(MouseAction.LeftClick);

                var import = new ModelImporter();
                device = import.Load(model);
            }catch(Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
            return device;
        }

        private void FileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = (ListBox) sender;
            var selectedItem = (FileListItem)listBox.SelectedItem;
            if(selectedItem?.Model == null)
            {
                return;
            }
            if (selectedItem.Model.Tags?.Count > 0)
            {
                var tags = string.Join(", ", selectedItem.Model.Tags);
                tagsTextBox.Text = tags;
            }
            else
            {
                tagsTextBox.Text = "";
            }
            Create3dViewPort(selectedItem.Model.FilePath);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var selectedItem = (FileListItem)fileList.SelectedItem;
            var tags = tagsTextBox.Text.Split(',').Select(x => x.Trim()).ToList();
            var model = _modelDB.Models.First(x => x.Guid == selectedItem.Model.Guid);
            model.Tags = tags;            
            ParseBl.WriteToFile(_modelDB.SaveToPath, _modelDB);
        }
    }
}
