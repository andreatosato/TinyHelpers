﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TinyHelpers.Extensions;
using TinyHelpers.Json.Serialization;
using TinyHelpers.Threading;

namespace TinyHelpers.Sample
{
    public class Person
    {
        [JsonPropertyName("first_name")]
        public string? FirstName { get; set; }

        public string? LastName { get; set; }
    }

    public enum Priority
    {
        [Display(Name = "Bassa")]
        [EnumMember(Value = "Bassa priorita")]
        Low,

        [Display(Name = "Media")]
        [EnumMember(Value = "Media priorita")]
        Medium,

        [Display(Name = "Alta")]
        [EnumMember(Value = "Alta priorita")]
        High
    };

    [Flags]
    public enum ConnectionTypes
    {
        [Display(Name = "Cavo")]
        Wired = 1,
        WiFi = 2,
        Bluetooth = 4,
        [Display(Name = "Via Satellite")]
        Satellite = 8
    };

    internal class Program
    {
        private static readonly AsyncLock syncObject = new AsyncLock();

        private static async Task Main(string[] args)
        {
            var dateTime = new DateTime(2020, 1, 1);
            var time = new TimeSpan(15, 42, 0);

            var list = new Dictionary<string, string> { ["A"] = "First Value", ["B"] = "Second Value" };

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };

            jsonSerializerSettings.Converters.Add(new StringEnumConverter());

            var oldJson = JsonConvert.SerializeObject(list, jsonSerializerSettings);
            var oldResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(oldJson, jsonSerializerSettings);


            var jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            jsonSerializerOptions.Converters.Add(new UtcDateTimeConverter());
            jsonSerializerOptions.Converters.Add(new TimeSpanConverter());
            jsonSerializerOptions.Converters.Add(new StringEnumMemberConverter());

            var json = System.Text.Json.JsonSerializer.Serialize(list, jsonSerializerOptions);
            var result = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json, jsonSerializerOptions);

            var connectionTypes = ConnectionTypes.Wired | ConnectionTypes.Satellite;
            foreach (var connectionType in connectionTypes.GetFlags())
            {
                Foo(connectionType);
            }

            connectionTypes.GetFlags().ForEach(c => Foo(c));

            var priority = Priority.Medium;
            Console.WriteLine($"La priorità è {priority.GetDescription()}");

            //var a = "Marco";
            //var b = "marco";

            //if (a.EqualsIgnoreCase(b))
            //{

            //}

            //var inputName = "";
            //var name = inputName.GetValueOrDefault("sconosciuto");

            var people = GetPeople();

            foreach (var (person, i) in people.WithIndex())
            {
                Console.WriteLine($"Elaboro la persona {i}...");
            }

            var task = Task.Delay(5000);
            await task.TimeoutAfterAsync(TimeSpan.FromSeconds(10));

            using (await syncObject.LockAsync())
            {
                await Task.Delay(1000);
            }
        }

        private static void Foo(ConnectionTypes connectionType)
        {
            Console.WriteLine($"Analizzo la connessione {connectionType.ToString()}...");
            // ...
        }

        private static IEnumerable<Person> GetPeople()
        {
            var people = new List<Person>
           {
                new Person { FirstName = "Donald", LastName = "Duck" },
                new Person { FirstName = "Mickey", LastName = "Mouse" },
                new Person { FirstName = "Daisy", LastName = "Duck" }
             };

            return people;
        }
    }
}
