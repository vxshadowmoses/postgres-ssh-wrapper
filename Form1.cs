using DB_NAVIGATOR.config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace DB_NAVIGATOR
{
    public partial class Form1 : Form
    {
        private ContextMenuStrip listboxContextMenu;
        private string _selectedMenuItem;
        private readonly ContextMenuStrip collectionRoundMenuStrip;
        public SshDbWrapper ssh = null;
        public Dictionary<String, Config> configList = new Dictionary<string, Config>();

        public Config SelectedConnection { get; private set; }

        public Form1()
        {
            InitializeComponent();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }


        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //select the item under the mouse pointer
                listBox1.SelectedIndex = listBox1.IndexFromPoint(e.Location);
                if (listBox1.SelectedIndex != -1)
                {
                    listboxContextMenu.Show();
                }
            }
        }

        private void selectMenu_Click(object sender, EventArgs e) {
            DMLForm dmlform = new DMLForm();
            dmlform.Table = listBox1.SelectedItem.ToString();
            dmlform.RemoteConfig = SelectedConnection;
            dmlform.ShowDialog();

        }
        private void describeMenu_Click(object sender, EventArgs e)
        {
           

            
            ssh.Connect();
            DescribeOutput descOutput = ssh.Describe(listBox1.SelectedItem.ToString());

            DataTable dtDescribe = new DataTable();
            dtDescribe.Columns.Add("Name");
            dtDescribe.Columns.Add("Type");
            dtDescribe.Columns.Add("Modifiers");
            dtDescribe.Columns.Add("Primary Key");
            foreach (DBColumn column in descOutput.Columns)
            {
                DataRow r = dtDescribe.NewRow();
                r["Name"] = column.ColumnName;
                r["Type"] = column.Type;
                r["Modifiers"] = column.Modifiers;
                r["Primary Key"] = column.IsPrimaryKey ? "X" : "";
                dtDescribe.Rows.Add(r);
            }
            describeTable.DataSource = dtDescribe;
            


            ssh.Disconnect();

        }
        private void listboxContextMenu_Opening(object sender, CancelEventArgs e)
        {
            //clear the menu and add custom items
            listboxContextMenu.Items.Clear();
            var describeMenu = new ToolStripMenuItem { Text = string.Format("DDL Edit [{0}]", listBox1.SelectedItem.ToString()) };
            var selectMenu = new ToolStripMenuItem { Text = string.Format("DML Edit [{0}]", listBox1.SelectedItem.ToString()) };

            describeMenu.Click += describeMenu_Click;
            selectMenu.Click += selectMenu_Click;

            listboxContextMenu.Items.Add(describeMenu);
            listboxContextMenu.Items.Add(selectMenu);

        }
        private void Load_Script(object sender, EventArgs e)
        {
            String scriptContent = String.Empty;
            if (SelectedConnection != null)
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = "c:\\";
                    openFileDialog.Filter = "txt files (*.txt)|*.txt|Sql files (*.sql)|*.sql|All files (*.*)|*.*";
                    openFileDialog.FilterIndex = 2;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        //Get the path of specified file
                        String filePath = openFileDialog.FileName;

                        //Read the contents of the file into a stream
                        var fileStream = openFileDialog.OpenFile();

                        using (StreamReader reader = new StreamReader(fileStream))
                        {
                            scriptContent = reader.ReadToEnd();
                        }

                        ExecuteSql executeSqlForm = new ExecuteSql();
                        executeSqlForm.Configuration = SelectedConnection;
                        executeSqlForm.ScriptText = scriptContent;
                        executeSqlForm.ShowDialog();

                    }
                }
            }
            else
            {
                MessageBox.Show("You have to choose a remote connection first","Error", MessageBoxButtons.OK);
            }
            
        }
        
        private void Chose_Remote(object sender, EventArgs e)
        {
            // save
        }
        private void Open_Click(object sender, EventArgs e)
        {
            // save
        }
        private void Diff_Open(object sender, EventArgs e) {
            DiffForm diffForm = new DiffForm();
            diffForm.ConfigList = configList;
            diffForm.ShowDialog();
        
        }
        private void GrepDB_Open(object sender, EventArgs e)
        {
            //GrepDB grepForm = new GrepDB();
            //grepForm.ConfigList = configList;
            //grepForm.ShowDialog();

        }
        private void Configuration_Click(object sender, EventArgs e)
        {
            String confName = ((MenuItem)sender).Text.Trim();
            SelectedConnection = configList[confName];
            RemoteConnection(configList[confName]);

        }
            
        private void Form1_Load(object sender, EventArgs e)
        {

            MenuItem fileMenu = new MenuItem("File");
            MenuItem optionsMenu = new MenuItem("Connection");
            MenuItem extraMenu = new MenuItem("Extra");
            mainMenu1.MenuItems.Add(fileMenu);
            mainMenu1.MenuItems.Add(optionsMenu);
            mainMenu1.MenuItems.Add(extraMenu);

            fileMenu.MenuItems.Add("Load Script", new EventHandler(Load_Script));
            fileMenu.MenuItems.Add("Open", new EventHandler(Open_Click));

            XmlLoadConfig(optionsMenu);

            extraMenu.MenuItems.Add("Diff", new EventHandler(Diff_Open));
            extraMenu.MenuItems.Add("Grep DB", new EventHandler(Diff_Open));





        }

        private void RemoteConnection(Config nConfig)
        {
            


            this.ssh = new SshDbWrapper(nConfig.SSH_HOST,
                                                    nConfig.SSH_USER,
                                                    nConfig.SSH_PASSW,
                                                    nConfig.DB_HOST,
                                                    nConfig.DB_PORT,
                                                    nConfig.DB_USER,
                                                    nConfig.DB_SCHEMA,
                                                    nConfig.DB_PASSWORD);
            ssh.Connect();
            List<String> tableList = ssh.TableList();
            ssh.Disconnect();


            //assign a contextmenustrip
            listboxContextMenu = new ContextMenuStrip();
            listboxContextMenu.Opening += new CancelEventHandler(listboxContextMenu_Opening);
            listBox1.ContextMenuStrip = listboxContextMenu;

            listBox1.Items.Clear();
            foreach (String table in tableList)
            {
                listBox1.Items.Add(table);
            }
        }

        private void XmlLoadConfig(MenuItem optionsMenu)
        {
            XDocument doc = XDocument.Load("C:\\Users\\BresciaRE\\source\\repos\\DB_NAVIGATOR\\config.xml");
            //XDocument doc = XDocument.Load("config.xml");

            foreach (var config in doc.Descendants("config"))
            {
                Config nConfig = new Config(config);
                configList.Add(config.Attribute("name").Value, nConfig);
                optionsMenu.MenuItems.Add(String.Format("{0}", config.Attribute("name").Value), Configuration_Click);

            }
        }

    }
}
