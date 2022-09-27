using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiNew
{
    public class Parametreler
    {
        public List<Prm> PARAMS { get; set; }

        public Parametreler()
        {
            PARAMS = new List<Prm>();
        }

        public void Clear()
        {
            PARAMS.Clear();
        }

        public void Add(string key, object value)
        {
            PARAMS.Add(new Prm(key, value));
        }

    }
}