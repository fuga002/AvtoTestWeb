using Example;
using Newtonsoft.Json;

var data = File.ReadAllText("products.json");
var products = JsonConvert.DeserializeObject<List<Product>>(data);

foreach (var product in products)
{
    Console.WriteLine($"Category : {product.Cateogory}");
    Console.WriteLine($"Product name : {product.ProductName}");
    if (product.Media.Exist)
    {
        Console.WriteLine($"FileName : {product.Media.Name}");
    }
}