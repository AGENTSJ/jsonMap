namespace Tester {
    
    public class Person
    {
        public int id;
        public string name;
        public string email;
        public Address address = new Address();
        public PhoneNumber[] phoneNumbers = new PhoneNumber[0];
        public Project[] projects = new Project[0];
    }
    
    public class Address
    {
        public string street;
        public string city;
        public string state;
        public string zipCode;
    }

    public class PhoneNumber
    {
        public string type;
        public string number;
    }

    public class Project
    {
        public int projectId;
        public string projectName;
        public string startDate;
        public string endDate;
        public Task[] tasks = new Task[0];
    }
   
    public class Task
    {
        public int taskId;
        public string taskName;
        public string status;
    }
    
    class SimpleTest
    {
        public string name;
        public innerSimpleTest numericDetails = new innerSimpleTest();
        public int[] randomArr;

        public innerSimpleTest[] testarr = new innerSimpleTest[5];
    }
    
    class innerSimpleTest
    {
        public int age;
        public int rollNo;
    }
}


