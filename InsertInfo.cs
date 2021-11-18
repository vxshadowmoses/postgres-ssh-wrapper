using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_NAVIGATOR
{
    public class InsertInfo : ModifyInfo
    {
        public String TableName{ get; set; }
        public Dictionary<String, Object> KeyValueList { get; set; }

        public InsertInfo(String tableName) {
            this.TableName = tableName;
            this.KeyValueList = new Dictionary<string, object>();
        }
        public void Add(String key, Object val)
        {
            this.KeyValueList.Add(key, val);
        }
        public string GetQuery()
        {
            
            String keys = "";
            string values = "";
            foreach (KeyValuePair<string, Object> entry in this.KeyValueList)
            {
                int oInt = 0;
                keys += entry.Key + ",";
                if (Int32.TryParse(entry.Value.ToString(), out oInt))
                    values += entry.Value.ToString() + ",";
                else
                    values += "'"+entry.Value.ToString()+"'" + ",";



            }
            keys = keys.Trim().TrimEnd(',');
            values = values.Trim().TrimEnd(',');
            return String.Format("INSERT INTO {0} ({1}) VALUES ({2});",TableName, keys,values);
            
        }
    }
}
