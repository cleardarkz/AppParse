using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace AppParse
{
    public partial class Form1 : Form
    {
        List<APUser> users = new List<APUser>();
        public Form1()
        {
            InitializeComponent();
        }

        public class APUser
        {
            public string Name;
            public List<APMsg> Messages;
        }

        public class APMsg
        {
            public string Date;
            public string Time;
            public string Content;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            users = new List<APUser>();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string[] lines = File.ReadAllLines(openFileDialog1.FileName);

                    APMsg lastMessage = null;

                    Regex rx = new Regex(@"(\d{0,2}\/\d{0,2}\/\d{0,4}, \d{0,2}:\d{0,2} - )([^:]*)(.*)", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    foreach (string line in lines)
                    {
                        var mc = rx.Matches(line);
                        foreach (Match mcc in mc)
                        {
                            GroupCollection groups = mcc.Groups;
                            var date = groups[1].Value.Replace(" - ", "").Replace(", ", "|");
                            var sd = date.Split('|');
                            var sfrom = groups[2].Value;
                            var smessage = groups[3].Value.Replace(":", "");

                            var user = (from us in users where us.Name == sfrom select us).FirstOrDefault();
                            if (user != null)
                            {
                                var nmsg = new APMsg() { Content = smessage, Date = sd[0], Time = sd[1] };
                                user.Messages.Add(nmsg);
                                lastMessage = nmsg;
                            }
                            else
                            {
                                APUser us = new APUser();
                                us.Name = sfrom;
                                us.Messages = new List<APMsg>();
                                var nmsg = new APMsg() { Content = smessage, Date = sd[0], Time = sd[1] };
                                us.Messages.Add(nmsg);
                                lastMessage = nmsg;
                                users.Add(us);
                            }
                        }

                        if (mc.Count == 0)
                        {
                            lastMessage.Content += " ** " + line;
                        }
                    }

                    button2.Enabled = true;
                }

                catch(Exception ex)
                {
                    MessageBox.Show("Cannot load file, unknown format");
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var count = 0;
            foreach (var us in users)
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(us.Name + ".txt"))
                    {
                        foreach (var msg in us.Messages)
                        {
                            writer.WriteLine(msg.Date + "\t" + msg.Time + "\t" + msg.Content);
                        }

                        count++;
                    }
                }

                catch(Exception ex) { }


            }

            MessageBox.Show("Exported " + count + " files successfully");
            button2.Enabled = false;
        }
    }
}
