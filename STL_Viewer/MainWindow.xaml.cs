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
using System.Windows.Shapes;
using System.Configuration;

namespace STL_Viewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly string defaultFolderPath = ConfigurationManager.AppSettings["defaultFolderPath"].ToString();
        public MainWindow()
        {
            InitializeComponent();
            folderLbl.Content = defaultFolderPath;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.InitialDirectory = defaultFolderPath;
            dialog.Filter = "STL file(*.stl)|*.stl|All files(*.*)|*.*";
            dialog.FilterIndex = 0;

            Nullable<bool> result = dialog.ShowDialog();

            if (result == true)
            {
                folderLbl.Content = dialog.FileName;
                Create3dViewPort(dialog.FileName);
            }
        }

        private void Create3dViewPort(string filePath)
        {
            var device3D = new ModelVisual3D();
            device3D.Content = Display3d(filePath);
            viewPort.Children.Add(device3D);
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

    }
}
