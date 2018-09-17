using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

//this one!!
namespace week_12_c_sharp
{

    class Buggy
    {

        // members/attributes
        public string message_from_buggy = "";
        public string previous_message_from_buggy = "";
        public int id;
        private int laps_complete;
        private int laps_to_complete;
        private int gantry_1_counter;
        private int gantry_2_counter;
        private int gantry_3_counter;
        private int buggy_direction;

        // constructors
        // default constructor
        public Buggy()
        {
            setBuggyID(0);
            setLapsCompleted(0);
            setLapsToComplete(0);
            setGantry1Counter(0);
            setGantry2Counter(0);
            setGantry3Counter(0);
            setBuggyDirection(0);
        }

        // second constructor
        public Buggy(int id) { setBuggyID(id); }

        // methods
        //***********************
        // accessors and mutators
        public int getBuggyID() { return id; }
        public void setBuggyID(int desired_id) { id = desired_id; }
        public int getLapsCompleted() { return laps_complete; }
        public void setLapsCompleted(int new_lap_complete) { laps_complete = new_lap_complete; }
        public int getLapsToComplete() { return laps_to_complete; }
        public void setLapsToComplete(int desired_laps) { laps_to_complete = desired_laps; }
        public int getGantry1Counter() { return gantry_1_counter; }
        public int getGantry2Counter() { return gantry_2_counter; }
        public int getGantry3Counter() { return gantry_3_counter; }
        public void setGantry1Counter(int g1) { gantry_1_counter = g1; }
        public void setGantry2Counter(int g2) { gantry_2_counter = g2; }
        public void setGantry3Counter(int g3) { gantry_3_counter = g3; }
        public int getBuggyDirection() { return buggy_direction; }
        public void setBuggyDirection(int direction) { buggy_direction = direction; }

        // method to convert incoming string to character array and invoke relevent methods accordingly
        public void messageFromBuggyHandler(char[] char_array)
        {
            directionDetermination();
            // numerical characters for returned values from buggy
            // capital letters for returned  values from manual entry for debugging
            if (char_array[1] == '1' || char_array[1] == '2' || char_array[1] == '3' ||
                char_array[1] == 'A' || char_array[1] == 'B' || char_array[1] == 'C')
            {
                gantryControl(char_array[1]);
                buggyLapControl();
                parkCommand(char_array[1]);
            }
            else if (char_array[0] == '2')
            {
                if (char_array[1] == 'L' && laps_to_complete + 1 != getLapsCompleted())
                {
                    Program.port.WriteLine("1m");
                }
            }
        }

        // buggy gantry counter control method
        public void gantryControl(char character)
        {
            if (character == '1' || character == 'A')
            {
                setGantry1Counter(getGantry1Counter() + 1);
            }
            else if (character == '2' || character == 'B')
            {
                setGantry2Counter(getGantry2Counter() + 1);
            }
            else if (character == '3' || character == 'C')
            {
                setGantry3Counter(getGantry3Counter() + 1);
                Program.port.WriteLine("1s");
                Thread.Sleep(50);
                Program.port.WriteLine("2m");
            }
            buggyLapControl();
            buggyStatsPrint();
        }

        // buggy stats print
        public void buggyStatsPrint()
        {
            Console.WriteLine("\n     ____________________________");
            Console.WriteLine("    |          Buggy_" + getBuggyID() + "           |");
            Console.WriteLine("    |============================|");
            Console.WriteLine("    |     Laps Completed = " + getLapsCompleted() + "     |");
            Console.WriteLine("    |----------------------------|");
            Console.WriteLine("    |     Gantry_1 count = " + getGantry1Counter() + "     |");
            Console.WriteLine("    |----------------------------|");
            Console.WriteLine("    |     Gantry_2 count = " + getGantry2Counter() + "     |");
            Console.WriteLine("    |----------------------------|");
            Console.WriteLine("    |     Gantry_3 count = " + getGantry3Counter() + "     |");
            Console.WriteLine("    |____________________________|");
            Console.WriteLine("\n");
        }

        // buggy park command issue method
        public void parkCommand(char character)
        {
            if (getBuggyDirection() == 1)
            {
                if ((getLapsCompleted() == getLapsToComplete()) &&
                    (getGantry2Counter() == (getLapsToComplete() + 1)))
                {
                    Program.port.WriteLine("1r");
                    Console.WriteLine("Command Issued -> Buggy_" + getBuggyID() + " Park Right.");
                }
            }
            if (getBuggyDirection() == 2)
            {
                if (character == 'A')
                {
                    Program.port.WriteLine("2l");
                    Console.WriteLine("Command Issued -> Buggy_" + getBuggyID() + " -> Park Left.");
                }
            }
        }

        // buggy direction determination method
        public void directionDetermination()
        {
            if (getBuggyDirection() == 0)
            {
                if (getGantry1Counter() > getGantry2Counter())
                {
                    setBuggyDirection(1);
                    Console.WriteLine("\n\n    Buggy_" + getBuggyID() + " direction determined as clockwise!\n\n");
                }
            }
            if (getBuggyDirection() == 0)
            {
                if (getGantry2Counter() > getGantry1Counter())
                {
                    setBuggyDirection(2);
                    Console.WriteLine("\n\n    Buggy_" + getBuggyID() + " direction determined as counter-clockwise!\n\n");
                }
            }
        }

        // buggy lap control method
        public void buggyLapControl()
        {
            if (getBuggyDirection() == 1)
            {
                if ((getGantry1Counter() != getLapsCompleted()) &&
                    (getGantry1Counter() == getGantry2Counter()) &&
                    (getGantry2Counter() == getGantry3Counter()))
                {
                    setLapsCompleted(getLapsCompleted() + 1);
                    Console.WriteLine("\nBuggy_" + getBuggyID() + " Lap " + getLapsCompleted() + " complete.");
                }
            }
            if (getBuggyDirection() == 2)
            {
                if ((getGantry2Counter() != getLapsCompleted()) &&
                    (getGantry2Counter() == getGantry1Counter()))
                {
                    setLapsCompleted(getLapsCompleted() + 1);
                    Console.WriteLine("\nBuggy_" + getBuggyID() + " Lap " + getLapsCompleted() + " complete.");
                }
            }
        }
    }
}
