using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace WebApiNew
{
    public class Prm
    {
        public Prm(string ParametreAdi, object ParametreDeger)
        {
            this.ParametreAdi = ParametreAdi;
            this.ParametreDeger = ParametreDeger;

            if (ParametreDeger == null) return;

            if (ParametreDeger is int)
                ParametreTip = SqlDbType.Int;
            else if (ParametreDeger is DateTime)
                ParametreTip = SqlDbType.DateTime;
            else if (ParametreDeger is double)
                ParametreTip = SqlDbType.Float;
            else if (ParametreDeger is bool)
                ParametreTip = SqlDbType.Bit;
            else
            {
                if (!(ParametreDeger is string))
                    ParametreDeger = Convert.ToString(ParametreDeger);
                ParametreTip = SqlDbType.NVarChar;
            }

        }

        public Prm()
        {

        }

        public string ParametreAdi { get; set; }
        public object ParametreDeger { get; set; }
        public SqlDbType ParametreTip { get; set; }
    }
}