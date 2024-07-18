using System.Reflection;
namespace jsonMap{

    class JsonMap {
        static int idx = 0;
        public T jsonMap<T>(string jsonString) where T : new()
        {
            /*
                returns a class that is poppulated with contents from json String
            */
            T result = new T();
            Dictionary<string, object> contentMap;

            contentMap = JsonParse(jsonString);//parser
            Maper(result, contentMap);//maper
            return result;
        } 
        private void Maper(object? resultInstance, Dictionary<string, object> contentMap)
        {
            /*
                Maps fields or attributes of a given instance to their corresponding value that is returned from jsonParser
            */
            Type typeOfInstance;
            FieldInfo[] attributes;
            if(resultInstance== null){
                return;
            }
            typeOfInstance = resultInstance.GetType();
            attributes = typeOfInstance.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            string varTypeName;
            string varName;
            object value;
            foreach (FieldInfo attr in attributes)
            {
                varTypeName = attr.FieldType.Name.ToLower();
                varName = attr.Name;
                value = contentMap[varName];
                
                if (varTypeName == "string")
                {
                    attr.SetValue(resultInstance, (string)value);
                }
                else if (varTypeName == "int32[]")
                {
                    attr.SetValue(resultInstance, (Int32[])value);
                }
                else if (varTypeName == "int"|| varTypeName == "int32")
                {
                    attr.SetValue(resultInstance, (int)value);
                }
                else
                {
                    if(attr.GetValue(resultInstance)!=null){
                        Maper(attr.GetValue(resultInstance), (Dictionary<string ,object>)value);
                    }   
                }
            }
            return;
        }
        private Dictionary<string, object> JsonParse(string json)
        {
            /*Parses a json string and creates a hashmap that contain attribute Name -> attribute value*/

            Dictionary<string, object> attributes = new Dictionary<string, object>();

            if (json[idx] != '{')
            {
                Console.WriteLine("error invalid start");
                return attributes;
            }
            idx++;

            while (json[idx] != '}' && idx < json.Length)
            {
                string key = readKey(json);
                object value = readValue(json);
                attributes.Add(key, value);
            }
            return attributes;
        }
        private string readKey(string json)
        {
            /*
                reads and return the attribute name or key 
            */

            string buffer = "";
            skipWhitespaces(json);
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
            skipWhitespaces(json);
            idx++; //skiping -> :
            return buffer;
        }
        private object readValue(string json)
        {
            /*Read attribute value from json string and returns*/
            
            skipWhitespaces(json);

            if (json[idx] == '{')
            {
                Dictionary<string, object> buffer = new Dictionary<string, object>();
                buffer = JsonParse(json);
                idx++;
                return (object)buffer;
            }

            if (json[idx] == '\"')
            {
                string buffer = "";
                buffer = parseString(json);
                return (object)buffer;
            }
            else if (json[idx] == '[')
            {
                int[] buffer = parseArray(json);
                return (object)buffer;
            }
            else
            {
                int buffer = parseInt(json);
                return (object)buffer;
            }

        }
        private int parseInt(string json)
        {
            /*reads integer that is in string format and returns*/
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
        private int[] parseArray(string json)
        {
            // parses array from with in [ -> ]
            
            if (json[idx] != '[')
            {
                Console.WriteLine("invalid start of an array ");
                return new int[] { 0 };
            }
            idx++;
            List<int> res = new List<int>();
            while (json[idx] != ']' && idx < json.Length)
            {
                if (json[idx] != ',')
                {
                    int inte;
                    int.TryParse(json[idx].ToString(), out inte);
                    res.Add(inte);
                }
                idx++;
            }
            idx++; //skipping ]
            return res.ToArray();
        }
        private string parseString(string json)
        {
            //parses string in json with in " -> "
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
        static void skipWhitespaces(string json)
        {
            //skips white spaces
            while (json[idx] == ' ' && json.Length > idx)
            {
                idx++;
            }
        }
    }    
}
