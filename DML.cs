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
    public partial class DMLForm : Form
    {
        public String Table { get; set; }
        public Config RemoteConfig { get; set; }
        public PrimaryKeyModel PrimaryKeyData { get; private set; }
        public int InitialRowsCount { get;  set; }

        public List<ModifyInfo> UpdateList = new List<ModifyInfo>();
        public List<ModifyInfo> InsertList = new List<ModifyInfo>();
        public SshDbWrapper ssh = null; 

        public DMLForm()
        {
            InitializeComponent();
        }

        private void DML_Load(object sender, EventArgs e)
        {
           

            this.ssh = new SshDbWrapper(RemoteConfig.SSH_HOST,
                                        RemoteConfig.SSH_USER,
                                        RemoteConfig.SSH_PASSW,
                                        RemoteConfig.DB_HOST,
                                        RemoteConfig.DB_PORT,
                                        RemoteConfig.DB_USER,
                                        RemoteConfig.DB_SCHEMA,
                                        RemoteConfig.DB_PASSWORD);
            QuerySelect();

        }

        private void QuerySelect()
        {
            
            DataTable sqlresponse = new DataTable();

            
            ssh.Connect();
            SelectOutput selectOutput = ssh.SelectFromTable(Table, txtWhere.Text, txtOrder.Text, txtLimit.Text);
            

            dataGridView1.DataSource = selectOutput.Table;
            PrimaryKeyData = selectOutput.PrimaryKey;
            ssh.Disconnect();

            UpdateList.Clear();
            InsertList.Clear();

            InitialRowsCount = dataGridView1.Rows.Count;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
         
            DataGridView gridView = ((DataGridView)sender);
            int r = e.RowIndex;
            int c = e.ColumnIndex;
            if (r < (gridView.Rows.Count - 1)) {
                UpdateList.Add(new UpdateInfo(gridView.Columns[e.ColumnIndex].Name,
                                            gridView.Rows[r].Cells[c].Value,
                                            PrimaryKeyData.PrimaryKeyName,
                                            Int32.Parse(gridView.Rows[r].Cells[PrimaryKeyData.PrimaryKeyIndex].Value.ToString()),
                                            Table
                                            ));
                gridView.Rows[r].Cells[c].Style.BackColor = Color.Yellow;
            }

        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            QuerySelect();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            
            ssh.Connect();
            
            DBModifyOutput output = ssh.ModifyDML(UpdateList);
            ssh.Disconnect();
            txtResult.Text = output.Response;

            QuerySelect();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            List<ModifyInfo> deleteList = new List<ModifyInfo>();
            foreach (DataGridViewRow row in dataGridView1.SelectedRows) {
                DeleteInfo newDel = new DeleteInfo(PrimaryKeyData.PrimaryKeyName, 
                                                    Int32.Parse(row.Cells[PrimaryKeyData.PrimaryKeyIndex].Value.ToString()), 
                                                    Table);

                deleteList.Add(newDel);
                
            }

           
            ssh.Connect();
            DBModifyOutput output = ssh.ModifyDML(deleteList);
            ssh.Disconnect();
            txtResult.Text = output.Response;

            QuerySelect();

        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > InitialRowsCount) {
                for (int i = InitialRowsCount-1; i < (dataGridView1.Rows.Count-1); i++)
                {
                    InsertInfo modInfo = new InsertInfo(Table);
                    for (int c = 0; c < dataGridView1.Columns.Count; c++)
                    {
                        if (PrimaryKeyData != null && PrimaryKeyData.PrimaryKeyIndex == c) //skip primary key in insert
                            continue;

                        modInfo.Add(dataGridView1.Columns[c].Name, dataGridView1.Rows[i].Cells[c].Value);
                        dataGridView1.Rows[i].Cells[c].Style.BackColor = Color.Green;
                    }

                    InsertList.Add(modInfo);
                }

                ssh.Connect();
                DBModifyOutput output = ssh.ModifyDML(InsertList);
                ssh.Disconnect();
                txtResult.Text = output.Response;

                QuerySelect();
            }
            
        }

       
    }
}
