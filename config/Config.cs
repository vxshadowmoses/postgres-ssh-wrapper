using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_NAVIGATOR.config
{
    public class Config
    {


        public String SSH_HOST = "";
        public String SSH_USER = "";
        public String SSH_PASSW = "";
        public String DB_HOST = "";
        public String DB_PORT = "";
        public String DB_USER = "";
        public String DB_SCHEMA = "";
        public String DB_PASSWORD = "";


     

        public Config(System.Xml.Linq.XElement option) {
            var sshOption = option.Descendants("ssh").ToList()[0];
            SSH_HOST = sshOption.Attribute("host").Value;
            SSH_USER = sshOption.Attribute("user").Value;
            SSH_PASSW = sshOption.Attribute("pass").Value;
            
            var dbOption = option.Descendants("database").ToList()[0];
            DB_HOST = dbOption.Attribute("host").Value;
            DB_PORT = dbOption.Attribute("port").Value;
            DB_USER = dbOption.Attribute("user").Value;
            DB_SCHEMA = dbOption.Attribute("schema").Value;
            DB_PASSWORD = dbOption.Attribute("pass").Value;

        }
    }
}
