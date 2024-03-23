using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lab3;
namespace WpfApp1
{
    // Пользователь вводит для DataArray: Число узлов сетки(TextBox), границы отрезка(TextBox) и выбирает тип сетки(RadioButton или ComboBox или CheckBox)  <summary>
    // ComboBox для выбора функции вычисления компонент поля
    // Пользователь вводит для SplineData: Число узлов сглаживающего сплайна(TextBox), число узлов равномерной сетки, на кот вычисляются значения сплайна (TextBox)
    // значение нормы невязки, при котором происходит остановка итераций(TextBox), максимальное число итераций при минимизации невязки(TextBox)
    // Вывод информации из SplineData
    public class ViewData
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
        public ViewData()
        {
            DA_SegBoundaries = new double[2];
            DA_Link = null;
            SD_Link = null;
        }

        public void InitDAThroughControl()
        {
            FValues F = Functions.FVFunc[DA_FunctionID];
            if (DA_SegBoundaries[1] == 0 || DA_SegBoundaries[1] <= DA_SegBoundaries[0])
            {
                throw new Exception("Некорректные границы отрезка");
            }
            if (DA_NodesNum <= 1)
            {
                throw new Exception("Некорректное число узлов");
            }
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
            if(SD_NodesNum <= 1)
            {
                throw new Exception("Некорректное число узлов сглаживающего сплайна");
            }
            if(SD_UniformNodesNum <= 1)
            {
                throw new Exception("Некорректное число узлов равномерной сетки");
            }
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
            return V2DataArray.Load(filename, ref DA_Link);
        }
    }
}
