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
    public partial class MainForm : Form
    {
        IConsumer consumer;
        IProducer producer;

        BindingList<object> putJobs = new BindingList<object>();
        BindingList<Job> reservedJobs = new BindingList<Job>();

        public MainForm()
        {
            InitializeComponent();
            putJobsListBox.DataSource = putJobs;
            reservedJobsListBox.DataSource = reservedJobs;
        }

        async void connectButton_Click(object sender, EventArgs e)
        {
            connectButton.Enabled = false;
            disconnectButton.Enabled = true;

            var hostname = hostnameTextBox.Text;
            var port = Convert.ToInt32(portTextBox.Text);

            var c = BeanstalkConnection.ConnectConsumerAsync(hostname, port);
            var p = BeanstalkConnection.ConnectProducerAsync(hostname, port);
            await Task.WhenAll(c, p);
            consumer = c.Result;
            producer = p.Result;
        }

        void disconnectButton_Click(object sender, EventArgs e)
        {
            consumer.Dispose();
            producer.Dispose();

            connectButton.Enabled = true;
            disconnectButton.Enabled = false;
        }

        async void putJobButton_Click(object sender, EventArgs e)
        {
            var buffer = new byte[4];
            new Random().NextBytes(buffer);
            var id = await producer.PutAsync(buffer, 1, TimeSpan.FromSeconds(10));
            putJobs.Add(new { Id = id, Length = buffer.Length });
        }

        async void reserveButton_Click(object sender, EventArgs e)
        {
            reserveButton.Enabled = false;
            try
            {
                var job = await consumer.ReserveAsync(TimeSpan.FromSeconds(30));
                if (job != null)
                    reservedJobs.Add(job);
            }
            catch (TimeoutException)
            {
            }
            reserveButton.Enabled = true;
        }

        async void deleteJobButton_Click(object sender, EventArgs e)
        {
            var job = (Job)reservedJobsListBox.SelectedItem;
            if (job == null) return;
            var deleted = await consumer.DeleteAsync(job.Id);
            reservedJobs.Remove(job);
        }

        async void reservedJobsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var job = (Job)reservedJobsListBox.SelectedItem;
            if (job == null) return;
            var stats = await consumer.JobStatisticsAsync(job.Id);
            if (stats == null) return;
            idTextBox.Text = stats.Id.ToString();
            tubeTextBox.Text = stats.Tube;
            ageTextBox.Text = stats.Age.ToString();
            ttrTextBox.Text = stats.TimeToRun.ToString();
            stateTextBox.Text = stats.State.ToString();
        }

        private void spawnWorkerButton_Click(object sender, EventArgs e)
        {
            var form = new WorkerForm(hostnameTextBox.Text, Convert.ToInt32(portTextBox.Text));
            form.Show();
        }
    }
}
