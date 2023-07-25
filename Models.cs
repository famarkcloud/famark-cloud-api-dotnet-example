using System.Runtime.Serialization;

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

public class RetrieveMultipleQuery
{
    [DataMember]
    public string Columns { get; set; }

    [DataMember]
    public string OrderBy { get; set; }
}