using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Extensions
{
    public static class CSVToArrayExt
    {
        public static int[] SplitAsInt (this string delimitedString) {
               bool isParsingOk = true;
               int[] results = Array.ConvertAll<string,int>(delimitedString.Split(','), 
                    new Converter<string,int>(
                         delegate(string num)
                         {
                              int r;
                              isParsingOk &= int.TryParse(num, out r);
                              return r;
                         }));
               return results;
          }

    }
}