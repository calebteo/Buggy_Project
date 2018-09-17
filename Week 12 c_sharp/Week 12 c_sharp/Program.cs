using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using System.Reflection;

namespace week_12_c_sharp
{
    class Program
    {

        public static string response_from_buggy = " ";
        public static string previous_message = "";
        public static string input_string = "";
        public static SerialPort port = new SerialPort("COM5", 9600);
        public static bool assigned_confirmed = false;
        static Buggy b1 = new Buggy(0);
        static Buggy b2 = new Buggy(0);
        public static char[] char_array = new char[2];
        static void Main(string[] args)
        {


            // initial setup & asking user to enter the number of buggys and laps to complete
            Console.WriteLine("GROUP W2");
            Console.WriteLine("\nBuggy Project Start ...");

            // code to initiate serial communication
            // create new serial port object & set comport and baud rate

            // opening serial port
            port.Open();
            Console.Write("\nCom-Port Open ... ");
            Thread.Sleep(5000);
            Console.WriteLine("configuring serial XBEE ...");
            port.Write("+++");
            Console.Write("Entering AT mode ... please wait ...");
            Thread.Sleep(1000);
            port.WriteLine("ATID 1234, CH C, CN");
            Console.WriteLine("\n\nConfiguring XBEE: \n\nATID: 1234 \n  CH: C \n\n... please wait ...");
            Thread.Sleep(1500);

            port.DataReceived += new SerialDataReceivedEventHandler(OnDataReceived);

            Console.WriteLine("\n... Serial XBEE configuring ... \n\n... please wait 10 seconds ...\n ");
            Thread.Sleep(10000);

            Console.WriteLine("\nPlease wait ... ");
            Console.WriteLine("            ... READY!");
            Console.WriteLine("\nHow many buggys will be on the track?");
            Console.WriteLine("Please enter 1 or 2.");
            string num_buggies = "";
            num_buggies = Console.ReadLine();
            int int_num_buggies = Convert.ToInt32(num_buggies);

            while (int_num_buggies != 1 && int_num_buggies != 2)
            {
                Console.WriteLine("Please enter ONLY 1 or 2.");
                num_buggies = Console.ReadLine();
                int_num_buggies = Convert.ToInt32(num_buggies);
            }
            Console.WriteLine("\nPlease enter the number of laps you would like to complete before parking.");
            string num_laps = "";
            num_laps = Console.ReadLine();
            int int_num_laps = Convert.ToInt32(num_laps);
            //Console.WriteLine("NUM LAPS = " + int_num_laps);
            if (int_num_buggies == 1)
            {
                b1.setBuggyID(1);
                b1.setLapsToComplete(int_num_laps);
                /*Console.WriteLine("Assigning Buggy 1");       //For dynamically assigning. 
                port5.WriteLine("You are B1");
                Console.WriteLine("Waiting for Confirmation...");
                while (!assigned_confirmed) {
                    
                }
                Console.WriteLine("Confirmed");
                */

            }
            else
            {
                b1.setBuggyID(1);
                b1.setLapsToComplete(int_num_laps);
                b2.setBuggyID(2);
                b2.setLapsToComplete(int_num_laps);

                /*Console.WriteLine("Assigning Buggy 1");       //For dynamically assigning. 
                port5.WriteLine("You are B1");
                Console.WriteLine("Waiting for Confirmation...");
                while (!assigned_confirmed) {

                }
                Console.WriteLine("Confirmed");
                */
            }


            Console.WriteLine("GOLD Challenge will start in: ");
            for (int i = 3; i > 0; i--)
            {
                Console.WriteLine("\r" + i);
                Thread.Sleep(1000);
            }
            Console.WriteLine("\nCommand Issued: Buggy_" + b1.getBuggyID() + " -> Line Follow.");
            port.WriteLine("1m");
            
            /*while (true)
            {

                input_string = Console.ReadLine();
                port.WriteLine(input_string);
            }*/

        }
        // This is the event handler 
        public static void OnDataReceived(object sender,
            SerialDataReceivedEventArgs e) {
            // throw new NotImplementedException();
            SerialPort port = (SerialPort)sender;
            string incoming_data = port.ReadLine();
            response_from_buggy = incoming_data;
            if (previous_message != response_from_buggy)
            {
                Console.WriteLine("Message Received <- " + response_from_buggy + "\n");
                previous_message = response_from_buggy;
                char_array = response_from_buggy.ToCharArray(0, 2);
                if (char_array[0] == '1')
                {
                    b1.messageFromBuggyHandler(char_array);
                    char_array[0] = '0';
                    char_array[1] = '0';
                }
                else if (char_array[0] == '2')
                {
                    b2.messageFromBuggyHandler(char_array);
                    char_array[0] = '0';
                    char_array[1] = '0';
                }

            }
        }
    }
}
