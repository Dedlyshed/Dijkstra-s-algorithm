using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace DeskretnaLab5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Regex regex = new Regex("(\\d+)-(\\d+) (\\d);");
            Match match = regex.Match(richTextBox1.Text);
            Regex startFinish = new Regex("(\\d+)>>(\\d+)");
            Match startFinishMatch = startFinish.Match(richTextBox1.Text);
            int vertexStart = int.Parse(startFinishMatch.Groups[1].Value);
            int vertexFinish = int.Parse(startFinishMatch.Groups[2].Value);
            int[,] nArr = new int[regex.Matches(richTextBox1.Text).Count, 3];
            richTextBox2.Text += "Входные данные: \n";
            for (int i = 0; match.Success && i < nArr.GetLength(0); i++)
            {
                for (int j = 0; j < nArr.GetLength(1); j++)
                    nArr[i, j] = int.Parse(match.Groups[j + 1].Value);
                richTextBox2.Text += $"Ребро: {nArr[i, 0]}-{nArr[i, 1]}, вес: {nArr[i, 2]}\n";
                match = match.NextMatch();
            }
            richTextBox2.Text += $"Вершина начала: {vertexStart}, вершина конца: {vertexFinish}\n";
            int[] tmpVertexArr = new int[regex.Matches(richTextBox1.Text).Count * 2];
            bool isRepeat;
            int vertexAmount = 0;
            for (int i = 0; i < nArr.GetLength(0); i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    isRepeat = false;
                    foreach (int x in tmpVertexArr)
                        if (x == nArr[i, j])
                        {
                            isRepeat = true;
                            break;
                        }
                    if (!isRepeat)
                    {
                        tmpVertexArr[vertexAmount] = nArr[i, j];
                        vertexAmount++;
                    }
                }
            }
            int[] vertexArr = new int[vertexAmount];
            int counter = 0;
            foreach (int x in tmpVertexArr)
            {
                if (x != 0) vertexArr[counter] = x;
                counter++;
            }
            Array.Sort(vertexArr);
            richTextBox2.Text += "\nИтак, имеем вершины:\n";
            foreach (int l in vertexArr) richTextBox2.Text += " " + l.ToString();
            //массив хранит таблицу смежности, значения массива - вес ребра, 0 - если ребро отсутствует
            int[,] adjacencyTable = new int[vertexArr.Length + 1, vertexArr.Length + 1];
            //цикл вводит вершины в таблицу
            for (int x = 1; x < adjacencyTable.GetLength(0); x++)
            {
                adjacencyTable[0, x] = vertexArr[x - 1];
                adjacencyTable[x, 0] = vertexArr[x - 1];
            }
            for (int i = 0; i < nArr.GetLength(0); i++)
            {
                //цикл по горизонтали ищет название первой вершины из nArr
                for (int x = 1; x < adjacencyTable.GetLength(0); x++)
                {
                    if (nArr[i, 0] == adjacencyTable[0, x])
                    {
                        // если находит, то цикл ищет название второй вершины из nArr
                        for (int y = 1; y < adjacencyTable.GetLength(1); y++)
                        {
                            if (nArr[i, 1] == adjacencyTable[0, y])
                            {
                                //если находит, то вписывает вес ребра между вершинами
                                adjacencyTable[x, y] = nArr[i, 2];
                                //из-за того, что граф неориентирован, то он симетричен
                                adjacencyTable[y, x] = nArr[i, 2];
                            }
                        }
                    }
                }
            }
            richTextBox2.Text += "\n\nТаблица смежности с весом ребер: \n";
            for (int i = 0; i < adjacencyTable.GetLength(0); i++)
            {
                if (i < 10) richTextBox2.Text += " ";
                for (int j = 0; j < adjacencyTable.GetLength(1); j++)
                {
                    richTextBox2.Text += "  " + adjacencyTable[i, j];
                }
                richTextBox2.Text += "\n";
            }
            //vertex[x, y], if y == 0 - название вершины, if y == 1 - метка вершины,
            //if y == 2 - состояние вершины: vertex[x, 2] == 0 - не открыта, vertex[x, 2] == 1 - открыта, vertex[x, 2] == -1 - проверена
            int[,] vertex = new int[vertexArr.GetLength(0), 3];
            int indexVertexStart = 0;
            int indexVertexFinish = 0;
            for (int i = 0; i < vertexArr.GetLength(0); i++)
            {
                vertex[i, 0] = vertexArr[i];
                vertex[i, 1] = 999999999;
                if (vertex[i, 0] == vertexStart)
                {
                    vertex[i, 1] = 0;
                    vertex[i, 2] = 1;
                    indexVertexStart = i;
                }
                else if (vertex[i, 0] == vertexFinish) indexVertexFinish = i;
            }
            int currentVertex = vertex[indexVertexStart, 0];
            int currentVertexIndex = 0;
            int tmp = 0;
            while (vertex[indexVertexFinish, 2] != -1 && tmp < 100)
            {
                //поиск открытой точки
                for (int i = 0; i < vertex.GetLength(0); i++)
                {
                    if (vertex[i, 2] == 1)
                    {
                        currentVertex = vertex[i, 0];
                        currentVertexIndex = i;
                        break;
                    }
                }
                richTextBox2.Text += "\nПроверяем вершину " + currentVertex + "\n";
                for (int i = 1; i < adjacencyTable.GetLength(0); i++)
                {
                    if (adjacencyTable[i, 0] == currentVertex)
                    {
                        for (int j = 1; j < adjacencyTable.GetLength(1); j++)
                        {
                            if (adjacencyTable[i, j] != 0)
                            {
                                for (int x = 0; x < vertex.GetLength(0); x++)
                                {
                                    if (vertex[x, 0] == adjacencyTable[0, j]) {
                                        richTextBox2.Text += $"Найдено соседнюю вершину {vertex[x, 0]}, ее метка {vertex[x, 1]}\n";
                                        //делает вершину открытой
                                        vertex[x, 2] = 1; 
                                        //если метка от текущей вершины к новой соседней меньше предыдущей найденной, то она заменяется
                                        
                                        if (vertex[x, 1] > vertex[currentVertexIndex, 1] + adjacencyTable[i, j])
                                        {
                                            richTextBox2.Text += $"Метка с текущей вершины на соседнюю меньше, чем предыдущая ({vertex[x, 1]}), уменьшаем до {vertex[currentVertexIndex, 1] + adjacencyTable[i, j]}\n";
                                            vertex[x, 1] = vertex[currentVertexIndex, 1] + adjacencyTable[i, j];
                                            
                                        }
                                        else richTextBox2.Text += $"Метка с текущей вершины на соседнюю больше ({vertex[currentVertexIndex, 1] + adjacencyTable[i, j]}), чем предыдущая ({vertex[x, 1]}), ничего не меняем\n";
                                        break;
                                    }
                                }
                                //убирает веса пройденных ребер
                                adjacencyTable[i, j] = 0;
                                adjacencyTable[j, i] = 0;
                            }
                        }
                        richTextBox2.Text += "Все соседние вершины просмотрены, вычеркиваем текущую вершину из графа как просмотренную\n";
                        vertex[currentVertexIndex, 2] = -1;
                    }
                }
                tmp++;
            }
            richTextBox2.Text += $"Итого, самый коротки путь из вершины {vertexStart} в {vertexFinish} это {vertex[indexVertexFinish, 1]}\n";
        }
    }
}
