using Newtonsoft.Json;
using System;

namespace Nop.Plugin.Misc.ContaAzul.Models.Message.Response
{
    public partial class CustomerResponse
    {
        public CustomerResponse()
        {
            address = new Address();
        }

        [JsonProperty("id")]
        public Guid id { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("company_name")]
        public string companyName { get; set; }

        [JsonProperty("email")]
        public string email { get; set; }

        [JsonProperty("business_phone")]
        public string businessPhone { get; set; }

        [JsonProperty("mobile_phone")]
        public string mobilePhone { get; set; }

        [JsonProperty("person_type")]
        public string personType { get; set; }

        [JsonProperty("document")]
        public string document { get; set; }

        [JsonProperty("identity_document")]
        public string identityDocument { get; set; }

        [JsonProperty("state_registration_number")]
        public string stateRegistrationNumber { get; set; }

        [JsonProperty("state_registration_type")]
        public string stateRegistrationType { get; set; }

        [JsonProperty("city_registration_number")]
        public string cityRegistrationNumber { get; set; }

        [JsonProperty("date_of_birth")]
        public string dateOfBirth { get; set; }

        [JsonProperty("notes")]
        public string notes { get; set; }

        [JsonProperty("address")]
        public Address address { get; set; }     
    }
    public partial class Address
    {
        public Address()
        {
            city = new City();
            state = new State();
        }

        [JsonProperty("street")]
        public string street { get; set; }

        [JsonProperty("number")]
        public string number { get; set; }

        [JsonProperty("complement")]
        public string complement { get; set; }

        [JsonProperty("zip_code")]
        public string zipCode { get; set; }

        [JsonProperty("neighborhood")]
        public string neighborhood { get; set; }

        [JsonProperty("city")]
        public City city { get; set; }

        [JsonProperty("state")]
        public State state { get; set; }
    }

    public partial class City
    {
        [JsonProperty("name")]
        public string name { get; set; }
    }

    public partial class State
    {
        [JsonProperty("name")]
        public string name { get; set; }
    }
}
