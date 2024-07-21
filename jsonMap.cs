using System.Reflection;
using System.Collections;
namespace jsonMap
{

    class JsonMap
    {
        private int idx = 0;

        /* returns a class that is poppulated with contents from json String */
        public T Extract<T>(string jsonString) where T : new()
        {
            T result = new T();
            Dictionary<string, object> contentMap;

            contentMap = JsonParse(jsonString);//parser
            Maper(result, contentMap);//maper
            return result;
        }
        
        /* Maps fields or attributes of a input instance to their corresponding value that is returned from jsonParser */
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

                    // Console.WriteLine(arrayElementType);
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
       
        /*Parses a json string and creates a hashmap that contain attribute Name -> attribute value*/
        private Dictionary<string, object> JsonParse(string json)
        {

            Dictionary<string, object> attributes = new Dictionary<string, object>();

            if (json[idx] != '{')
            {
                Console.WriteLine("error invalid start");
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
        
        /* reads and return the attribute name or key */
        private string ReadKey(string json)
        {
            string buffer = "";
            SkipWhitespaces(json);
            if (json[idx] == ',')
            {
                idx++;
            }

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
        
        /*Read attribute value from json string and returns*/
        private object ReadValue(string json)
        {
            SkipWhitespaces(json);

            if (json[idx] == '{')
            {
                Dictionary<string, object> buffer = new Dictionary<string, object>();
                buffer = JsonParse(json);
                return (object)buffer;
            }

            if (json[idx] == '\"')
            {
                string buffer = "";
                buffer = ParseString(json);
                return (object)buffer;
            }
            else if (json[idx] == '[')
            {
                object[] buffer = ParseArray(json);
                return (object)buffer;
            }
            else
            {
                int buffer = ParseInt(json);
                return (object)buffer;
            }

        }
        
        /*reads integer that is in string format and returns*/
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
        
        // parses array from with in [ -> ]
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
        
        //parses string in json with in " -> "
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
        
        //skips white spaces
        private void SkipWhitespaces(string json)
        {
            while (json[idx] == ' ' && json.Length > idx)
            {
                idx++;
            }
        }
    }
}