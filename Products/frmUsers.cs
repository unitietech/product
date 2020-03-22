using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.Odbc;

namespace Products
{
    public partial class frmUsers : frmInheritance
    {
        string strAccessConnectionString = "Driver={Microsoft Access Driver (*.mdb)}; Dbq=Products.mdb; Uid=Admin; Pwd=;";

        bool boolUserExists = false;

        int intUserID = 0;

        public frmUsers()
        {
            InitializeComponent();
        }

        private void frmUsers_Load(object sender, EventArgs e)
        {
            controlsLoad();
            loadUsers();
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            frmMain frmMain = new frmMain();
            frmMain.Show();
            this.Hide();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (btnCreate.Text == "Save")
            {
                if (txtUserName.Text == "")
                {
                    MessageBox.Show("User Name field cannot be left empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (txtPassword.Text == "")
                {
                    MessageBox.Show("Password field cannot be left empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (txtFirstName.Text == "")
                {
                    MessageBox.Show("First Name field cannot be left empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (txtLastName.Text == "")
                {
                    MessageBox.Show("Last Name field cannot be left empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    checkIfUserExists();
                    if (boolUserExists == false)
                    {
                        createUser();
                        controlsLoad();
                        clearTextBoxes();
                        loadUsers();
                    }
                    else if (boolUserExists == true)
                    {
                        MessageBox.Show("User Already Exists ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    
                }
            }
            else if (btnCreate.Text == "Create")
            {
                controlsCreate();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            controlsEdit();
            editUser();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            updateUser();
            controlsLoad();
            clearTextBoxes();
            loadUsers();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            deleteUser();
            controlsLoad();
            clearTextBoxes();
            loadUsers();
        }

        private void controlsLoad()
        {
            txtPassword.Enabled = false;
            txtLastName.Enabled = false;
            txtFirstName.Enabled = false;
            txtUserName.Enabled = false;

            cboUsers.Enabled = true;

            btnCreate.Enabled = true;
            btnDelete.Enabled = false;
            btnEdit.Enabled = true;
            btnUpdate.Enabled = false;
            btnReturn.Enabled = true;

            btnCreate.Text = "Create";
        }

        private void controlsCreate()
        {
            txtPassword.Enabled = true;
            txtLastName.Enabled = true;
            txtFirstName.Enabled = true;
            txtUserName.Enabled = true;

            cboUsers.Enabled = false;

            btnCreate.Enabled = true;
            btnDelete.Enabled = false;
            btnEdit.Enabled = false;
            btnUpdate.Enabled = false;
            btnReturn.Enabled = false;

            btnCreate.Text = "Save";
        }

        private void controlsEdit()
        {
            txtPassword.Enabled = true;
            txtLastName.Enabled = true;
            txtFirstName.Enabled = true;
            txtUserName.Enabled = true;

            cboUsers.Enabled = false;

            btnCreate.Enabled = false;
            btnDelete.Enabled = true;
            btnEdit.Enabled = false;
            btnUpdate.Enabled = true;
            btnReturn.Enabled = false;
        }

        private void clearTextBoxes()
        {
            txtPassword.Text = "";
            txtLastName.Text = "";
            txtFirstName.Text = "";
            txtUserName.Text = "";
        }

        private void loadUsers()
        {
            cboUsers.DataSource = null;
            cboUsers.Items.Clear();

            OdbcConnection OdbcConnection = new OdbcConnection();
            OdbcConnection.ConnectionString = strAccessConnectionString;

            string query = "select username from Users";

            OdbcCommand cmd = new OdbcCommand(query, OdbcConnection);

            OdbcConnection.Open();

            OdbcDataReader dr = cmd.ExecuteReader();
            AutoCompleteStringCollection UserCollection = new AutoCompleteStringCollection();

            while (dr.Read())
            {
                UserCollection.Add(dr.GetString(0));
            }

            OdbcConnection.Close();

            cboUsers.DataSource = UserCollection;
        }

        private void checkIfUserExists()
        {
            string query = "select * from Users where UserName='" + txtUserName.Text + "'";

            OdbcConnection OdbcConnection = new OdbcConnection();
            OdbcCommand cmd;
            OdbcDataReader dr;

            OdbcConnection.ConnectionString = strAccessConnectionString;

            OdbcConnection.Open();

            cmd = new OdbcCommand(query, OdbcConnection);
            dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                boolUserExists = true;
            }

            dr.Close();
            OdbcConnection.Close();
            dr.Dispose();
            OdbcConnection.Dispose();
        }

        private void createUser()
        {
            string query = "select * from users where ID=0";

            OdbcConnection OdbcConnection = new OdbcConnection();
            OdbcDataAdapter da = new OdbcDataAdapter(query, OdbcConnection);

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DataRow dr;

            OdbcConnection.ConnectionString = strAccessConnectionString;

            da.Fill(ds, "Users");
            dt = ds.Tables["Users"];

            try
            {
                dr = dt.NewRow();
                dr["UserName"] = txtUserName.Text;
                dr["Password"] = txtPassword.Text;
                dr["FirstName"] = txtFirstName.Text;
                dr["LastName"] = txtLastName.Text;

                dt.Rows.Add(dr);
                OdbcCommandBuilder cmd = new OdbcCommandBuilder(da);
                da.Update(ds, "Users");
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message.ToString());
            }
            finally
            {
                OdbcConnection.Close();
                OdbcConnection.Dispose();
            }

        }

        private void editUser()
        {
            string query = "select * from users where username='" + cboUsers.Text + "'";

            OdbcConnection OdbcConnection = new OdbcConnection();
            OdbcCommand cmd;
            OdbcDataReader dr;

            OdbcConnection.ConnectionString = strAccessConnectionString;

            OdbcConnection.Open();

            cmd = new OdbcCommand(query, OdbcConnection);
            dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                intUserID = dr.GetInt32(0);
                txtUserName.Text = dr.GetString(1);
                txtPassword.Text = dr.GetString(2);
                txtFirstName.Text = dr.GetString(3);
                txtLastName.Text = dr.GetString(4);
                
            }

            dr.Close();
            OdbcConnection.Close();
            dr.Dispose();
            OdbcConnection.Dispose();
        }

        private void updateUser()
        {
            string query = "select * from users where id=" + intUserID;
            OdbcConnection OdbcConnection = new OdbcConnection();
           
            OdbcConnection.ConnectionString = strAccessConnectionString;

            OdbcDataAdapter da = new OdbcDataAdapter(query, OdbcConnection);
            DataSet ds = new DataSet("Users");

            da.FillSchema(ds, SchemaType.Source, "Users");
            da.Fill(ds, "Users");
            DataTable dt;
            
            dt = ds.Tables["Users"];
            DataRow dr;
            dr = dt.NewRow();

            try
            {
                dr = dt.Rows.Find(intUserID);
                dr.BeginEdit();

                dr["UserName"] = txtUserName.Text;
                dr["Password"] = txtPassword.Text;
                dr["FirstName"] = txtFirstName.Text;
                dr["LastName"] = txtLastName.Text;

                dr.EndEdit();

                OdbcCommandBuilder cmd = new OdbcCommandBuilder(da);
                da.Update(ds, "Users");
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message.ToString());
            }
            finally
            {
                OdbcConnection.Close();
                OdbcConnection.Dispose();
            }
        }

        private void deleteUser()
        {
            string query = "delete from users where id =" + intUserID;

            OdbcConnection OdbcConnection = new OdbcConnection();
            OdbcCommand cmd;
            OdbcDataReader dr;

            OdbcConnection.ConnectionString = strAccessConnectionString;

            OdbcConnection.Open();

            cmd = new OdbcCommand(query, OdbcConnection);
            dr = cmd.ExecuteReader();

            if (dr.Read())
            {

            }

            dr.Close();
            OdbcConnection.Close();
            dr.Dispose();
            OdbcConnection.Dispose();
        }
       
    }
}
        