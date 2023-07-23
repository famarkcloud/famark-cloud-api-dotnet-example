using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

internal class Program
{
    [DataContract]
    public class Credential
    {
        [DataMember]
        public string DomainName { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string Password { get; set; }
    }

    [DataContract]
    public class Business_Contact
    {
        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string Phone { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string Business_ContactId { get; set; }
    }

    private static async Task Main(string[] args)
    {
        Console.Write("Enter Domain Name: ");
        string domainName = Console.ReadLine();
        Console.Write("Enter User Name: ");
        string userName = Console.ReadLine();
        Console.Write("Enter Password: ");
        string passord = Console.ReadLine();

        Credential credential = new Credential
        {
            DomainName = domainName,
            UserName = userName,
            Password = passord
        };

        string credString = SerializeToJson<Credential>(credential);

        FamarkCloud api = new FamarkCloud();
        string responseData = await api.PostData("Credential/Connect", credString, null);

        if (string.IsNullOrEmpty(responseData))
        {
            Console.Error.WriteLine("Login Failed: " + api.ErrorMessage);
            return;
        }

        string sessionId = DeserializeFromJson<string>(responseData);

        Console.Write("Do you want to create a new record? (Y or N): ");
        ConsoleKeyInfo ans = Console.ReadKey();
        while (ans.KeyChar == 'Y' || ans.KeyChar == 'y')
        {
            Console.WriteLine();
            Business_Contact contact = new Business_Contact();
            Console.Write("Enter First Name: ");
            contact.FirstName = Console.ReadLine();
            Console.Write("Enter Last Name: ");
            contact.LastName = Console.ReadLine();
            Console.Write("Enter Phone: ");
            contact.Phone = Console.ReadLine();
            Console.Write("Enter Email: ");
            contact.Email = Console.ReadLine();

            string contactData = SerializeToJson(contact);
            responseData = await api.PostData("Business_Contact/CreateRecord", contactData, sessionId);

            if (!string.IsNullOrEmpty(responseData))
            {
                string contactId = DeserializeFromJson<string>(responseData);
                Console.WriteLine("Created RecordId: " + contactId);
                Console.Write("Do you want to create another record? (Y or N): ");
            }
            else
            {
                Console.WriteLine("Cannot Create Record!");
                Console.Write("Do you want to try again? (Y or N): ");
            }

            ans = Console.ReadKey();
        }
    }

    private static string SerializeToJson<T>(T item)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            new DataContractJsonSerializer(typeof(T)).WriteObject(ms, item);
            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }

    public static T DeserializeFromJson<T>(string jsonString)
    {
        using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
        {
            return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(ms);
        }
    }
}