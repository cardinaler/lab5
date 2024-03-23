using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        ViewData VD;
        public MainWindow()
        {
            InitializeComponent();
            VD = new ViewData();
            BindConnections(VD); // Установка привязки с элементами управления
        }


        public void BindConnections(ViewData VD)
        {
            Binding Binding_DAFunctionID = new Binding(); // Выбор функции
            Binding_DAFunctionID.Source = VD;
            Binding_DAFunctionID.Path = new PropertyPath("DA_FunctionID");
            DAFunctionComboBox.SetBinding(ComboBox.SelectedIndexProperty, Binding_DAFunctionID);

            SegBoundariesConverter SD_Converter = new SegBoundariesConverter();

            Binding Binding_DASegBoundaries = new Binding(); // Ввод границ равномерной сетки
            Binding_DASegBoundaries.Source = VD;
            Binding_DASegBoundaries.Path = new PropertyPath("DA_SegBoundaries");
            Binding_DASegBoundaries.Converter = SD_Converter;
            DASegBoundariesBox.SetBinding(TextBox.TextProperty, Binding_DASegBoundaries);

            Binding Binding_DANodesNum = new Binding(); // Ввод числа узлов сетки
            Binding_DANodesNum.Source = VD;
            Binding_DANodesNum.Path = new PropertyPath("DA_NodesNum");
            DANodesNumBox.SetBinding(TextBox.TextProperty, Binding_DANodesNum);

            Binding Binding_SDNodesNum = new Binding(); // Ввод числа узлов сглаживающего сплайна
            Binding_SDNodesNum.Source = VD;
            Binding_SDNodesNum.Path = new PropertyPath("SD_NodesNum");
            SDNodesNumBox.SetBinding(TextBox.TextProperty, Binding_SDNodesNum);

            Binding Binding_SDUniformNodsNum = new Binding(); // Ввод числа узлов равномерной сетки
            Binding_SDUniformNodsNum.Source = VD;
            Binding_SDUniformNodsNum.Path = new PropertyPath("SD_UniformNodesNum");
            SDUniformNodesNumBox.SetBinding(TextBox.TextProperty, Binding_SDUniformNodsNum);

            Binding Binding_SDBreakConditionNorma = new Binding(); // Ввод значение нормы невязки для остановки
            Binding_SDBreakConditionNorma.Source = VD;
            Binding_SDBreakConditionNorma.Path = new PropertyPath("SD_BreakConditionNorma");
            SDBreakConditionNormaBox.SetBinding(TextBox.TextProperty, Binding_SDBreakConditionNorma);

            Binding Binding_SDMaxItersNum = new Binding(); // Ввод максимально допустимого числа итераций
            Binding_SDMaxItersNum.Source = VD;
            Binding_SDMaxItersNum.Path = new PropertyPath("SD_MaxItersNum");
            SDMaxItersNumBox.SetBinding(TextBox.TextProperty, Binding_SDMaxItersNum);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(DAIsGridUniformComboBox.SelectedIndex == 0)
            {
                VD.DA_IsGridUniform = true;
            }
            else
            {
                VD.DA_IsGridUniform = false;
            }
            MessageBox.Show(VD.SD_MaxItersNum.ToString());
        }

        private void SaveItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                string FilePath = "";
                if (saveFileDialog.ShowDialog() == true)
                {
                    FilePath = saveFileDialog.FileName;
                }
                VD.InitDAThroughControl();
                VD.Save(FilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString()); 
            }
        }

        private void DataFromControlsItem_Click(object sender, RoutedEventArgs e)
        {
            try 
            {
                VD.InitDAThroughControl();
                VD.InitSD();
                VD.CalcSpline();
                SplineValuesList.ItemsSource = VD.SD_Link.ApproximationRes;
                UniformGridValuesList.ItemsSource = VD.SD_Link.ResultOnAddonGrid;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DataFromFileItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                string FilePath = "";
                if (openFileDialog.ShowDialog() == true)
                {
                    FilePath = openFileDialog.FileName;
                }
                VD.Load(FilePath);
                VD.InitSD();
                VD.CalcSpline();
                SplineValuesList.ItemsSource = VD.SD_Link.ApproximationRes;
                UniformGridValuesList.ItemsSource = VD.SD_Link.ResultOnAddonGrid;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }
    }
}
