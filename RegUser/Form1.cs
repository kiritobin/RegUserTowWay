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
using System.Configuration;

namespace RegUser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string stuID = this.textBox1.Text.Trim();
            string stuName = this.textBox2.Text.Trim();
            string stuSex = this.radioButton1.Checked ? "男" : "女";

            string testDB = ConfigurationManager.ConnectionStrings["testDB"].ConnectionString;
            SqlConnection conn = new SqlConnection(testDB);
            try { 
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select * from T_StuInfo where StuID = @StuID";
            cmd.Parameters.AddWithValue("@StuID", stuID);
            cmd.Parameters.AddWithValue("@StuName", stuName);
            cmd.Parameters.AddWithValue("@StuSex", stuSex);
            SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    //判断读取到的数据库中的学号与输入的学号是否相同
                    if (reader["StuID"].ToString() == stuID)
                    {
                        MessageBox.Show("学号已存在");
                        this.textBox1.Focus();
                        this.textBox1.SelectAll();
                    }
                    //关闭读取器
                    reader.Close();
                }
                else
                {
                    //关闭读取器
                    reader.Close();
                    cmd.CommandText = "insert into T_StuInfo values(@StuID, @StuName, @StuSex)";

                    //获取执行sql语句后受影响的行数
                    int rowCount = cmd.ExecuteNonQuery();
                    if (rowCount == 1) //Update、Insert和Delete返回1，其他返回-1
                    {
                        MessageBox.Show("学生【" + this.textBox2.Text + "】录入成功！");
                        reader.Close(); 
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string stuID = this.textBox1.Text.Trim();
            string stuName = this.textBox2.Text.Trim();
            string stuSex = this.radioButton1.Checked ? "男" : "女";

            string testDB = ConfigurationManager.ConnectionStrings["testDB"].ConnectionString;
            SqlConnection conn = new SqlConnection(testDB);
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("up_addStu", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                //定义参数的值
                cmd.Parameters.AddWithValue("@StuID", stuID);
                cmd.Parameters.AddWithValue("@StuName", stuName);
                cmd.Parameters.AddWithValue("@sex", stuSex);

                //添加返回值参数
                SqlParameter count = cmd.Parameters.Add("@count", SqlDbType.Int);
                count.Direction = ParameterDirection.ReturnValue;
                cmd.ExecuteNonQuery(); //Update、Insert和Delete返回1 ，其他返回-1
                int i = int.Parse(cmd.Parameters["@count"].Value.ToString());

                if (i == -1)
                {
                    MessageBox.Show("学号已存在");
                    this.textBox1.Focus();
                    this.textBox1.SelectAll();
                }
                else if (i == 1)
                {
                    MessageBox.Show("学生【" + this.textBox2.Text + "】录入成功！");
                }
                else
                {
                    MessageBox.Show("添加失败");
                    this.textBox1.Focus();
                    this.textBox1.SelectAll();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
        }
    }
}
