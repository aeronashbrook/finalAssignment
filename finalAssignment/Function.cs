using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Dynamic;


[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace finalAssignment
{
    public class Item
    {
        public string id;
        public string name;
        public double price;
        public double market_cap;

    }
    public class Function
    {

        public static readonly HttpClient client = new HttpClient();
        private static AmazonDynamoDBClient dbClient = new AmazonDynamoDBClient();
        private static string tableName = "crypto";
        public async Task<ExpandoObject> FunctionHandler(APIGatewayProxyRequest input, ILambdaContext context)
        {

            string symbol = "";
            Dictionary<string, string> dict = (Dictionary<string, string>)input.QueryStringParameters;
            dict.TryGetValue("symbol", out symbol);

            Console.WriteLine(symbol);

            HttpResponseMessage response = await client.GetAsync(
                "https://coinlib.io/api/v1/coin?key=0a6960060053679d&symbol=" + symbol
                );
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(responseBody);
            Console.WriteLine(obj.symbol);
            Console.WriteLine(obj.name);
            Console.WriteLine(obj.price);
            Console.WriteLine(obj.market_cap);

            double price = Convert.ToDouble(obj.price);
            double market_cap = Convert.ToDouble(obj.market_cap);


            //Table items = Table.LoadTable(dbClient, tableName);
            //Item myItem = new Item();
            //myItem.id = obj.symbol;
            //myItem.name = obj.name;
            //myItem.price = obj.price;
            //myItem.market_cap = obj.market_cap;


            Dictionary<string, AttributeValue> myDictionary = new Dictionary<string, AttributeValue>();
            myDictionary.Add("id", new AttributeValue { S = obj.symbol });
            myDictionary.Add("name", new AttributeValue { S = obj.name });
            myDictionary.Add("price", new AttributeValue { N = price.ToString() });
            myDictionary.Add("market_cap", new AttributeValue { N = market_cap.ToString() });
            PutItemRequest myRequest = new PutItemRequest(tableName, myDictionary);
            PutItemResponse res = await dbClient.PutItemAsync(myRequest);


            return obj;
        }
    }
}
