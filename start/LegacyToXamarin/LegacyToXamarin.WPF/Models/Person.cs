﻿using Newtonsoft.Json;
using System;

namespace LegacyToXamarin.WPF.Models
{
    public class Person
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "birthday")]
        public DateTimeOffset Birthday { get; set; }
    }
}
