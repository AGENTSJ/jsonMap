using Tester;

namespace jsonMap
{
    internal class Program
    {
        static void Main(string[] args)
        {
           
            string jsonTest1 = File.ReadAllText("/home/abhijithsj/Desktop/Active/jsonMap/TestJsons/test.json");
            JsonMap jMap = new();
            SimpleTest simpleTest = jMap.Extract<SimpleTest>(jsonTest1);

            string test2 = File.ReadAllText("/home/abhijithsj/Desktop/Active/jsonMap/TestJsons/test2.json");
            Person person = jMap.Extract<Person>(test2);
        }
    }
}
