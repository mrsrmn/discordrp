using System;
using DiscordRPC;
using System.Windows.Forms;
using System.Drawing;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using DiscordRPC.Logging;
using Newtonsoft.Json.Linq;
using Button = DiscordRPC.Button;

namespace discordrp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        private void button_Click(object sender, EventArgs e)
        {
            var client = new DiscordRpcClient(clientId.Text);
            Timestamps timestamp;
            
            client.Initialize();
            if (button.Text == "Start RP")
            {
                timestamp = timestampBox.Checked ? Timestamps.Now : null;

                client.SetPresence(new RichPresence()
                {
                    Details = detailsBox.Text, 
                    Timestamps = timestamp, 
                    State = stateBox.Text,
                    Assets = new Assets()
                    {
                        //small image
                        SmallImageKey = smallImageKey.Text,
                        SmallImageText = smallImageText.Text,
                        
                        //large image
                        LargeImageKey = largeImageKey.Text,
                        LargeImageText = largeImageText.Text
                    }
                });
                button.Enabled = false; 
                
                client.OnReady += (senderOnready, eOnready) =>
                {
                    labelUsername.Text = $"{client.CurrentUser.Username}#{client.CurrentUser.Discriminator}";
                    
                    var request = WebRequest.Create($"https://cdn.discordapp.com/avatars/{client.CurrentUser.ID}/{client.CurrentUser.Avatar}.png?width=70&height=70");
                    using (var response = request.GetResponse())
                    using (var stream = response.GetResponseStream())
                    {
                        pictureBox1.Image = Bitmap.FromStream(stream);
                    }
                    
                    label6.Text = "Online";
                    
                    button.Text = "Stop RP";
                    button.BackColor = Color.Crimson;
                    button.Enabled = true;
                    button1.Enabled = true;
                };
            }
            else if (button.Text == "Stop RP")
            {
                button.Enabled = false;
                client.ClearPresence();
                
                label6.Text = "Offline";
                button.Text = "Start RP";
                button.BackColor = Color.GreenYellow;
                button.Enabled = true;
            }
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This function doesn't work yet!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }
        
        private void buttonSave_Click(object sender, EventArgs e)
        {
            dynamic settings = JObject.Parse(File.ReadAllText(@"..\..\..\settings.json"));
            
            settings["client_id"] = clientId.Text;
            settings["details"] = detailsBox.Text;
            settings["state"] = stateBox.Text;
            settings["timestamp"] = timestampBox.Checked;
            
            settings["small_image_key"] = smallImageKey.Text;
            settings["small_image_text"] = smallImageText.Text;
            settings["large_image_key"] = largeImageKey.Text;
            settings["large_image_text"] = largeImageText.Text;
            
            File.WriteAllText(@"..\..\..\settings.json", JsonConvert.SerializeObject(settings, Formatting.Indented));
            
            MessageBox.Show("Successfully saved your settings", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
        
        private void buttonReset_Click(object sender, EventArgs e)
        {
            var message = MessageBox.Show("Are you sure that you want to reset your settings? This will delete your settings from the database.",
                                                    "Reset Settings", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (message == DialogResult.Yes)
            {
                File.WriteAllText(@"..\..\..\settings.json", JsonConvert.SerializeObject(JObject.Parse("{}"), Formatting.Indented));
                MessageBox.Show("Successfully reset your settings", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            button1.Enabled = false;
            
            dynamic settings = JObject.Parse(File.ReadAllText(@"..\..\..\settings.json"));

            try
            {
                clientId.Text = settings["client_id"];
                detailsBox.Text = settings["details"];
                stateBox.Text = settings["state"];
                timestampBox.Checked = settings["timestamp"];
            
                smallImageKey.Text = settings["small_image_key"];
                smallImageText.Text = settings["small_image_text"];
                largeImageKey.Text = settings["large_image_key"];
                largeImageText.Text = settings["large_image_text"];
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
