<<<<<<< HEAD
using System;
using Newtonsoft.Json;

namespace YandexDiskSDK
{
    [Serializable]
    public class PersonData
    {
        public string Id { get; private set; }
        public string ClientId { get; private set; }
        public string PsuId { get; private set; }
        public string[] Emails { get; private set; }
        public string DefaultEmail { get; private set; }
        public string Login { get; private set; }
        public string Name { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string DisplayName { get; private set; }
        public string Country { get; private set; }
        public Phone Phone { get; private set; }
        public string Sex { get; private set; }

        [JsonConstructor]
        public PersonData(string id, string client_id, string psuId, string[] emails,
                          string default_email, string login, string name, string first_name,
                          string last_name, string display_name, string country, Phone default_phone, string sex)
        {
            Name = name;
            Id = id;
            ClientId = client_id;
            PsuId = psuId;
            Emails = emails;
            DefaultEmail = default_email;
            Login = login;
            FirstName = first_name;
            LastName = last_name;
            DisplayName = display_name;
            Country = country;
            Phone = default_phone;
            Sex = sex;
        }
    }

    public class Phone
    {
        public int Id { get; private set; }
        public string Number { get; private set; }

        [JsonConstructor]
        public Phone(int id, string number)
        {
            Id = id;
            Number = number;
        }
    }
=======
using System;
using Newtonsoft.Json;

namespace YandexDiskSDK
{
    [Serializable]
    public class PersonData
    {
        public string Id { get; private set; }
        public string ClientId { get; private set; }
        public string PsuId { get; private set; }
        public string[] Emails { get; private set; }
        public string DefaultEmail { get; private set; }
        public string Login { get; private set; }
        public string Name { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string DisplayName { get; private set; }
        public string Country { get; private set; }
        public Phone Phone { get; private set; }
        public string Sex { get; private set; }

        [JsonConstructor]
        public PersonData(string id, string client_id, string psuId, string[] emails,
                          string default_email, string login, string name, string first_name,
                          string last_name, string display_name, string country, Phone default_phone, string sex)
        {
            Name = name;
            Id = id;
            ClientId = client_id;
            PsuId = psuId;
            Emails = emails;
            DefaultEmail = default_email;
            Login = login;
            FirstName = first_name;
            LastName = last_name;
            DisplayName = display_name;
            Country = country;
            Phone = default_phone;
            Sex = sex;
        }
    }

    public class Phone
    {
        public int Id { get; private set; }
        public string Number { get; private set; }

        [JsonConstructor]
        public Phone(int id, string number)
        {
            Id = id;
            Number = number;
        }
    }
>>>>>>> 54935b7afcc8c9ace832f3baf9523de90599e286
}