using System.Reflection;
using System.Collections;

namespace jsonMap
{
    /*
        A Library that can map values of a json string to attributes of a class 

        Entry point Extract()
        
        Note : nested classes should be initilzed with new()
            
            class ExampleClass {
                innerClass test2 = new innerClass();
            }
        
    */

    class JsonMap
    {
        private int idx = 0;

        /* 
            Returns a instance of the class that user want to get poppulated with the contents of json string
            JsonMap jMap = new JsonMap()
            ExampleClass exmp = jMap.Extract<ExampleClass>(jsonString);
        */
        public T Extract<T>(string jsonString) where T : new()
        {
            this.idx = 0;
            T result = new T();
            Dictionary<string, object> contentMap;

            contentMap = JsonParse(jsonString);//parser
            Maper(result, contentMap);//maper
            return result;
        }
        
        /*

            Returns a Dictionary 
            Dictinoary contains keys and and values extracted from the json string

            Note : nested json (equivalent to nested classes) are treated as another json and handled by ReadValue()
            and stored as a dictionary 

        */

        private Dictionary<string, object> JsonParse(string json)
        {
            Dictionary<string, object> attributes = new Dictionary<string, object>();

            if (json[idx] != '{')
            {
                Console.WriteLine("error invalid start of an json");
                return attributes;
            }
            idx++;

            while (json[idx] != '}' && idx < json.Length)
            {
                string key = ReadKey(json);
                object value = ReadValue(json);
                attributes.Add(key, value);
            }
            idx++; //skipping -> }
            return attributes;
        }
        
        /* 

            Input  : 1 instance of the class that need to be poppulated ,  
                     2 Dictionary of Name of attribute to Value object
            Output : poppulated class instance

        */
        private void Maper(object? resultInstance, Dictionary<string, object> contentMap)
        {
           
            const string STRING = "string";
            const string INT = "int32";
            const string ARRAY_IDEN = "[]";

            Type typeOfInstance;
            FieldInfo[] attributes;
            if (resultInstance == null)
            {
                return;
            }

            typeOfInstance = resultInstance.GetType();
            attributes = typeOfInstance.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            string varTypeFullName;
            string varTypeName;
            string varName;
            object value;

            foreach (FieldInfo attr in attributes) // set each attribute of a class
            {
                varTypeFullName = attr.FieldType.FullName;
                varTypeName = attr.FieldType.Name.ToLower();
                varName = attr.Name;
                value = contentMap[varName];

                if (varTypeName == STRING)
                {
                    attr.SetValue(resultInstance, (string)value);

                }

                else if (varTypeName.Contains(ARRAY_IDEN))
                {
                    object[] temp = (object[])value;
                    Type arrayElementType = attr.FieldType.GetElementType();
                    var listType = typeof(List<>).MakeGenericType(arrayElementType);
                    var list = (IList)Activator.CreateInstance(listType);

                    foreach (object ob in temp)
                    {
                        if (!ob.GetType().Name.ToLower().Contains("dictionary"))
                        {
                            var castedValue = Convert.ChangeType(ob, arrayElementType);
                            list.Add(castedValue);
                        }
                        else
                        {
                            object nestedClassInstance = Activator.CreateInstance(arrayElementType);
                            Maper(nestedClassInstance,(Dictionary<string,object>)ob);
                            list.Add(nestedClassInstance);
                        }
                        var toArrayMethod = list.GetType().GetMethod("ToArray");
                        var array = toArrayMethod.Invoke(list, null);
                        attr.SetValue(resultInstance, toArrayMethod.Invoke(list, null));
                        
                    }
                }

                else if (varTypeName == INT || varTypeName == "int")
                {
                    attr.SetValue(resultInstance, (int)value);

                }

                else if (attr.GetValue(resultInstance) != null)
                {
                    Maper(attr.GetValue(resultInstance), (Dictionary<string, object>)value);
                }
            }
            return;
        }
        
        /* 
        
            Reads and return the attribute Name or key
        
        */
        private string ReadKey(string json)
        {
            string buffer = "";
            SkipWhitespaces(json);
            if (json[idx] == ',')
            {
                idx++;
            }
            SkipWhitespaces(json);
            if (json[idx] != '\"')
            {
                Console.WriteLine("invalid start of a key " + json[idx]);
                return "";
            }
            idx++;

            while (json[idx] != '\"' && idx < json.Length)
            {
                buffer += json[idx];
                idx++;
            }
            idx++; //skipping -> " in json
            SkipWhitespaces(json);
            idx++; //skiping -> :
            return buffer;
        }
        
        /*
        
            Read attribute value from json string and returns the value 

            Note : if the value is an another json (equvalent to another class instance it calls JsonParse()) 
        
        */
        private object ReadValue(string json)
        {
            SkipWhitespaces(json);

            if (json[idx] == '{')
            {
                Dictionary<string, object> buffer = new Dictionary<string, object>();
                buffer = JsonParse(json);
                SkipWhitespaces(json);
                return (object)buffer;
            }

            if (json[idx] == '\"')
            {
                string buffer = "";
                buffer = ParseString(json);
                SkipWhitespaces(json);
                return (object)buffer;
            }
            else if (json[idx] == '[')
            {
                object[] buffer = ParseArray(json);
                SkipWhitespaces(json);
                return (object)buffer;
            }
            else
            {
                int buffer = ParseInt(json);
                SkipWhitespaces(json);
                return (object)buffer;
            }

        }
        
        /*
        
            Reads integer that is in string format and returns
        
        */
        private int ParseInt(string json)
        {
            string buffer = "";
            int res = 0;
            while ("1234567890".Contains(json[idx]) && idx < json.Length)
            {
                buffer += json[idx];
                idx++;
            }
            int.TryParse(buffer, out res);
            return res;
        }
        
        /*
        
            Parses array from with in [ -> ]
        
        */
        private object[] ParseArray(string json)
        {
            if (json[idx] != '[')
            {
                Console.WriteLine("invalid start of an array ");
                return new object[] { 0 };
            }
            idx++;
            List<object> res = new List<object>();
            while (json[idx] != ']' && idx < json.Length)
            {
                if (json[idx] != ',')
                {
                    object insideVal = ReadValue(json);
                    res.Add(insideVal);
                }
                else
                {
                    idx++;
                }

            }
            idx++; //skipping ]
            return res.ToArray();
        }
        
        /*
          
            Parses string in json with in " -> "
        
        */
        private string ParseString(string json)
        {
            string buffer = "";
            if (json[idx] != '\"')
            {
                Console.WriteLine("inavlid start of a key");
                return buffer;
            }
            idx++;
            while (json[idx] != '\"' && idx < json.Length)
            {
                buffer += json[idx];
                idx++;
            }
            idx++;//skiping "
            return buffer;

        }
        
        /* 
          
            Skips white spaces

        */
        private void SkipWhitespaces(string json)
        {
            while ((json[idx] == ' ' || json[idx] == '\n') && json.Length > idx)
            {
                idx++;
            }
        }
    }
}