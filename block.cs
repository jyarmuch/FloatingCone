using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PitLib
{
    public class block
    {
        //attributes of the object block
        public double x, y, z, grade, mass,profit_waste,profit_process, profit_max; //possition in x,y,z; ton: tonnage of the block; profit: economic value of the block
        public int rocktype, slope, label, pit, pit_temp,pushback;//label: numeric label of the block; slope: geotecnical constraint for the excavation; pit: the number of the pit (nested pits); pushback: the number of mining pushback.

        //default values of the object block
        public block(double _grade, int _rocktype, int _slope, double _mass, double _profit_waste, double _profit_process )
        {
            //default values from the block model construction
            grade = _grade;
            rocktype = _rocktype;
            slope = _slope;
            mass = _mass;
            profit_waste = _profit_waste;
            profit_process = _profit_process;

            //default values for the optimisation process
            profit_max = Math.Max(profit_waste,profit_process);
            pit = 0;        // by default the block don't belong to the ultimate pit limit
            pit_temp = 0;        // by default the block don't belong to the ultimate pit limit
            pushback = 0;   // by default the block don't belong to any pushback or phase
        }
        
    }
}
