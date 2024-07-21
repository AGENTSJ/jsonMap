using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace jsonMap
{
    internal class Program
    {
        static void Main(string[] args)
        {
           
            string jsonTest = "{\"name\":  \"Abhijith SJ\",\"numericDetails\":{\"age\": 21,\"rollNo\": 2},\"randomArr\":[1,2,3,4],\"testarr\":[{\"age\":18,\"rollNo\":14}]}";
            // Test temper = JsonMap.jsonMap<Test>(jsonTest);
            // Console.WriteLine(temper.numericDetails.age);
            JsonMap jMap = new();
            Test test = jMap.Extract<Test>(jsonTest);
            Console.WriteLine(test.numericDetails.rollNo);
            Console.WriteLine(test.testarr[0].age);
        }
    }
    class Test
    {
        public string name;
        public test2 numericDetails = new test2();
        public int[] randomArr;

        public test2[] testarr = new test2[5];
    }
    class test2
    {
        public int age;
        public int rollNo;
    }
}
