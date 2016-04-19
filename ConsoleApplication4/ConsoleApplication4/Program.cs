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
        const string Source = @"[{""price"":15.99,""chapters"":[""one"",""two"",""three""],""year"":1999,""title"":""foo"",""author"":""mary"",""type"":""book""},{""price"":11.99,""minutes"":90,""year"":2004,""title"":""bar"",""director"":""alan"",""type"":""dvd""},{""price"":15.99,""tracks"":[{""seconds"":180,""name"":""one""},{""seconds"":200,""name"":""two""}],""year"":2000,""title"":""baz"",""author"":""joan"",""type"":""cd""}]";

        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Enter 1 argument. It should be a json string");
            }

            Parser parser = new Parser();
            if (parser.IsValidJson(args[0]))
            {
                parser.GetAuthorsWithCds(args[0]);
            }

            parser.PrettyPrint();
        }
    }
}
