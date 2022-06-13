using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PitLib
{
    class BlockModel
    {
        /*
        This class create the block model collection of blocks
        */
        
        //DECLARATION OF VARIABLES
        //public string filename;
        public block[] BM; //list of blocks
        public int nx;
        public int ny;
        public int nz;
        public int nblocks; //number of blocks in the block model
        public double dx;   //dimension of blocks in x coordinate
        public double dy;   //dimension of blocks in y coordinate
        public double dz;   //dimension of blocks in z coordinate
        public double xmin; // mininum position in coordinate X of the block model (min(xcentre-dx/2))
        public double xmax; // maximum position in coordinate X of the block model (max(xcentre+dx/2))
        public double ymin; // mininum position in coordinate Y of the block model (min(ycentre-dy/2))
        public double ymax; // maximum position in coordinate Y of the block model (max(ycentre+dy/2))
        public double zmin; // mininum position in coordinate Z of the block model (min(zcentre-dz/2))
        public double zmax; // maximum position in coordinate Z of the block model (max(zcentre+dz/2))
        //CLASS CONSTRUCTORS

        // DEFAULT VALUES
        //        public BlockModel()
        //            {

        //            }

        public BlockModel(bm_parameters _bm_pars)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            xmin = _bm_pars.xmin;
            xmax = _bm_pars.xmax;
            ymin = _bm_pars.ymin;
            ymax = _bm_pars.ymax;
            zmin = _bm_pars.zmin;
            zmax = _bm_pars.zmax;
            dx = _bm_pars.dx;
            dy = _bm_pars.dy;
            dz = _bm_pars.dz;

            nx = Convert.ToInt32((xmax - xmin) / dx); //number of blocks in X
            ny = Convert.ToInt32((ymax - ymin) / dy); //number of blocks in Y
            nz = Convert.ToInt32((zmax - zmin) / dz); //number of blocks in Z
            
            nblocks = nx * ny * nz;
            BM = new block [nblocks];

            int id = 0;
            for (int iz = 0; iz < nz; ++iz)
            {
                for (int iy = 0; iy < ny; ++iy)
                {
                    for (int ix = 0; ix < nx; ++ix)
                    {
                        block block_temp = new block(_bm_pars.grade, _bm_pars.rocktype, _bm_pars.slope, _bm_pars.mass, _bm_pars.profit_waste, _bm_pars.profit_process);
                        block_temp.x = xmin + dx / 2.0 + ix * dx;
                        block_temp.y = ymin + dy / 2.0 + iy * dy;
                        block_temp.z = zmin + dz / 2.0 + iz * dz;
                        block_temp.label = id;
                        BM[id] = block_temp;
                        ++id;
                    }
                }
            }
            stopwatch.Stop();
            Console.WriteLine("> A block model of  {0} blocks ({1}) has been created in {2} hh:mm:ss.", nblocks, nx + "x" + ny + "x" + nz, stopwatch.Elapsed);                      
        }

        
        
        public int FindIndex(double _xi, double _yi, double _zi)
        {
            /*
            Function description: FindIndex finds the label of each block based on the x,y,z coordinates.
            input: x,y,z (doubles).
            output:label (int).
             */
            int label;
            double ix = (_xi - (xmin + dx/2.0))/ dx  + 1;
            int idx= Convert.ToInt32(Math.Ceiling(ix));
            double iy = (_yi - (ymin + dy/2.0)) /dy  + 1;
            int idy = Convert.ToInt32(Math.Ceiling(iy));
            double iz = (_zi - (zmin + dz/2.0)) /dz + 1;
            int idz = Convert.ToInt32(Math.Ceiling(iz));
            //Console.WriteLine("idx: {0}",idx);
            //Console.WriteLine(ratio_x);
            label =(idx - 1) + (idy - 1) * nx + (idz - 1) * nx * ny;
            if ((label >= nblocks) || (label < 0))
            {
                label = -99;
            }
            return label;
        }

        public void Import(string _filename)
        {
            /*This function imports a block model from a text file. its allocate each value of the block model into a cell of an instance of the Block Model object
             The map array provides a mapping between the row in the text file and the location of the Block Model object, this is only for debuging purpose.
             */
            int i, id,rocktype,slope;
            double d, x, y, z,grade, mass, profit_waste, profit_process;
            string import_filename = _filename;
            string[] Map = new string[nblocks];
            bool[] Flag = new bool[nblocks];
            i = 0;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            foreach (var line in System.IO.File.ReadLines(@import_filename))//read every line of the text file
            {

                string[] str = line.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries); // split the line by space or commas

                if (Double.TryParse(str[0], out d) & (Double.TryParse(str[1], out d)) & (Double.TryParse(str[2], out d)) & (Double.TryParse(str[3], out d))) // if done, then is a number
                {
                    x = Convert.ToDouble(str[0]);
                    y = Convert.ToDouble(str[1]);
                    z = Convert.ToDouble(str[2]);
                    grade= Convert.ToDouble(str[3]);
                    rocktype = Convert.ToInt32(str[4]);
                    slope = Convert.ToInt32(str[5]);
                    mass = Convert.ToDouble(str[6]);
                    profit_waste = Convert.ToDouble(str[7]);
                    profit_process= Convert.ToDouble(str[8]);

                    id = FindIndex(x, y, z);
                    if (id >= 0)
                    {
                        if ((x < BM[id].x + dx / 2.0) & (x > BM[id].x- dx / 2.0) & (y < BM[id].y + dy / 2.0) & (y > BM[id].y - dy / 2.0) & (z < BM[id].z + dz / 2.0) & (z > BM[id].z - dz / 2.0))
                        {
                            if (!Flag[id]) // this flag is created to identify the first element of the line
                            {
                                Map[id] = Convert.ToString(i);
                                Flag[id] = true;
                            }
                            else
                            {
                                Map[id] = Map[id] + "," + Convert.ToString(i);
                            }

                            BM[id].grade = grade;
                            BM[id].rocktype = rocktype;
                            BM[id].slope = slope;
                            BM[id].mass = mass;
                            BM[id].profit_waste = profit_waste;
                            BM[id].profit_process = profit_process;
                            BM[id].profit_max = Math.Max(profit_waste,profit_process);
                        }

                        ++i;
                    }
                }
                else
                {
                    Console.WriteLine("> Line {0} has no numerical fields: {1}",i,line);
                }


                //
                //Console.WriteLine("X: {0}", str[0]);
                //Console.WriteLine("Y: {0}", str[1]);
                //Console.WriteLine("Z: {0}", str[2]);
            }
            //Console.WriteLine("X: [{0}]", string.Join(", ", X));
            stopwatch.Stop();
            Console.WriteLine("> A number of {0} lines from the file {1} have been successfully imported in {2} hh:mm:ss.", i,import_filename, stopwatch.Elapsed);
            //string map_filename = _filename + ".maping";
            //File.WriteAllLines(@map_filename, Map);
            //Console.WriteLine("File '{0}' successfully written", map_filename);
        }
        public void Export(string _filename, bool _All)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            string export_filename = _filename;

            if (_All)
            {
                string[] textfile = new string[nblocks + 1];
                textfile[0] = "X,Y,Z,Grade,Rocktype,Slope,Mass,Profit_Waste,Profit_Process,Profit_Max,Pit,Pushback";
                for (int id = 0; id < nblocks; ++id)
                {
                    textfile[id + 1] = Convert.ToString(BM[id].x) + "," + Convert.ToString(BM[id].y) + "," + Convert.ToString(BM[id].z) + "," + Convert.ToString(BM[id].grade) + "," + Convert.ToString(BM[id].rocktype) + "," + Convert.ToString(BM[id].slope) + "," + Convert.ToString(BM[id].mass) + "," + Convert.ToString(BM[id].profit_waste) + "," + Convert.ToString(BM[id].profit_process) + ","+ Convert.ToString(BM[id].profit_max) + "," + Convert.ToString(BM[id].pit) + "," + Convert.ToString(BM[id].pushback);
                }

                File.WriteAllLines(@export_filename, textfile);

            }
            else
            {
                List<string> Txt = new List<string>();
                Txt.Add("X,Y,Z,Grade,Rocktype,Slope,Mass,Profit_Waste,Profit_Process,Profit_Max,Pit,Pushback");
                for (int id = 0; id < nblocks; ++id)
                {
                    if (BM[id].pit > 0)
                    {
                        Txt.Add(Convert.ToString(BM[id].x) + "," + Convert.ToString(BM[id].y) + "," + Convert.ToString(BM[id].z) + "," + Convert.ToString(BM[id].grade) + "," + Convert.ToString(BM[id].rocktype) + "," + Convert.ToString(BM[id].slope) + "," + Convert.ToString(BM[id].mass) + "," + Convert.ToString(BM[id].profit_waste) + "," + Convert.ToString(BM[id].profit_process) + "," + Convert.ToString(BM[id].profit_max) + "," + Convert.ToString(BM[id].pit) + "," + Convert.ToString(BM[id].pushback));
                    }

                }
                File.WriteAllLines(@export_filename, Txt);
            }
            stopwatch.Stop();
            Console.WriteLine("> File '{0}' successfully created in {1} hh:mm:ss.", export_filename, stopwatch.Elapsed);
        }

        public Graph Precedences()
        {
            Graph graph = new Graph();
            int idprec; //label for the precedence block
            int nr_arcs = 0;//number of arcs
            double xb, yb, zb, xp, yp, zp, dh, rz; //xb,yb,zb coordinates of the block; xp,yp,zp coordinates of the block in the precedence; dh: horizontal distance; rz:cone radii.

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < nblocks; ++i) //loop over the whole block model
            {
                xb = BM[i].x; // x coordinate of block i
                yb = BM[i].y; // y coordinate of block i
                zb = BM[i].z; // z coordinate of block i
                if (BM[i].mass > 0) //if the block has positive tonnage
                {
                    var ni = new Vertex(BM[i].label+2, BM[i].profit_max);
                    graph.V.Add(ni);
                    int levels = 3; //number of levels to create the slope constraints
                    for (int incrz = 1; incrz <= levels; ++incrz)
                    {
                        for (int incry = -incrz; incry <= incrz; ++incry)
                        {
                            for (int incrx = -incrz; incrx <= incrz; ++incrx)
                            {
                                xp = xb + incrx * dx;
                                yp = yb + incry * dy;
                                zp = zb + incrz * dz;
                                dh = Math.Sqrt(Math.Pow((xp - xb), 2) + Math.Pow((yp - yb), 2));
                                rz = (zp - zb) / Math.Tan(BM[i].slope * Math.PI / 180);

                                //Console.WriteLine(Convert.ToString(dh) +","+ Convert.ToString(rz));
                                if (dh <= rz)
                                {
                                    idprec = FindIndex(xp, yp, zp);
                                    //Console.WriteLine(Convert.ToString(idblock) + "," + Convert.ToString(idprec));
                                    if (idprec >= 0)
                                    {
                                        if (BM[idprec].mass > 0)
                                        {
                                            var nj = new Vertex(BM[idprec].label+2, BM[idprec].profit_max);
                                            graph.A.Add(new Arc(ni,nj));
                                            ++nr_arcs;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    
                }
            }

            //File.WriteAllLines(@prec_filename, Prec);
            //Console.WriteLine("File '{0}' successfully written", prec_filename);
            stopwatch.Stop();
            Console.WriteLine("> A total of {0} arcs have been created in {1} hh:mm:ss.", nr_arcs, stopwatch.Elapsed);
            return graph;
        }
    }

}
