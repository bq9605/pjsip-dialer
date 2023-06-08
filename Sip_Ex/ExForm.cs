using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sip_EX;
namespace Sip_EX
{
    public partial class ExForm : Form
    {
        private SoftPhone softPhone;
        public ExForm()
        {
            InitializeComponent();
            softPhone = new SoftPhone();
            softPhone.init();

        }
        protected override void OnClosed(EventArgs e)
        {
            softPhone.UnRegister();
            softPhone.Dispose();
            base.OnClosed(e);
        }
        private void ExForm_Load(object sender, EventArgs e)
        {

        }

        private void Reg_btn_Click(object sender, EventArgs e)
        {
            string hostname = "pbx.doorman24.net";
            string port = "5080";
            string displayName = "002Visentry";
            string authId = "002Visentry";
            string password = "Abcd@1234!";
            string userName = "002";

            softPhone.Register(displayName, userName, authId, password, hostname, port);
            
        }

        private void Unreg_btn_Click(object sender, EventArgs e)
        {
            softPhone.hangUp();
            softPhone.UnRegister();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if ((WAVopenFileDialog.ShowDialog() == DialogResult.OK))
            {
                string ext = "000";
                softPhone.makeCall(ext, WAVopenFileDialog.FileName.ToString());
            }
       
        }

        private void btn_hangup_Click(object sender, EventArgs e)
        {
            softPhone.hangUp();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            if ((WAVopenFileDialog.ShowDialog() == DialogResult.OK))
            {
                string ext = "005";
                softPhone.makeCall(ext, WAVopenFileDialog.FileName.ToString());
            }
        }
    }
}
