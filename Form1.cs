using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.OleDb;

namespace TestTaskSA
{
    public partial class Form1 : Form
    {
        private SqlConnection sqlConnection = null;

        private SqlCommandBuilder sqlBuilder = null;

        private SqlDataAdapter sqlDataAdapter = null;

        private DataSet dataSet = null;
        private string SelectTableList { get; set;}

        private bool newRowAdding = false;
        public Form1()
        {
            InitializeComponent();
            
        }

        private void LoadData()
        {

            try
            {
                sqlDataAdapter = new SqlDataAdapter($"SELECT *, 'Delete' AS [Delete] FROM {SelectTableList}", sqlConnection);

                sqlBuilder = new SqlCommandBuilder(sqlDataAdapter);

                sqlBuilder.GetInsertCommand();
                sqlBuilder.GetUpdateCommand();
                sqlBuilder.GetDeleteCommand();

                dataSet = new DataSet();

                sqlDataAdapter.Fill(dataSet, "Users");
                dataGridView1.DataSource = dataSet.Tables["Users"];

                //LinkLabel

                for (int i = 0; i < dataGridView1.Rows.Count; i++) {
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView1[6, i] = linkCell;
                }
            }

            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Eror!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReloadData()
        {

            try
            {

                // Стираем прошлые данные, чтобы избежать дубликатов
                dataSet.Tables["Users"].Clear();


                sqlDataAdapter.Fill(dataSet, "Users");
                dataGridView1.DataSource = dataSet.Tables["Users"];

                //LinkLabel

                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView1[6, i] = linkCell;
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Eror!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            LoadData();
            
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "sATestDBDataSet.Users". При необходимости она может быть перемещена или удалена.
            this.usersTableAdapter.Fill(this.sATestDBDataSet.Users);

            SelectTableList = "Users";
            string BDpath = Application.StartupPath + "\\SATestDB.mdf";
            MessageBox.Show(BDpath);
            sqlConnection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Олег\Desktop\TestTask\TestTaskSA\SATestDB.mdf;Integrated Security=True");
            // Доработать строку подключения, сделать ее универсальной
            sqlConnection.Open();
            if (sqlConnection.State == ConnectionState.Open) { MessageBox.Show($"Base connection: {sqlConnection.ConnectionString}"); }
            else { MessageBox.Show("Base connection lost"); }
            LoadData();

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ReloadData();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try {
                if (e.ColumnIndex == 6) {
                    string task = dataGridView1.Rows[e.RowIndex].Cells[6].Value.ToString();
                    // Разные действия зачем мне 3 if, если есть switch)
                    switch (task)
                    {
                        case "Delete":
                            if (MessageBox.Show("Удалить эту строку", "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                                int rowIndex = e.RowIndex;
                                dataGridView1.Rows.RemoveAt(rowIndex);
                                dataSet.Tables["Users"].Rows[rowIndex].Delete();
                                sqlDataAdapter.Update(dataSet, "Users");
                            }
                            break;
                        case "Insert":
                            {
                                int rowIndex = dataGridView1.Rows.Count - 2;
                                DataRow row = dataSet.Tables["Users"].NewRow();

                                row["Name"] = dataGridView1.Rows[rowIndex].Cells["Name"].Value;
                                row["Surname"] = dataGridView1.Rows[rowIndex].Cells["Surname"].Value;
                                row["Birthday"] = dataGridView1.Rows[rowIndex].Cells["Birthday"].Value;
                                row["Email"] = dataGridView1.Rows[rowIndex].Cells["Email"].Value;
                                row["Phone"] = dataGridView1.Rows[rowIndex].Cells["Phone"].Value;

                                dataSet.Tables["Users"].Rows.Add(row);

                                dataSet.Tables["Users"].Rows.RemoveAt(dataSet.Tables["Users"].Rows.Count - 1);

                                dataGridView1.Rows.RemoveAt(dataGridView1.Rows.Count - 2);
                                dataGridView1.Rows[e.RowIndex].Cells[6].Value = "Delete";

                                sqlDataAdapter.Update(dataSet, "Users");
                                newRowAdding = false;


                            }
                            break;
                        case "Update":
                            {
                                int r = e.RowIndex;

                                dataSet.Tables["Users"].Rows[r]["Name"] = dataGridView1.Rows[r].Cells["Name"].Value;
                                dataSet.Tables["Users"].Rows[r]["Surname"] = dataGridView1.Rows[r].Cells["Surname"].Value;
                                dataSet.Tables["Users"].Rows[r]["Birthday"] = dataGridView1.Rows[r].Cells["Birthday"].Value;
                                dataSet.Tables["Users"].Rows[r]["Email"] = dataGridView1.Rows[r].Cells["Email"].Value;
                                dataSet.Tables["Users"].Rows[r]["Phone"] = dataGridView1.Rows[r].Cells["Phone"].Value;


                                sqlDataAdapter.Update(dataSet, "Users");

                                dataGridView1.Rows[e.RowIndex].Cells[6].Value = "Delete";


                            }
                            break;


                    }
                    ReloadData();
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Eror!", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            try {
                if (newRowAdding == false) {
                    newRowAdding = true;

                    int lastRow = dataGridView1.Rows.Count - 2;

                    DataGridViewRow row = dataGridView1.Rows[lastRow];
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();

                    dataGridView1[6, lastRow] = linkCell;
                    row.Cells["Delete"].Value = "Insert";
                }
            }

            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Eror", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (newRowAdding == false) {
                    int rowIndex = dataGridView1.SelectedCells[0].RowIndex;

                    DataGridViewRow editingRow = dataGridView1.Rows[rowIndex];

                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();

                    dataGridView1[6, rowIndex] = linkCell;
                    editingRow.Cells["Delete"].Value = "Update";
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Eror", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Column_KeyPress);

            if (dataGridView1.CurrentCell.ColumnIndex == 5) {
                TextBox textBox = e.Control as TextBox;

                if (textBox != null) {
                    textBox.KeyPress += new KeyPressEventHandler(Column_KeyPress);
                }
            }
        }

        private void Column_KeyPress(object sender, KeyPressEventArgs e) {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; //27 05 пересмотр и дебаг
            }
        }
        private void TestENT()
        {
            //https://stackoverflow.com/questions/12552058/import-from-dataset-to-combobox-c-sharp
            //https://pyatnitsev.ru/2012/03/28/accesscombobox/

            SelectTableList = listBox1.Text.ToString();
            MessageBox.Show(SelectTableList, Text);


        }
        
        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void ViewTable()
        {
        }

        private void bindingSource1_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            TestENT();
        }
    }
}
