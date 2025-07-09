using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace App_Consummer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            HttpClient client = new HttpClient();
           HttpResponseMessage responseMessage = client.GetAsync("https://localhost:7163/api/Employee").Result;

            if (responseMessage.IsSuccessStatusCode)
            {
               List<EmployeeData> employees = responseMessage.Content.ReadAsAsync<List<EmployeeData>>().Result;
                 DGV_Employees.DataSource = employees;
            }
            else
            {
                MessageBox.Show("Error fetching data: " + responseMessage.ReasonPhrase);
            }
        }
    }
}
