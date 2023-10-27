﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOTHistoricalDataService.Config
{
    public class RabbitMQSettings
    {
        public string Hostname { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
        // Otros campos según tus necesidades
    }

}
