using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Turbocharged.Beanstalk;

namespace SampleApp
{
    public partial class WorkerForm : Form
    {
        string _hostname;
        int _port;
        IDisposable subscription;
        BindingList<Job> jobs = new BindingList<Job>();

        public WorkerForm(string hostname, int port)
        {
            InitializeComponent();
            _hostname = hostname;
            _port = port;
            jobsListBox.DataSource = jobs;
        }

        async void connectButton_Click(object sender, EventArgs e)
        {
            connectButton.Enabled = false;
            var tube = watchTextBox.Text;
            subscription = await BeanstalkConnection.ConnectWorkerAsync(_hostname, _port, tube, async (conn, job) =>
            {
                jobs.Add(job);
                await conn.DeleteAsync(job.Id);
                await Task.Delay(1000);
            });
            disconnectButton.Enabled = true;
        }

        void disconnectButton_Click(object sender, EventArgs e)
        {
            disconnectButton.Enabled = false;
            if (subscription != null)
                subscription.Dispose();
            subscription = null;
            connectButton.Enabled = true;
        }
    }
}
