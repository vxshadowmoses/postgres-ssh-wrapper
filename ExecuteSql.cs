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
    public partial class ExecuteSql : Form
    {
        public String ScriptText { get; set; }
        public Config Configuration { get; internal set; }
        public SshDbWrapper ssh = null;

        public ExecuteSql()
        {
            InitializeComponent();
        }

        private void ExecuteSql_Load(object sender, EventArgs e)
        {
            txtScript.Text = ScriptText;

            this.ssh = new SshDbWrapper(Configuration.SSH_HOST,
                                        Configuration.SSH_USER,
                                        Configuration.SSH_PASSW,
                                        Configuration.DB_HOST,
                                        Configuration.DB_PORT,
                                        Configuration.DB_USER,
                                        Configuration.DB_SCHEMA,
                                        Configuration.DB_PASSWORD);
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            ssh.Connect();
            DBModifyOutput output = ssh.ExecuteScript(txtScript.Text);
            txtOuput.ForeColor = output.Status == 0 ? Color.Green : Color.Red;
            txtOuput.Text = output.Response;
            ssh.Disconnect();
        }
    }
}
