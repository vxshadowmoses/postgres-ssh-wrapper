using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DB_NAVIGATOR
{
    public class PrimaryKeyModel
    {
        public String PrimaryKeyName { get; set; }
        public int PrimaryKeyIndex { get; set; }
    }
    public class DBColumn
    {
        public String ColumnName { get; set; }
        public String Type { get; set; }
        public String Modifiers { get; set; }
        public bool IsPrimaryKey = false;
    }
    public class DescribeOutput
    {
        public String TableName { get; set; }
        public String RawText { get; set; }
        public List<DBColumn> Columns { get; set; }
        public PrimaryKeyModel PrimaryKey { get; set; }
        public void SetPrimaryKey(String primaryKeyName)
        {
            int i = 0;
            foreach (DBColumn col in Columns) {
                if (col.ColumnName.Equals(primaryKeyName, StringComparison.InvariantCultureIgnoreCase)) {
                    col.IsPrimaryKey = true;
                    PrimaryKey = new PrimaryKeyModel();
                    PrimaryKey.PrimaryKeyName = col.ColumnName;
                    PrimaryKey.PrimaryKeyIndex = i;
                }
                i++;
                    
            }
        }

    }

    public class SelectOutput {
        public DataTable Table {get;set;}
        public PrimaryKeyModel PrimaryKey { get; set; }
    
    }

    public class DBModifyOutput
    {
        public string Response { get; internal set; }
        public int Status { get; set; }
    }
    public class SshDbWrapper
    {
        
        SshClient sshClient = null;

        public string SshHostName { get; private set; }
        public string SshUsername { get; private set; }
        public string SshPassword { get; private set; }
        public string DbHost { get; private set; }
        public string DbPort { get; private set; }
        public string DbUser { get; private set; }
        public string DbSchema { get; private set; }
        public string DbPassword { get; private set; }
        public string BashPath { get; private set; }
        public string SqlFile { get; private set; }

        public SshDbWrapper(String sshHostName, 
                            String sshUsername,
                            String sshPassword,
                            String dbHost,
                            String dbPort,
                            String dbUser,
                            String dbSchema,
                            String dbPassword) {

            this.SshHostName = sshHostName;
            this.SshUsername = sshUsername;
            this.SshPassword = sshPassword;
            this.DbHost = dbHost;
            this.DbPort = dbPort;
            this.DbUser = dbUser;
            this.DbSchema = dbSchema;
            this.DbPassword = dbPassword;
            this.BashPath = "/bin/bash";
            this.SqlFile = "/tmp/query.sql";
            sshClient = new SshClient(this.SshHostName, this.SshUsername, this.SshPassword);
            
        }

        public String PsqlInlineCommand(String query) {
            return String.Format("{0} -c 'PGPASSWORD={1} psql -h {2} -p {3} -U {4} -d {5} -c \"{6}\"'",
                                    BashPath,
                                    DbPassword,
                                    DbHost,
                                    DbPort,
                                    DbUser,
                                    DbSchema,
                                    query);
        }
        public String PsqlScriptExec()
        {
            return String.Format("{0} -c 'PGPASSWORD={1} psql -h {2} -p {3} -U {4} -d {5} -f \"{6}\"'",
                                    BashPath,
                                    DbPassword,
                                    DbHost,
                                    DbPort,
                                    DbUser,
                                    DbSchema,
                                    SqlFile);
        }
        public void Connect() {
            sshClient.Connect();
           
        }
        
        public List<String> TableList() {


           
            var response = sshClient.RunCommand(PsqlInlineCommand("\\dt"));

            List<String> tableList = new List<String>();

            foreach(String line in response.Result.Split('\n')) {
                String[] properties = line.Split('|');
                if (properties.Length > 2 &&
                    properties[0].Trim().Equals("public", StringComparison.InvariantCultureIgnoreCase)) {
                    tableList.Add(properties[1].Trim());
                }

            }

            return tableList;
            
        }

        public DescribeOutput Describe(string tableName)
        {
            var response = sshClient.RunCommand(PsqlInlineCommand(String.Format("\\d {0}",tableName)));
            DescribeOutput output = new DescribeOutput();
            List<DBColumn> describeColumns = new List<DBColumn>();

            output.TableName = tableName;
            int rowCounter = 0;
            string primaryKey = "";
            foreach (String line in response.Result.Split('\n'))
            {
                output.RawText += line + System.Environment.NewLine;
                if (rowCounter > 1)
                {
                    string[] spltArg = line.Split('|');
                    if (spltArg.Length > 1)
                    {
                        DBColumn newColumn = new DBColumn();
                        newColumn.ColumnName = spltArg[0].Trim();
                        newColumn.Type = spltArg[1].Trim();
                        if (spltArg.Length > 2)
                            newColumn.Modifiers = spltArg[2].Trim();
                        describeColumns.Add(newColumn);

                    }
                    else {

                        if (line.Trim().ToUpper().Contains("PRIMARY KEY,")) {
                            string[] keyval = line.Split('(');
                            primaryKey = keyval[1].TrimEnd(')'); 
                           
                        }
                            
                        
                    }
                }
                rowCounter++;
            }

            output.Columns = describeColumns;
            output.SetPrimaryKey(primaryKey);
            return output;
        }

        public DBModifyOutput ExecuteScript(String scriptContent) {
            DBModifyOutput output = new DBModifyOutput();
            sshClient.RunCommand(String.Format("rm {0}", SqlFile));

            foreach (String splitLine in scriptContent.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)) {
                sshClient.RunCommand(String.Format("echo \"{0}\" >> {1}", splitLine, SqlFile));
            }
            var response = sshClient.RunCommand(PsqlScriptExec());
            output.Response = response.ExitStatus == 0 ? response.Result : response.Error;
            output.Status = response.ExitStatus;

            return output;
        }
      
        public DBModifyOutput ModifyDML(List<ModifyInfo> updateList)
        {
            DBModifyOutput output = new DBModifyOutput();
            if (updateList != null)
            {
                sshClient.RunCommand(String.Format("rm {0}",SqlFile));
                foreach (ModifyInfo update in updateList) {
                    sshClient.RunCommand(String.Format("echo \"{0}\" >> {1}", update.GetQuery(),SqlFile));

                }

                var response = sshClient.RunCommand(PsqlScriptExec());
                output.Response = response.ExitStatus==0 ? response.Result : response.Error;
                output.Status = response.ExitStatus;
            }
            
            return output;
        }
        public SelectOutput SelectFromTable(string table, 
                                        String whereCondition, 
                                        String order,
                                        String limit
                                        ){
            String columnList = "";
            

            DescribeOutput describe = Describe(table);
            DataTable outputTable = new DataTable();
            SelectOutput selectOutput = new SelectOutput();

            foreach (DBColumn column in describe.Columns)
            {
                columnList += column.ColumnName + ", ";
                outputTable.Columns.Add(column.ColumnName);
            }

            columnList = columnList.Trim().TrimEnd(',');
            if (!string.IsNullOrEmpty(whereCondition)) {
                whereCondition = "WHERE " + whereCondition;
            }
            if (!string.IsNullOrEmpty(order))
            {
                order = "ORDER BY " + order;
            }
            if (!string.IsNullOrEmpty(limit))
            {
                limit = "LIMIT " + limit;
            }
            String writeCommand = String.Format("echo \"SELECT {0} FROM {1} {2} {3} {4};\" > {5}", columnList, table, whereCondition, order, limit,SqlFile);
            var response = sshClient.RunCommand(writeCommand);
            response = sshClient.RunCommand(PsqlScriptExec());

            int counter = 0;

            foreach (String line in response.Result.Split('\n')) {
                if (counter > 1) {
                    string[] listOfData = line.Split('|');
                    if (listOfData.Length > 1)
                    {
                        DataRow r = outputTable.NewRow();
                        int i = 0;  
                        foreach (DataColumn column in outputTable.Columns)
                        {
                            r[column.ColumnName] = listOfData[i].Trim();
                            i++;
                        }
                        
                        outputTable.Rows.Add(r);
                       

                    }
                }
                counter++;
            }

            selectOutput.Table = outputTable;
            selectOutput.PrimaryKey = describe.PrimaryKey;
            return selectOutput;
        }

        public void Disconnect() {
            
            sshClient.Disconnect();
        }
    }
}
