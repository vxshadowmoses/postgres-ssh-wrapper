using DB_NAVIGATOR.config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DB_NAVIGATOR
{
    public partial class DiffForm : Form
    {

        public Dictionary<String, Config> ConfigList { get; set; }
        public SshDbWrapper SourceConnection { get; private set; }
        public SshDbWrapper DestinationConnection { get; private set; }

        public DiffForm()
        {
            InitializeComponent();
        }

        private void DiffForm_Load(object sender, EventArgs e)
        {
            foreach (KeyValuePair<String, Config> entry in ConfigList)
            {
                ddlListConnection1.Items.Add(entry.Key);
                ddlListConnection2.Items.Add(entry.Key);
            }

        }

        private void btnDdlCompare_Click(object sender, EventArgs e)
        {
            Config source = ConfigList[(String)ddlListConnection1.SelectedItem];
            Config destination = ConfigList[(String)ddlListConnection2.SelectedItem];

            SourceConnection = new SshDbWrapper(source.SSH_HOST,
                                                                source.SSH_USER,
                                                                source.SSH_PASSW,
                                                                source.DB_HOST,
                                                                source.DB_PORT,
                                                                source.DB_USER,
                                                                source.DB_SCHEMA,
                                                                source.DB_PASSWORD);

            DestinationConnection = new SshDbWrapper(destination.SSH_HOST,
                                                                    destination.SSH_USER,
                                                                    destination.SSH_PASSW,
                                                                    destination.DB_HOST,
                                                                    destination.DB_PORT,
                                                                    destination.DB_USER,
                                                                    destination.DB_SCHEMA,
                                                                    destination.DB_PASSWORD);

            SourceConnection.Connect();
            DestinationConnection.Connect();

            lstBox1.Items.Clear();
            foreach (String table in SourceConnection.TableList())
            {

                lstBox1.Items.Add(table);
            }
            lstBox2.Items.Clear();
            foreach (String table in DestinationConnection.TableList())
            {

                lstBox2.Items.Add(table);
            }


            SourceConnection.Disconnect();
            DestinationConnection.Disconnect();
        }

        private void btnCompare_Click(object sender, EventArgs e)
        {
            SourceConnection.Connect();
            DestinationConnection.Connect();
            progressBar.Maximum = lstBox1.Items.Count;
            progressBar.Value = 0;

            foreach (String table in lstBox1.Items) {
                if (!In(table, lstBox2.Items))
                {
                    resultDiff.Nodes.Add(String.Format("{1} - missing table", ddlListConnection1.SelectedItem, table));
                }
                else
                {
                    TreeNode node = null;
                    List <DBColumn> columnsMismatch = GetMissingColumn(table, SourceConnection, DestinationConnection);
                    if (columnsMismatch.Count > 0)
                    {
                        node = resultDiff.Nodes.Add(String.Format("{1} - missing column", ddlListConnection1.SelectedItem, table));
                        foreach(DBColumn col in columnsMismatch)
                        {
                            node.Nodes.Add(col.ColumnName);
                        }
                    }

                   
                }
                progressBar.Value += 1;
            }

            SourceConnection.Disconnect();
            DestinationConnection.Disconnect();
        }

        private List<DBColumn> GetMissingColumn(string table, 
                                                SshDbWrapper sourceConnection, 
                                                SshDbWrapper destinationConnection)
        {
            DescribeOutput sourceTabDescription = sourceConnection.Describe(table);
            DescribeOutput destinationTabDescription = destinationConnection.Describe(table);
            List<DBColumn> missingColumns = new List<DBColumn>();

            foreach(DBColumn col in sourceTabDescription.Columns)
            {
                if(!ExistColumn(destinationTabDescription, col))
                {
                    missingColumns.Add(col);
                }
            }
            return missingColumns;
        }

        private bool ExistColumn(DescribeOutput destinationTabDescription, DBColumn dbColumn)
        {
            foreach(DBColumn destColumn in destinationTabDescription.Columns)
            {
                if (destColumn.ColumnName.Equals(dbColumn.ColumnName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private bool In(string table, ListBox.ObjectCollection items)
        {
            foreach(String item in items)
            {
                if (table.Trim().Equals(item, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private void lstBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void resultDiff_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }
    }
}
