using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_NAVIGATOR
{
    public class DeleteInfo : ModifyInfo
    {

        public string whereCondition;
        public int primaryKeyValue;
        public string tableName { get; set; }
        public DeleteInfo(string whereCondition, int primaryKeyValue, string tableName) {
            this.whereCondition = whereCondition;
            this.primaryKeyValue = primaryKeyValue;
            this.tableName = tableName;
        }

       

        public string GetQuery()
        {
            return String.Format("DELETE FROM {0} WHERE {1}={2};", tableName, whereCondition, primaryKeyValue);
        }
    }
}
