using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
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
            string path = "D:\\cvml\\repos\\test\\data\\towers_00";
            int i = 0;
            foreach (var subpath in Directory.GetDirectories(path))
            {
                List<double[]> lines1 = ReadLines(subpath + "\\trunks.csv");
                List<double[]> lines2 = ReadLines(subpath + "\\branches.csv");
                // 创建材质
                Material material1 = new DiffuseMaterial(new SolidColorBrush(Colors.Silver));
                Material material2 = new DiffuseMaterial(new SolidColorBrush(Colors.Silver));
                Model3DGroup modelGroup = new Model3DGroup();
                modelGroup = CreateTower(modelGroup, lines1, material1, radius: 2.5);
                modelGroup = CreateTower(modelGroup, lines2, material2, radius: 1);
                //modelGroup = CreateTower(modelGroup, lines1, material1, width: 2, thickness: 0.5);
                //modelGroup = CreateTower(modelGroup, lines2, material2, width: 1, thickness: 0.2);
                // 创建模型视图
                ModelVisual3D modelVisual = new ModelVisual3D
                {
                    Content = modelGroup
                };
                //modelVisual.Transform = new TranslateTransform3D(0, i*500, 0);
                helixView.Children.Add(modelVisual);
                helixView.Export(subpath  + "\\model.stl");
                helixView.Children.Remove(modelVisual);
                i++;
            }
        }


        // 读取csv文件
        public List<double[]> ReadLines(String path)
        {
            List<double[]> lines = new List<double[]>();
            string[] lines1 = System.IO.File.ReadAllLines(path);
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

        private Model3DGroup CreateTower(Model3DGroup modelGroup,List<double[]> lines, Material material, double width, double thickness)
        {

            foreach (double[] line in lines)
            {
                double height = Math.Sqrt(Math.Pow(line[3] - line[0], 2) + Math.Pow(line[4] - line[1], 2) + Math.Pow(line[5] - line[2], 2));
                GeometryModel3D model = CreateAngelIron(material: material, width: width, thickness: thickness, height: height);
                model.Transform = Transform(line);
                modelGroup.Children.Add(model);
            }

            return modelGroup;
        }
        private Model3DGroup CreateTower(Model3DGroup modelGroup, List<double[]> lines, Material material, double radius)
        {
            foreach (double[] line in lines)
            {
                double height = Math.Sqrt(Math.Pow(line[3] - line[0], 2) + Math.Pow(line[4] - line[1], 2) + Math.Pow(line[5] - line[2], 2));
                GeometryModel3D model = CreateCylinder(material: material, radius:radius, height: height);
                model.Transform = Transform(line,false);
                modelGroup.Children.Add(model);
            }

            return modelGroup;
        }
        // 创建角钢
        private GeometryModel3D CreateAngelIron(Material material, double width = 5, double thickness = 1, double height = 300)
        {

            // 建模
            helix.MeshBuilder meshBuilder = new helix.MeshBuilder();
            meshBuilder.AddPolygon(new List<Point3D> { new Point3D(-width, -width, -height / 2), new Point3D(-width, width, -height / 2), new Point3D(-width + thickness, width, -height / 2), new Point3D(-width + thickness, -width + thickness, -height / 2) });
            meshBuilder.AddPolygon(new List<Point3D> { new Point3D(-width + thickness, -width + thickness, -height / 2), new Point3D(width, -width + thickness, -height / 2), new Point3D(width, -width, -height / 2), new Point3D(-width, -width, -height / 2) });

            meshBuilder.AddPolygon(new List<Point3D> { new Point3D(-width + thickness, -width + thickness, height / 2), new Point3D(-width + thickness, width, height / 2), new Point3D(-width, width, height / 2), new Point3D(-width, -width, height / 2) });
            meshBuilder.AddPolygon(new List<Point3D> { new Point3D(-width, -width, height / 2), new Point3D(width, -width, height / 2), new Point3D(width, -width + thickness, height / 2), new Point3D(-width + thickness, -width + thickness, height / 2) });

            meshBuilder.AddPolygon(new List<Point3D> { new Point3D(-width, -width, -height / 2), new Point3D(-width, -width, height / 2), new Point3D(-width, width, height / 2), new Point3D(-width, width, -height / 2) });
            meshBuilder.AddPolygon(new List<Point3D> { new Point3D(-width, -width, height / 2), new Point3D(-width, -width, -height / 2), new Point3D(width, -width, -height / 2), new Point3D(width, -width, height / 2) });

            meshBuilder.AddPolygon(new List<Point3D> { new Point3D(-width, width, -height / 2), new Point3D(-width, width, height / 2), new Point3D(-width + thickness, -width + thickness, height / 2), new Point3D(-width + thickness, -width + thickness, -height / 2) });
            meshBuilder.AddPolygon(new List<Point3D> { new Point3D(width, -width, height / 2), new Point3D(width, -width, -height / 2), new Point3D(-width + thickness, -width + thickness, -height / 2), new Point3D(-width + thickness, -width + thickness, height / 2) });

            meshBuilder.AddPolygon(new List<Point3D> { new Point3D(-width, width, -height / 2), new Point3D(-width, width, height / 2), new Point3D(-width + thickness, width, height / 2), new Point3D(-width + thickness, width, -height / 2) });
            meshBuilder.AddPolygon(new List<Point3D> { new Point3D(width, -width + thickness, -height / 2), new Point3D(width, -width + thickness, height / 2), new Point3D(width, -width, height / 2), new Point3D(width, -width, -height / 2) });



            MeshGeometry3D mesh = meshBuilder.ToMesh();
            GeometryModel3D model = new GeometryModel3D(mesh, material);
            return model;
        }
        private GeometryModel3D CreateCylinder(Material material, double radius = 5, double height = 300)
        {
            // 创建圆柱体
            helix.MeshBuilder meshBuilder = new helix.MeshBuilder();
            meshBuilder.AddCylinder(new Point3D(0, 0, -height / 2), new Point3D(0, 0, height / 2), radius);
            MeshGeometry3D mesh = meshBuilder.ToMesh();
            GeometryModel3D model = new GeometryModel3D(mesh, material);
            return model;
        }
        private Transform3DGroup Transform(double[] line,Boolean is_AngelIron = true)
        {
            Transform3DGroup group = new Transform3DGroup();
            if (is_AngelIron)
            {
                // 角钢自转旋转角
                Vector3D var3 = new Vector3D(1, 1, 0);
                Vector3D var4 = new Vector3D(-(line[3] + line[0]) / 2, -(line[4] + line[1]) / 2, 0);
                Vector3D axi2 = Vector3D.CrossProduct(var3, var4);
                double theta2 = Vector3D.AngleBetween(var3, var4);
                axi2 = axi2 == new Vector3D(0, 0, 0) ? new Vector3D(0, 0, 1) : axi2;
                theta2 += (line[5] - line[2]) == 0 ? 45 : 0;
                // 自转
                group.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(axi2, theta2)));
            }
            
            
            // 角钢移动到对于位置所需旋转角
            Vector3D var1 = new Vector3D(line[3] - line[0], line[4] - line[1], line[5] - line[2]);
            Vector3D var2 = new Vector3D((line[3] + line[0]) / 2, (line[4] + line[1]) / 2, (line[5] + line[2]) / 2);
            Vector3D axi1 = Vector3D.CrossProduct(new Vector3D(0, 0, 1), var1);
            double theta1 = Vector3D.AngleBetween(new Vector3D(0, 0, 1), var1);
            
            // 旋转
            group.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(axi1, theta1)));
            // 平移
            group.Children.Add(new TranslateTransform3D(var2));

            return group;
        }
    }
    
}
