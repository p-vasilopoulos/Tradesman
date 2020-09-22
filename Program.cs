using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json.Converters;


namespace Tradesman
{

    public class Request
    {
        [JsonProperty("query")]
        Query query = new Query();

        [JsonProperty("sort")]
        Sort sort = new Sort();

        public Request(string option, string name, string type, List<Stat> stats,string sorting)
        {
            query = new Query(option,name,type,stats);
            sort = new Sort(sorting);
        }
        

    }

    public class Query
    {
        [JsonProperty("status")]
        public Status status;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("type")]
        public string type;
        [JsonProperty("stats")]
        public List<Stat> stats;

        public Query()
        {

        }
        public Query(string option, string name, string type, List<Stat> stats)
        {
            status = new Status(option);
            this.name = name;
            this.type = type;
            this.stats = stats;
        }


    }
    public class Status
    {
        [JsonProperty("option")]
        public string option { get; set; } // online or offline

        public Status(string option)
        {
            this.option = option;
        }
    }

    public class Stat
    {
        [JsonProperty("type")]
        public string type { get; set; } // and
        [JsonProperty("filters")]
        public List<Filter> filters { get; set; }

        public Stat(string type)
        {
            this.type = type;
            filters = new List<Filter>();


        }

        public Stat(string type, List<Filter> filters)
        {
            this.type = type;
            this.filters = filters;
        }

    }

    public class Filter
    {
    }

    public class Sort
    {
        [JsonProperty("price")]
        public string sorting; // asc or desc
        public Sort()
        {

        }
        public Sort(string sorting)
        {
            this.sorting = sorting;
        }
    }

    public class Response
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("complexity")]
        public string complexity { get; set; }
        [JsonProperty("result")]
        public List<string> result = new List<string>();
        [JsonProperty("total")]
        public string total{ get; set; }

    }

    class Program
    {

        static async Task Main(string[] args)
        {
            List<Stat> stats = new List<Stat>();
            stats.Add(new Stat("and"));

            Request item = new Request("online","The Pariah","Unset Ring",stats, "asc");
            


            var json = JsonConvert.SerializeObject(item);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var url = "https://www.pathofexile.com/api/trade/search/Harvest";
            using var client = new HttpClient();

            var response = await client.PostAsync(url, data);       // Send Object and await response

            string result = response.Content.ReadAsStringAsync().Result;
            var responseResult = JsonConvert.DeserializeObject<Response>(result);
            /*// Format Result Lines
            var formatted = JsonConvert.SerializeObject(responseResult, Formatting.Indented);
            Console.WriteLine(formatted);*/

            responseResult.result.RemoveRange(0,90);// trim list by 90 elements

            var resultArrayToString = string.Join(",", responseResult.result);

            //Console.WriteLine(responseResult.id);

            url = "https://www.pathofexile.com/api/trade/fetch/" + resultArrayToString; 
            //Console.WriteLine(responseResult.result.Count);
            var response2 = await client.GetStringAsync(url);   // Send request to get each item, MAXIMUM OF 10 ITEMS PER REQUEST!!!!!!!!!

            Console.WriteLine(response2);




        }
    }
}
