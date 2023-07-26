using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Console.Write("Enter Domain Name: ");
        string domainName = Console.ReadLine();
        Console.Write("Enter User Name: ");
        string userName = Console.ReadLine();
        Console.Write("Enter Password: ");
        string password = Console.ReadLine();

        Credential credential = new Credential
        {
            DomainName = domainName,
            UserName = userName,
            Password = password
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
                Console.WriteLine("Cannot Create Record! " + api.ErrorMessage);
                Console.Write("Do you want to try again? (Y or N): ");
            }

            ans = Console.ReadKey();
        }

        RetrieveMultipleQuery query = new RetrieveMultipleQuery
        {
            Columns = "FirstName,LastName,Phone,Email,Business_ContactId",
            OrderBy = "FirstName"
        };

        string queryData = SerializeToJson(query);
        responseData = await api.PostData("Business_Contact/RetrieveMultipleRecords", queryData, sessionId);

        if (responseData != null)
        {
            Business_Contact[] contacts = DeserializeFromJson<Business_Contact[]>(responseData);
            int i = 0;
            Console.WriteLine();
            foreach (Business_Contact contact in contacts)
            {
                Console.WriteLine("{0}| {1} | {2} | {3} | {4} | {5}", (++i).ToString("000"), contact.FirstName, contact.LastName, contact.Phone, contact.Email, contact.Business_ContactId);
            }
        }
        // Performing Update action getting Information from user
        Console.WriteLine("Do you wish to update record?  (Y or N):");
        ans = Console.ReadKey();
        if (ans.KeyChar == 'Y' || ans.KeyChar == 'y')
        {
            Console.WriteLine();
            Business_Contact contact = new Business_Contact();
            Console.Write("Enter Record Id: ");
            contact.Business_ContactId = Console.ReadLine();
            Console.Write("Enter First Name: ");
            contact.FirstName = Console.ReadLine();
            Console.Write("Enter Last Name: ");
            contact.LastName = Console.ReadLine();
            Console.Write("Enter Phone: ");
            contact.Phone = Console.ReadLine();
            Console.Write("Enter Email: ");
            contact.Email = Console.ReadLine();

            string contactData = SerializeToJson(contact);
            responseData = await api.PostData("Business_Contact/UpdateRecord", contactData, sessionId);

            if (!string.IsNullOrEmpty(responseData))
            {
                string contactId = DeserializeFromJson<string>(responseData);
                Console.WriteLine("Record Updated: " + contactId);
            }
            else
            {
                Console.WriteLine("Cannot Update Record! " + api.ErrorMessage);
            }
        }

        Console.WriteLine();
        // Performing Delete action getting Information from user
        Console.WriteLine("Do you wish to delete record? (Y or N): ");
        ans = Console.ReadKey();
        if (ans.KeyChar == 'Y' || ans.KeyChar == 'y')
        {
            Console.WriteLine();
            Business_Contact contact = new Business_Contact();
            Console.Write("Enter Record Id: ");
            contact.Business_ContactId = Console.ReadLine();

            string contactData = SerializeToJson(contact);
            responseData = await api.PostData("Business_Contact/DeleteRecord", contactData, sessionId);

            if (!string.IsNullOrEmpty(responseData))
            {
                string contactId = DeserializeFromJson<string>(responseData);
                Console.WriteLine("Record Deleted: " + contactId);
            }
            else
            {
                Console.WriteLine("Cannot Delete Record! " + api.ErrorMessage);
            }
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