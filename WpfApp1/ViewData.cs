using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using lab3;
using System.Windows;
using LiveCharts.Wpf;
using LiveCharts;
using LiveCharts.Configurations;
using System.Windows.Media;
namespace WpfApp1
{
    // Пользователь вводит для DataArray: Число узлов сетки(TextBox), границы отрезка(TextBox) и выбирает тип сетки(RadioButton или ComboBox или CheckBox)  <summary>
    // ComboBox для выбора функции вычисления компонент поля
    // Пользователь вводит для SplineData: Число узлов сглаживающего сплайна(TextBox), число узлов равномерной сетки, на кот вычисляются значения сплайна (TextBox)
    // значение нормы невязки, при котором происходит остановка итераций(TextBox), максимальное число итераций при минимизации невязки(TextBox)
    // Вывод информации из SplineData
    public class ViewData : IDataErrorInfo
    {
        public V2DataArray? DA_Link;     // Ссылка на DataArray
        public double[] DA_SegBoundaries { get; set; } // Границы отрезка с узлами сетки
        public int DA_NodesNum { get; set; }         // Число узлов сетки
        public bool DA_IsGridUniform { get; set; }  // Сетка равномерна/неравномерная
        public int DA_FunctionID { get; set; }  // Функция для инициализации


        public SplineData? SD_Link;      // Ссылка на SplineData
        public int SD_NodesNum { get; set; }         // Число узлов сглаживающего сплайна (для построения)
        public int SD_UniformNodesNum { get; set; } // Число узлов равномерной сетки, на которой вычисляются значения сплайна
        public double SD_BreakConditionNorma { get; set; } // Значение нормы невязки для остановки
        public int SD_MaxItersNum { get; set; } // Масимальное число итераций   


        public CartesianChart ChartModelSpline { get; set; } // График проходящий через точки сплайн апроксимации
        public CartesianChart ChartModelFunc { get; set; } // Точки значений функции на сетке
        public ViewData()
        {
            DA_SegBoundaries = new double[2];
            DA_Link = null;
            SD_Link = null;
            this.ChartModelSpline = new CartesianChart(); 
        }
        public void InitChartModelSpline()
        {
           
            ChartModelSpline.Series.Clear();
            
            var SplineValues = new ChartValues<Point>();
            var FuncValues = new ChartValues<Point>();
            InitChartSplinePoints(SplineValues);
            InitChartFuncPoints(FuncValues);
            this.ChartModelSpline.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Configuration = new CartesianMapper<Point>()
                    .X(point => point.X) // Define a function that returns a value that should map to the x-axis
                    .Y(point => point.Y), // Define a function that returns a value that should map to the y-axis
                    Title = "Series",
                    Values = SplineValues,
                    PointGeometry = null,

                },

                new ScatterSeries
                {
                    Configuration = new CartesianMapper<Point>()
                    .X(point => point.X) // Define a function that returns a value that should map to the x-axis
                    .Y(point => point.Y), // Define a function that returns a value that should map to the y-axis
                    Title = "Series",
                    Values = FuncValues,
                },

            };
        }
        private void InitChartSplinePoints(ChartValues<Point> L)
        {
            if(SD_Link is null)
            {
                throw new Exception("SD_Link is null");
            }
            else
            {
                for(int i = 0; i < SD_UniformNodesNum; ++i)
                {
                    var point = new Point() { X = SD_Link.ResultOnAddonGrid[i].Node, Y = SD_Link.ResultOnAddonGrid[i].SplineValue };
                    L.Add(point);
                }
            }
        }
        private void InitChartFuncPoints(ChartValues<Point> L)
        {
            if(DA_Link is null)
            {
                throw new Exception("DA_Lind is null");
            }
            else
            {
                for (int i = 0; i < DA_NodesNum; ++i)
                {
                    var point = new Point() { X = DA_Link.Net[i], Y = DA_Link.Field_values[0, i]};
                    L.Add(point);
                }
            }
        }
        
        public void InitDAThroughControl()
        {
            FValues F = Functions.FVFunc[DA_FunctionID];
            DA_Link = new V2DataArray("Moonlight", new DateTime(), DA_NodesNum, DA_SegBoundaries[0], DA_SegBoundaries[1], F);
        }
        public void CalcSpline()
        {
            if (SD_Link is null)
            {
                throw new Exception("SplineData is null");
            }
            Func<double, double> F_Init = Functions.f_Init;
            SD_Link.SplineMklCall(F_Init);
        }
        public void InitSD()
        {
            if(DA_Link is null)
            {
                throw new Exception("DataArray is null");
            }
            SD_Link = new SplineData(DA_Link, SD_NodesNum, SD_MaxItersNum, SD_UniformNodesNum);
            SD_Link.DebugMod = false;
        }

        public bool Save(string filename)
        {
            if (DA_Link is null)
            {
                throw new Exception("DataArray is null");
            }
            return DA_Link.Save(filename); //false => исключение
        }

        public bool Load(string filename)
        {
            DA_Link = new V2DataArray("", new DateTime()); // Нулевая инициализация
            bool status = V2DataArray.Load(filename, ref DA_Link);
            if (status)
            {
                DA_NodesNum = DA_Link.Net.Length;
                DA_SegBoundaries[0] = DA_Link.Net[0];
                DA_SegBoundaries[1] = DA_Link.Net[DA_NodesNum - 1];
            }
            return status;
        }

        public string this[string ColumnName]
        {
            get
            {
                string error = string.Empty;
                switch (ColumnName)
                {
                    case "DA_NodesNum":
                        if(DA_NodesNum < 3)
                        {
                            error += "Число узлов сетки должно быть не менее трёх.\n";
                        }
                        break;
                    case "SD_UniformNodesNum":
                        if(SD_UniformNodesNum <= 3)
                        {
                            error += "Число узлов равномерной сетки, на которой вычисляются значения сплайна должно быть не менее трёх.\n";
                        }
                        break;
                    case "DA_SegBoundaries":
                        if (DA_SegBoundaries[0] >= DA_SegBoundaries[1])
                        {
                            error += "Левый конец отрезка, на котором заданы дискретные значения функции, должен быть меньше чем правый.\n";
                        }
                        break;
                    case "SD_NodesNum":
                        if(SD_NodesNum < 2)
                        {
                            error += "Число узлов сглаживающего сплайна должно быть больше или равно двум.\n";
                        }
                        if(SD_NodesNum > DA_NodesNum)
                        {
                            error += "Число узлов сглаживающего сплайна должно быть не больше числа заданных дискретных значений функции.\n";
                        }
                        break;
                }
                return error;
            }
        }
        public string Error
        {
            get { throw new NotImplementedException(); }
        }

    }
}
