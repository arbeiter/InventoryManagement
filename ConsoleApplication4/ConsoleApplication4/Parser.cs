using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApplication4
{
    public class Parser
    {
        public string PrettyPrint()
        {
            return String.Empty;
        }

        public Dictionary<string, string> GetLongCDs(string Source)
        {
            var model = new
            {
                Method = "Expando Object",
                Source,
                Result = JsonConvert.DeserializeObject<ExpandoObject[]>(Source),
                Query = new StrongBox<IEnumerable>()
            };

            model.Query.Value = from dynamic item in model.Result
                                where this.IsKeyValid(item, "type") && item.type.ToString().Equals("cd")
                                select item;

            Dictionary<string, string> cds = new Dictionary<string, string>();
            foreach (dynamic cd in model.Query.Value)
            {
                //Check whether cd contains tracks
                if (this.IsKeyValid(cd, "tracks")==false)
                {
                    continue;
                }

                if (cd.tracks != null)
                {
                    int total = 0;
                    foreach (dynamic track in cd.tracks)
                    {
                        if (track.seconds != null)
                        {
                            total += track.seconds;
                        }
                    }

                    if (total > (60 * 60))
                    {
                        cds.Add(cd.title, total.ToString());
                    }
                }
            }

            return cds;
        }

        public List<string> GetAuthorsWithCds(string Source)
        {
            var model = new
            {
                Method = "Expando Object",
                Source,
                Result = JsonConvert.DeserializeObject<ExpandoObject[]>(Source),
                Query = new StrongBox<IEnumerable>()
            };

            model.Query.Value = from dynamic item in model.Result
                                where this.IsKeyValid(item, "type") && item.type.ToString().Equals("cd")
                                select item;

            var res = model.Query.Value;

            HashSet<string> val = new HashSet<string>();
            foreach (dynamic value in res)
            {
                if (value.author != null)
                {
                    val.Add(value.author.ToString());
                }
            }

            return val.ToList();
        }

        public Dictionary<string, List<string>> GetTopResults(string Source)
        {
            //This is a little more convoluted than it should be thanks to my decision to use Expando Objects
            var model = new
            {
                Method = "Expando Object",
                Source,
                Result = JsonConvert.DeserializeObject<ExpandoObject[]>(Source),
                Query = new StrongBox<IEnumerable>()
            };

            //Step 1 : Map query to anonymous type of form : Key, List of Items
            model.Query.Value = (from dynamic item in model.Result
                                 where this.IsKeyValid(item, "type")
                                 group item by item.type into g
                                 select new { ItemType = g.Key, Items = g.ToList() });

            //Step 2 : Construct a strongly typed dictionary with Key of type string and Items of type List<String>
            Dictionary<string, List<string>> outputDict = new Dictionary<string, List<string>>();
            foreach (dynamic item in model.Query.Value)
            {
                var out1 = item.ItemType;
                if (out1 == null)
                {
                    continue;
                }

                List<String> prices = new List<string>();
                foreach (dynamic boxer in item.Items)
                {
                    if (this.IsKeyValid(boxer, "price"))
                    {
                        prices.Add(boxer.price.ToString());
                    }
                }

                outputDict.Add(out1.ToString(), prices);
            }

            //Step 3 : Perform a  LINQ query on the output dictionary's values and pick the top 5 elements from them
            // The result is a transformed dictionary
            // You can't do it in the same loop because C# doesn't allow you to modify a collection while you loop over it.
            List<string> keys = new List<string>(outputDict.Keys);
            foreach (var input in keys)
            {
                var result = outputDict[input].OrderByDescending(x => Double.Parse(x)).Take(5).ToList();
                outputDict[input] = result;
            }

            return outputDict;
        }

        public List<String> GetItemsWithYearName(string Source)
        {
            var model = new
            {
                Method = "Expando Object",
                Source,
                Result = JsonConvert.DeserializeObject<ExpandoObject[]>(Source),
                Query = new StrongBox<IEnumerable>()
            };

            model.Query.Value = from dynamic item in model.Result
                                where this.IsKeyValid(item, "type")
                                select item;

            var res = model.Query.Value;

            HashSet<String> output = new HashSet<string>();
            foreach (dynamic val in res)
            {
                //Cast expando to object
                object x = (object)val;

                //Title
                if (this.IsKeyValid(val, "title"))
                {
                    if (val.title != null && this.IsYear(val.title.ToString()))
                    {
                        output.Add(val.title);
                    }
                }

                //Track
                if (this.IsKeyValid(val, "tracks"))
                {
                    if (val.tracks != null)
                    {
                        foreach (dynamic track in val.tracks)
                        {
                            if (this.IsYear(track.name.ToString()))
                            {
                                output.Add(val.title);
                            }
                        }
                    }
                }

                if (this.IsKeyValid(val, "chapters"))
                {
                    //Chapter
                    if (val.chapters != null)
                    {
                        foreach (dynamic chapter in val.chapters)
                        {
                            if (this.IsYear(chapter.ToString()))
                            {
                                output.Add(val.title);
                            }
                        }
                    }
                }
            }

            return output.ToList();
        }

        public bool IsValidJson(string Source)
        {
            Source = Source.Trim();

            if ((Source.StartsWith("{") && Source.EndsWith("}")) || //For object
                (Source.StartsWith("[") && Source.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(Source);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private bool IsYear(string input)
        {
            if (String.IsNullOrEmpty(input))
            {
                return false;
            }

            bool containsYears = false;

            string[] numbers = Regex.Split(input, @"\D+");
            foreach(var number in numbers)
            {

                if (!string.IsNullOrEmpty(number))
                {
                    int i = int.Parse(number);
                    //Todo: Use gregorian calendar max. The whole date time validation thing.
                    if (i>1 && i < 3000)
                    {
                        containsYears = containsYears || true;
                    }
                }
            }

            return containsYears;
        }

        private bool IsKeyValid(ExpandoObject obj, string member)
        {
            if (((IDictionary<String, object>)obj).ContainsKey(member))
            {
                return true;
            }
            return false;
        }
    }
}
