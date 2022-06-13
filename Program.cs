using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PitLib
{
    class bm_parameters
    {
        public double xmin,xmax, ymin,ymax, zmin,zmax,dx,dy,dz,grade, profit_waste, profit_process,mass;
        public int rocktype, slope; 
        public bm_parameters()
            {
            xmax = 0;
            ymin = 0;
            ymax = 0;
            zmin = 0;
            zmax = 0;
            dx = 0;
            dy = 0;
            dz = 0;
            grade = 0;
            profit_waste = 0;
            profit_process = 0;
            mass = 0;
            rocktype = 0;
            slope = 0;
        }
    }

    class Program
    {
        static Graph graph = new Graph();

        static void Main(string[] args)
        {
            /*double xmin = 0;
            double xmax = 0;
            double ymin = 0;
            double ymax = 0;
            double zmin = 0;
            double zmax = 0;
            int dx = 0;
            int dy = 0;
            int dz=  0;
            
            double grade = 0;
            
            int rocktype = 0;
            int slope = 0;
            double mass = 0;
            double profit_waste = 0;
            double profit_process = 0;
            */

            //optimisation parameters
            double rf_min = 0;
            double rf_max = 0;
            double rf_i = 0;
            string imp_filename="";
            string exp_filename = "";
            int opt_alg = 0;

            bm_parameters bm_pars=new bm_parameters();

            foreach (var line in System.IO.File.ReadLines(@"pitlib_par.txt"))//read every line of the text file
            {
                string[] str = line.Split(new[] { ':', ' ' }, StringSplitOptions.RemoveEmptyEntries); // split the line by space or commas
                if (!str[0].Contains('#'))
                {
                    switch (str[0])
                    {
                        case "import_filename":
                            imp_filename = str[1];
                            break;
                        case "export_filename":
                            exp_filename = str[1];
                            break;
                        case "xmin":
                            bm_pars.xmin= Convert.ToDouble(str[1]);
                            break;
                        case "xmax":
                            bm_pars.xmax = Convert.ToDouble(str[1]);
                            break;
                        case "ymin":
                            bm_pars.ymin = Convert.ToDouble(str[1]);
                            break;
                        case "ymax":
                            bm_pars.ymax = Convert.ToDouble(str[1]);
                            break;
                        case "zmin":
                            bm_pars.zmin = Convert.ToDouble(str[1]);
                            break;
                        case "zmax":
                            bm_pars.zmax = Convert.ToDouble(str[1]);
                            break;
                        case "dx":
                            bm_pars.dx = Convert.ToInt32(str[1]);
                            break;
                        case "dy":
                            bm_pars.dy = Convert.ToInt32(str[1]);
                            break;
                        case "dz":
                            bm_pars.dz = Convert.ToInt32(str[1]);
                            break;
                        case "grade":
                            bm_pars.grade = Convert.ToDouble(str[1]);
                            break;
                        case "rocktype":
                            bm_pars.rocktype = Convert.ToInt32(str[1]);
                            break;
                        case "slope":
                            bm_pars.slope = Convert.ToInt32(str[1]);
                            break;
                        case "mass":
                            bm_pars.mass = Convert.ToDouble(str[1]);
                            break;
                        case "profit_waste":
                            bm_pars.profit_waste = Convert.ToDouble(str[1]);
                            break;
                        case "profit_process":
                            bm_pars.profit_process = Convert.ToDouble(str[1]);
                            break;
                        case "rf_min":
                            rf_min = Convert.ToDouble(str[1]);
                            break;
                        case "rf_max":
                            rf_max = Convert.ToDouble(str[1]);
                            break;
                        case "rf_i":
                            rf_i = Convert.ToDouble(str[1]);
                            break;
                        case "opt_alg":
                            opt_alg = Convert.ToInt32(str[1]);
                            break;
                    }
                }
            }   


            Console.WriteLine("> ***************************************************************************************");
            Console.WriteLine("> **                          Welcome to the PitLib v0                                 **");
            Console.WriteLine("> **-----------------------------------------------------------------------------------**");
            Console.WriteLine("> ** Copyright © 2017 Juan L. Yarmuch. All rights reserved. Permission to use, copy,   **");
            Console.WriteLine("> ** and distribute this software and its documentation for educational, research,     **");
            Console.WriteLine("> ** and not-for-profit purposes, without fee and without a signed licensing agreement,**");
            Console.WriteLine("> ** is hereby granted.                                                                **");
            Console.WriteLine("> ***************************************************************************************");
            Console.WriteLine("> Please check if the file 'pitlib_par.txt' is in the current folder");
            Console.WriteLine("> Press enter to continue...");
            Console.ReadLine();
            BlockModel bm = new BlockModel(bm_pars);
            bm.Import(imp_filename);
            
            FloatingCone fc = new FloatingCone(bm,opt_alg);
            bm.Export(exp_filename,false);
            Console.ReadLine();
            /*
            
            var n1 = new Vertex(0, -10);
            var n2 = new Vertex(1, -10);
            var n3 = new Vertex(2, -10);
            var n4 = new Vertex(3, -10);
            var n5 = new Vertex(4, -10);
            var n6 = new Vertex(5, -10);
            var n7 = new Vertex(6, -10);
            var n8 = new Vertex(7, -10);
            var n9 = new Vertex(8, -10);
            var n10 = new Vertex(9, 100.0154545456465464212);
            var n11 = new Vertex(10, -10);
            graph.V.Add(n1);
            graph.V.Add(n2);
            graph.V.Add(n3);
            graph.V.Add(n4);
            graph.V.Add(n5);
            graph.V.Add(n6);
            graph.V.Add(n7);
            graph.V.Add(n8);
            graph.V.Add(n9);
            graph.V.Add(n10);
            graph.V.Add(n11);
            graph.A.Add(new Arc(n8, n1)); 
            graph.A.Add(new Arc(n8, n2)); 
            graph.A.Add(new Arc(n8, n3)); 
            graph.A.Add(new Arc(n9, n2)); 
            graph.A.Add(new Arc(n9, n3)); 
            graph.A.Add(new Arc(n9, n4)); 
            graph.A.Add(new Arc(n10, n3));
            graph.A.Add(new Arc(n10, n4));
            graph.A.Add(new Arc(n10, n5));
            graph.A.Add(new Arc(n11, n4)); 
            graph.A.Add(new Arc(n11, n5)); 
            graph.A.Add(new Arc(n11, n6)); 
            //graph=bm.Precedences();
            Pseudoflow pf = new Pseudoflow(graph);
            //bm.Export(exp_filename,false);
            Console.ReadLine();
            */
        }
    }
}
