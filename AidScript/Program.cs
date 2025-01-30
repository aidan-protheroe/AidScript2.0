namespace AidScript
{
    internal class Program
    {
        static void Main()
        {
            var running = true;
            while (running)
            {
                var command = Console.ReadLine();
                if (command == "exit")
                    break;
                if (command.Contains(".aid"))
                {
                    var rawInput = FileReader.ReadFile(command);
                    var interpreter = new Interpreter();
                    interpreter.Run(rawInput);
                }
                else
                {
                    var interpreter = new Interpreter();
                    interpreter.Run(command); //make it work like python in the cmd
                }
            }
        }
    }
}

//if you add booleans things will become simpler
//need a way to get user input
//add random

//GONNA HAVE TO ADD ORDER OF OPERATIONS EVENTUALLY


//CLEAN UP EVERYTHING! MAKE FIELDS AND METHODS THAT CAN BE PRIVATE BE PRIVATE! NAMES ARE ALL OVER THE PLACE
