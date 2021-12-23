using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ITSSBWinforms
{
    public partial class Form1 : Form
    {
        AmionicData db;
        ErrorProvider errorProvider;
        List<String> sortList = new List<string>()
        {
            "Date-Time","Price","Confirmed"
        };
        public Form1()
        {
            InitializeComponent();
            errorProvider=new ErrorProvider();
            db= new AmionicData();
            var q = db.Airports.ToList();
            q.Insert(0, new Airport()
            {
                ID = 0,
                IATACode = "All Airports"
            });
            var p = db.Airports.ToList();
            p.Insert(0, new Airport()
            {
                ID = 0,
                IATACode = "All Airports"
            });
            comboBox1.DataSource = q;
            comboBox1.DisplayMember = "IATACode";
            comboBox1.ValueMember = "ID";
            comboBox2.DataSource = p;
            comboBox2.DisplayMember = "IATACode";
            comboBox2.ValueMember = "ID";
            comboBox3.DataSource = sortList;
            LoadData();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!checkBox1.Checked)
            {
                dateTimePicker1.Enabled = false;
            }
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                Schedule s = (Schedule)dataGridView1.Rows[i].Cells["Obj"].Value;
                if (!s.Confirmed)
                {
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                }

            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                dateTimePicker1.Enabled = true;
            }
            else
            {
                dateTimePicker1.Enabled=false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == comboBox2.SelectedItem)
            {
                errorProvider.SetError(comboBox1, "From and To Cannot be Same");
                errorProvider.SetError(comboBox2, "From and To Cannot be Same");
                return;
            }
            LoadData();
        }

        private void LoadData()
        {
            String sortBy = (string)comboBox3.SelectedValue;
            bool outboundStatus = checkBox1.Checked;
            int fromID = (int)comboBox1.SelectedValue;
            int toId = (int)comboBox2.SelectedValue;
            DateTime outbound = dateTimePicker1.Value.Date;
            String flightNumber = textBox1.Text.ToString();
            var ds = db.Schedules.Where(x =>
              (x.Route.Airport.ID == fromID || fromID == 0) &&
              (x.Route.Airport1.ID == toId || toId == 0) &&
              (x.Date == outbound.Date || outboundStatus == false) &&
              (x.FlightNumber == flightNumber || flightNumber == "")
            ).Select(x => new
            {
                Date = x.Date,
                Time = x.Time,
                From = x.Route.Airport.IATACode,
                To = x.Route.Airport1.IATACode,
                FlightNumber = x.FlightNumber,
                Aircraft = x.Aircraft.MakeModel,
                EconomyPrice = x.EconomyPrice,
                BusinessPrice = x.EconomyPrice + (0.35M * x.EconomyPrice),
                FirstClassPrice = x.EconomyPrice + (0.35M * x.EconomyPrice) + (0.3M * (x.EconomyPrice + (0.35M * x.EconomyPrice))),
                Confirmed = x.Confirmed,
                Obj = x
            }).ToList();
            if (sortBy == "Date-Time")
            {
                ds = ds.OrderByDescending(x => x.Date + x.Time).ToList();
            }
            else if (sortBy == "Price")
            {
                ds = ds.OrderByDescending(x => x.EconomyPrice).ToList();
            }

            dataGridView1.DataSource = ds;
            dataGridView1.Columns["Obj"].Visible = false;
            dataGridView1.Columns["FlightNumber"].HeaderText = "Flight Number";
            dataGridView1.Columns["FirstClassPrice"].HeaderText = "First Class Price";
            dataGridView1.Columns["Confirmed"].Visible = false;
        }
    }
}
