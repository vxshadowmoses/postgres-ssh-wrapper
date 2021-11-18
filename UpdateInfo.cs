using System;
using System.Collections.Generic;

namespace DB_NAVIGATOR
{
    public class UpdateInfo : ModifyInfo
    {
        public object editData;
        public string whereCondition;
        public int primaryKeyValue;
        public string editColumn { get; set; }
        public string tableName { get; set; }
        public UpdateInfo(string editColumn, object newData, string whereCondition, int primaryKeyValue, string tableName)
        {
            this.editColumn = editColumn;
            this.editData = newData;
            this.whereCondition = whereCondition;
            this.primaryKeyValue = primaryKeyValue;
            this.tableName = tableName;
        }
      

        public string GetQuery()
        {
            int res;
            if (Int32.TryParse(editData.ToString(), out res))
                return string.Format("UPDATE {0} SET {1}={2} WHERE {3}={4};", tableName, editColumn, editData, whereCondition, primaryKeyValue);
            else
                return string.Format("UPDATE {0} SET {1}='{2}' WHERE {3}={4};", tableName, editColumn, editData, whereCondition, primaryKeyValue);

        }
    }
}