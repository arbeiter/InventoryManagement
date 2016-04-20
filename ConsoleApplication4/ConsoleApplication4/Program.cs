using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Converters;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Text.RegularExpressions;

namespace ConsoleApplication4
{
    public class Program
    {

        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Enter 1 argument. It should be a json string");
            }

            Parser parser = new Parser();
            if (parser.IsValidJson(args[0]))
            {
                parser.PrettyPrint(args[0]);
            }
            else
            {
                Console.WriteLine("Invalid Json");
            }
        }
    }
}
