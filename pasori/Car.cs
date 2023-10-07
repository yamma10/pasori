using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace pasori
{
    class Car
    {
        [JsonProperty("車No")]
        private String carNumber;
        [JsonProperty("車名")]
        private String carName;

        public String propertyCarNumber
        {
            set { carNumber = value; }
            get { return carNumber; }
        }

        public String propertyCarName
        {
            set { carName = value; }
            get { return carName; }
        }
    }
}
