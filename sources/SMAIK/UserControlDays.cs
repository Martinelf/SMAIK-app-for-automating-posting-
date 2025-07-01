using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMAIK
{
    public partial class UserControlDays : UserControl
    {
        public int nday;

        public UserControlDays()
        {
            InitializeComponent();
            
        }

        private void UserControlDays_Load(object sender, EventArgs e)
        {

        }

        public void days(int numday, string publications)
        {
            lbdays.Text = numday + "";
            nday = numday;
            lbpubls.Text = publications;
        }

        public Action<int> OnDayPlanUpdateNeeded;
        public Action<int> OnAddPublicationNeeded;


        private void UserControlDays_Click(object sender, EventArgs e)
        {
            //MainForm.displayDayPlan(nday);
            //DayPlanRequested?.Invoke(nday);

            OnDayPlanUpdateNeeded?.Invoke(nday);
            
        }
        
        private void UserControlDays_DoubleClick(object sender, EventArgs e)
        {
            OnAddPublicationNeeded?.Invoke(nday);
        }

        public void removeSelection()
        {
            this.BackColor = Color.White;
        }


    }
}
