using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using helix = HelixToolkit.Wpf;

namespace TowerModeling
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            List<double[]> lines = ReadLines();
            CreateTower(lines);
        }
        // 读取csv文件
        public List<double[]> ReadLines()
        {
            List<double[]> lines = new List<double[]>();
            string[] lines1 = System.IO.File.ReadAllLines(@"D:\cvml\repos\TowerModeling\lines.csv");
            foreach (string line in lines1)
            {
                string[] line1 = line.Split(',');
                double[] line2 = new double[6];
                for (int i = 0; i < 6; i++)
                {
                    line2[i] = Convert.ToDouble(line1[i]);
                }
                lines.Add(line2);
            }

            return lines;
        }
        private void CreateTower(List<double[]> lines)
        {
            // 创建模型组
            Model3DGroup modelGroup = new Model3DGroup();

            foreach (double[] line in lines)
            {
                double height = Math.Sqrt(Math.Pow(line[3] - line[0], 2) + Math.Pow(line[4] - line[1], 2) + Math.Pow(line[5] - line[2], 2));
                GeometryModel3D model = CreateAngelIron(height: height + 10);
                model.Transform = Transform(line);
                modelGroup.Children.Add(model);
            }

            // 创建模型视图
            ModelVisual3D modelVisual = new ModelVisual3D
            {
                Content = modelGroup
            };
            helixView.Children.Add(modelVisual);

        }
        // 创建角钢
        private GeometryModel3D CreateAngelIron(double length = 5, double width = 1, double height = 300)
        {
            // 创建材质
            Material material = new DiffuseMaterial(new SolidColorBrush(Colors.Silver));
            // 建模
            helix.MeshBuilder meshBuilder = new helix.MeshBuilder();
            meshBuilder.AddPolygon(new List<Point3D> { new Point3D(-length, -length, -height / 2), new Point3D(-length, length, -height / 2), new Point3D(-length + width, length, -height / 2), new Point3D(-length + width, -length + width, -height / 2) });
            meshBuilder.AddPolygon(new List<Point3D> { new Point3D(-length + width, -length + width, -height / 2), new Point3D(length, -length + width, -height / 2), new Point3D(length, -length, -height / 2), new Point3D(-length, -length, -height / 2) });

            meshBuilder.AddPolygon(new List<Point3D> { new Point3D(-length + width, -length + width, height / 2), new Point3D(-length + width, length, height / 2), new Point3D(-length, length, height / 2), new Point3D(-length, -length, height / 2) });
            meshBuilder.AddPolygon(new List<Point3D> { new Point3D(-length, -length, height / 2), new Point3D(length, -length, height / 2), new Point3D(length, -length + width, height / 2), new Point3D(-length + width, -length + width, height / 2) });

            meshBuilder.AddPolygon(new List<Point3D> { new Point3D(-length, -length, -height / 2), new Point3D(-length, -length, height / 2), new Point3D(-length, length, height / 2), new Point3D(-length, length, -height / 2) });
            meshBuilder.AddPolygon(new List<Point3D> { new Point3D(-length, -length, height / 2), new Point3D(-length, -length, -height / 2), new Point3D(length, -length, -height / 2), new Point3D(length, -length, height / 2) });

            meshBuilder.AddPolygon(new List<Point3D> { new Point3D(-length, length, -height / 2), new Point3D(-length, length, height / 2), new Point3D(-length + width, -length + width, height / 2), new Point3D(-length + width, -length + width, -height / 2) });
            meshBuilder.AddPolygon(new List<Point3D> { new Point3D(length, -length, height / 2), new Point3D(length, -length, -height / 2), new Point3D(-length + width, -length + width, -height / 2), new Point3D(-length + width, -length + width, height / 2) });

            meshBuilder.AddPolygon(new List<Point3D> { new Point3D(-length, length, -height / 2), new Point3D(-length, length, height / 2), new Point3D(-length + width, length, height / 2), new Point3D(-length + width, length, -height / 2) });
            meshBuilder.AddPolygon(new List<Point3D> { new Point3D(length, -length + width, -height / 2), new Point3D(length, -length + width, height / 2), new Point3D(length, -length, height / 2), new Point3D(length, -length, -height / 2) });



            MeshGeometry3D mesh = meshBuilder.ToMesh();
            GeometryModel3D model = new GeometryModel3D(mesh, material);
            return model;
        }
        private Transform3DGroup Transform(double[] line)
        {
            // 角钢移动到对于位置所需旋转角
            Vector3D var1 = new Vector3D(line[3] - line[0], line[4] - line[1], line[5] - line[2]);
            Vector3D var2 = new Vector3D((line[3] + line[0]) / 2, (line[4] + line[1]) / 2, (line[5] + line[2]) / 2);
            Vector3D axi1 = Vector3D.CrossProduct(new Vector3D(0, 0, 1), var1);
            double theta1 = Vector3D.AngleBetween(new Vector3D(0, 0, 1), var1);

            // 角钢自转旋转角
            Vector3D var3 = new Vector3D(1, 1, 0);
            Vector3D var4 = new Vector3D((line[3] + line[0]) / -2, (line[4] + line[1]) / -2, 0);
            Vector3D axi2 = Vector3D.CrossProduct(var3, var4);
            double theta2 = Vector3D.AngleBetween(var3, var4);


            Transform3DGroup group = new Transform3DGroup();
            group.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(axi2, theta2)));
            group.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(axi1, theta1)));
            group.Children.Add(new TranslateTransform3D(var2));

            return group;
        }
    }
}
