using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace WebPSOC.Models
{
    public sealed class ClsConfig
    {
        private static readonly ClsConfig instance = new ClsConfig();

        private ClsConfig() { }

        public static ClsConfig Instance
        {
            get
            {
                return instance;
            }
        }

        public string GetConnectionString()
        {
            //string connString = "Data Source=localhost; Initial Catalog=SPOC;Integrated Security=SSPI; Min Pool Size=10";
            return ConfigurationManager.ConnectionStrings["SPOC"].ConnectionString;
            ;
        }

        //public string GetTruckConnectionString()
        //{
        //    return ConfigurationManager.ConnectionStrings["TRUCK"].ConnectionString;
        //}
    }
}