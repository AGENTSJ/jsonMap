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
           
            string jsonTest = "{\"name\":  \"Abhijith SJ\",\"numericDetails\":{\"age\": 21,\"rollNo\": 2},\"randomArr\":[1,2,3,4]}";
            Test temper = JsonMap.jsonMap<Test>(jsonTest);
            Console.WriteLine(temper.numericDetails.rollNo);
        }
    }
    class Test
    {
        public string? name;
        public test2 numericDetails = new test2();
        public int[]? randomArr;
    }
    class test2
    {
        public int? age;
        public int? rollNo;
    }
}
