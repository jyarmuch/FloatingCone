using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PitLib
{
    class FloatingCone
    {
        //public BlockModel bm;

        public FloatingCone(BlockModel bm1,int algorithm)
        {
            //bm = bm1;
            if (algorithm == 1)
            {
                Pana(ConePrecedences(bm1), bm1);
            }
            else if (algorithm == 2)
            {
                Lemieux(ConePrecedences(bm1), bm1);
            }
        }
        public double ObjVal(BlockModel bm1)
        {

            double objval = 0;
            for (int i = 0; i < bm1.nblocks; ++i)
            {
                objval = objval + bm1.BM[i].pit * bm1.BM[i].profit_max;
            }
            return objval;
        }

        public double ObjVal_Temp(BlockModel bm1)
        {

            double objval_tmp = 0;
            for (int i = 0; i < bm1.nblocks; ++i)
            {
                objval_tmp = objval_tmp + bm1.BM[i].pit_temp * bm1.BM[i].profit_max;
            }
            return objval_tmp;
        }

        public void Pana(Dictionary<int,string> _ConePrec,BlockModel bm1)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<block> Candidates = new List<block>();

            int sizeOfDict = _ConePrec.Count;
            
            foreach (var key in _ConePrec.Keys)
            {
                Candidates.Add(bm1.BM[key]);
            }
            
            List<block> SortedCandidates = Candidates.OrderByDescending(o => o.z).ThenByDescending(o => o.profit_max).ToList();

            int sizeOfList = SortedCandidates.Count;
            for (int i = 0; i < sizeOfList; ++i)
            {
                int label_tmp = SortedCandidates[i].label;
                double cone_value=0;
                string[] str = _ConePrec[label_tmp].Split(new[] { ','}, StringSplitOptions.RemoveEmptyEntries); // split the string by commas
                int sizeOfStrArray = str.Length;
                for (int j = 0; j < sizeOfStrArray; ++j)
                {
                    int index = Convert.ToInt32(str[j]);
                    cone_value = cone_value + bm1.BM[index].profit_max;
                }

                if (cone_value > 0)
                {
                    for (int j = 0; j < sizeOfStrArray; ++j)
                    {
                        int index = Convert.ToInt32(str[j]);
                        bm1.BM[index].pit = 1;
                    }
                }
            }
            stopwatch.Stop();
            Console.WriteLine("> A UPL with a value of {0} was found using the Floating Cone Algorithm (Pana) in {1} hh:mm:ss.", ObjVal(bm1), stopwatch.Elapsed);
        }

        public void Lemieux(Dictionary<int, string> _ConePrec, BlockModel bm1)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<block> Candidates = new List<block>();

            int sizeOfDict = _ConePrec.Count;

            foreach (var key in _ConePrec.Keys)
            {
                Candidates.Add(bm1.BM[key]);
            }

            double value_with_new_cone = 0;
            double value_without_new_cone = 0;
            List<block> SortedCandidates = Candidates.OrderByDescending(o => o.z).ThenByDescending(o => o.profit_max).ToList();

            int sizeOfList = SortedCandidates.Count;
            for (int i = 0; i < sizeOfList; ++i)
            {
                int label_tmp = SortedCandidates[i].label;
                double cone_value = 0;
                string[] str = _ConePrec[label_tmp].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries); // split the string by commas
                int sizeOfStrArray = str.Length;
                for (int j = 0; j < sizeOfStrArray; ++j)
                {
                    int index = Convert.ToInt32(str[j]);
                    cone_value = cone_value + bm1.BM[index].profit_max;
                }

                if (cone_value > 0)
                {
                    for (int j = 0; j < sizeOfStrArray; ++j)
                    {
                        int index = Convert.ToInt32(str[j]);
                        bm1.BM[index].pit_temp = 1;
                    }
                    value_with_new_cone = ObjVal_Temp(bm1);
                    value_without_new_cone = ObjVal(bm1);

                    if (value_with_new_cone > value_without_new_cone) //undo the addition of the new cone
                    {
                        for (int j = 0; j < sizeOfStrArray; ++j)
                        {
                            int index = Convert.ToInt32(str[j]);
                            bm1.BM[index].pit = 1;
                        }
                        for (int b = 0; b < bm1.nblocks; ++b)
                        {
                            bm1.BM[i].pit_temp = bm1.BM[i].pit;
                        }
                    }
                }




                //Console.WriteLine("> Value of cone {0} is {1}. Cum value is: {2}",label_tmp,cone_value, ObjVal(bm1));


            }
            stopwatch.Stop();
            Console.WriteLine("> A UPL with a value of {0} was found using the Floating Cone Algorithm (Lemieux) in {1} hh:mm:ss.", ObjVal(bm1), stopwatch.Elapsed);
        }

        public Dictionary<int,string> ConePrecedences(BlockModel bm1)
        {
            
            int idprec; //label for the precedence block
            int nr_arcs=0;//number of arcs
            double xb, yb, zb, xp, yp, zp, dh, rz; //xb,yb,zb coordinates of the block; xp,yp,zp coordinates of the block in the precedence; dh: horizontal distance; rz:cone radii.
            //string prec_filename = "cone.prec";    //write filename

            //List<string> Prec = new List<string>();
            Dictionary < int ,string > Prec= new Dictionary<int,string > ();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < bm1.nblocks; ++i) //loop over the whole block model
            {
                xb = bm1.BM[i].x; // x coordinate of block i
                yb = bm1.BM[i].y; // y coordinate of block i
                zb = bm1.BM[i].z; // z coordinate of block i
                string temp_str = Convert.ToString(i);
                if (bm1.BM[i].profit_max > 0) //if the block (vertex of the inverted cone) has positive value
                {
                    int levels = Convert.ToInt32((bm1.zmax-bm1.dz/2-zb) / bm1.dz);
                    //int levels = 5;
                    for (int incrz = 1; incrz <= levels; ++incrz)
                    {
                        for (int incry = -incrz; incry <= incrz; ++incry)
                        {
                            for (int incrx = -incrz; incrx <= incrz; ++incrx)
                            {
                                xp = xb + incrx * bm1.dx;
                                yp = yb + incry * bm1.dy;
                                zp = zb + incrz * bm1.dz;
                                dh = Math.Sqrt(Math.Pow((xp - xb), 2) + Math.Pow((yp - yb), 2));
                                rz = (zp - zb) / Math.Tan(bm1.BM[i].slope * Math.PI / 180);
                                
                                //Console.WriteLine(Convert.ToString(dh) +","+ Convert.ToString(rz));
                                if (dh <= rz)
                                {
                                    idprec = bm1.FindIndex(xp, yp, zp);
                                    //Console.WriteLine(Convert.ToString(idblock) + "," + Convert.ToString(idprec));
                                    if (idprec >= 0)
                                    {
                                        if (bm1.BM[idprec].mass > 0)
                                        {
                                            temp_str= temp_str + "," + Convert.ToString(idprec);
                                            ++nr_arcs;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    Prec.Add(i,temp_str);
                }
            }

            //File.WriteAllLines(@prec_filename, Prec);
            //Console.WriteLine("File '{0}' successfully written", prec_filename);
            stopwatch.Stop();
            Console.WriteLine("> A total of {0} arcs have been created in {1} hh:mm:ss.", nr_arcs, stopwatch.Elapsed);
            return Prec;
        }
    }
}
