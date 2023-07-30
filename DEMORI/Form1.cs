using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DEMORI
{
    [SuppressMessage("ReSharper", "LocalizableElement")]
    public partial class Form1 : Form
    {
        private string _chowdrenDir;
        private string _targetDir;
        private bool _chowdrenDirValid;
        private byte[] encryptedBytes;
        private bool decrypting;

        private static readonly Dictionary<string, string> Checksum = new Dictionary<string, string>()
        {
            {"Encrypted", "891F21421D9C66BCA158D6F2A758AEAC896066373F840D016032670DC0195D95"},
            {"Decrypted", "31F1260026CF671DB729059E896D2E7D9C801D240CDF2FA13E2582930E086562"}
        };
        
        private static readonly string[] Files = {
            "appxmanifest.xml",
            "Assets.dat",
            "atoms.dat",
            "full.bin",
            "MicrosoftGame.Config",
            "resources.pri",
            "GraphicsLogo.png",
            "LargeLogo.png",
            "SmallLogo.png",
            "SplashScreen.png",
            "StoreLogo.png"
        };

        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            if (!File.Exists("Chowdren.bin"))
            {
                MessageBox.Show("Failed to find Chowdren.bin", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(-1);
            }

            encryptedBytes = File.ReadAllBytes("Chowdren.bin");

            if (Decryption.Sha256(encryptedBytes) == Checksum["Encrypted"]) return;
            MessageBox.Show("Checksum doesn't match for Chowdren.bin", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Environment.Exit(-1);
        }

        private static List<string> Search(string path)
        {
            var list = Directory.GetFiles(path).ToList();

            foreach (var subdir in Directory.GetDirectories(path))
            {
                list.AddRange(Search(subdir));
            }
            
            return list;
        }

        private bool ValidateChowdren()
        {
            var result = Decryption.Validate(_chowdrenDir);
            var success = true;

            foreach (var pair in result.Item1)
            {
                var fn = pair.Key;
                var fSuccess = pair.Value == "Pass";
                
                richTextBox1.SelectionColor = fSuccess ? Color.Lime : Color.Red;
                richTextBox1.AppendText($"{(fSuccess ? "Pass" : "FAIL")} {fn} {pair.Value}\n");
                richTextBox1.SelectionColor = richTextBox1.ForeColor;

                if (!fSuccess) success = false;
            }

            if (result.Item2 == null || result.Item2.IdVersion == "2.1.0.0") return success;
            richTextBox1.SelectionColor = Color.Orange;
            richTextBox1.AppendText($"!!!WARNING!!! The version of omori that you have is {result.Item2.IdVersion} " +
                                    $"while the latest one is 2.1.0.0, some things *might* break\n");
            richTextBox1.SelectionColor = richTextBox1.ForeColor;

            return success;
        }

        private void browseBtn_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            var res = dialog.ShowDialog();
            if (res != DialogResult.OK) return;

            _chowdrenDir = dialog.SelectedPath;
            locationLbl.Text = $@"Xbox Game Location: {dialog.SelectedPath}";
            if (File.Exists(Path.Combine(_chowdrenDir, "Chowdren.exe")))
            {
                _chowdrenDirValid = ValidateChowdren();
                locationLbl.ForeColor = _chowdrenDirValid ? Color.Lime : Color.Red;
            }
            else
            {
                locationLbl.ForeColor = Color.Red;
            }
        }

        private void outputBtn_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            var res = dialog.ShowDialog();
            if (res != DialogResult.OK) return;

            _targetDir = dialog.SelectedPath;
            targetLbl.Text = $@"Target Location: {dialog.SelectedPath}";
        }

        private void decryptBtn_Click(object sender, EventArgs e)
        {
            if (decrypting)
            {
                MessageBox.Show("Decryption already in progress", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            decrypting = true;
            if (_chowdrenDir.Length == 0)
            {
                MessageBox.Show("You have not selected OMORI's directory", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            if (!_chowdrenDirValid)
            {
                MessageBox.Show("The Chowdren directory that you have select is not valid", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (_targetDir.Length == 0)
            {
                MessageBox.Show("You have not selected the output directory", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            new Thread(() =>
            {
                richTextBox1.Invoke((MethodInvoker)delegate
                {
                    richTextBox1.AppendText("Copying files used for decryption...\n");
                });
                foreach (var file in Files)
                {
                    var from = Path.Combine(_chowdrenDir, file);
                    var to = Path.Combine(_targetDir, file);
                
                    File.Copy(from, to, true);
                    richTextBox1.Invoke((MethodInvoker) delegate {
                        richTextBox1.AppendText($"Copied {file}\n");
                    });
                }
                
                richTextBox1.Invoke((MethodInvoker) delegate {
                    richTextBox1.AppendText("Decrypting OMORI.bin\n");
                });
                var decrypted = new byte[encryptedBytes.Length];
                encryptedBytes.CopyTo(decrypted, 0);

                Decryption.Decrypt(_chowdrenDir, decrypted);
                
                richTextBox1.Invoke((MethodInvoker) delegate {
                    richTextBox1.AppendText($"Validating... ");
                });
                if (Decryption.Sha256(decrypted) != Checksum["Decrypted"])
                {
                    richTextBox1.Invoke((MethodInvoker) delegate {
                        richTextBox1.AppendText($"Fail!\n");
                    });
                    return;
                }
                
                richTextBox1.Invoke((MethodInvoker) delegate {
                    richTextBox1.AppendText($"Pass!\n");
                });
                File.WriteAllBytes(Path.Combine(_targetDir, "Chowdren.exe"), decrypted);

                var files = Search(_chowdrenDir);
                var total = files.Count - Files.Length - 1;

                for (var i = 0; i < files.Count; i++)
                {
                    var file = files[i];
                    var fn = Path.GetFileName(file);
                    if (fn == "Chowdren.exe" || Files.Contains(fn)) continue;

                    var parentDir = file.Substring(_chowdrenDir.Length + 1);
                    parentDir = parentDir.Substring(0, parentDir.Length - fn.Length);
                    Debug.WriteLine($"{file} {parentDir}");
                    var targetParent = Path.Combine(_targetDir, parentDir);
                    if (!Directory.Exists(targetParent)) Directory.CreateDirectory(targetParent);

                    richTextBox1.Invoke((MethodInvoker)delegate
                    {
                        richTextBox1.AppendText($"Copying {i+1}/{total} {fn}\n");
                        richTextBox1.SelectionStart = richTextBox1.Text.Length;
                        richTextBox1.ScrollToCaret();
                    });
                    File.Copy(file, Path.Combine(targetParent, fn), true);
                }
                
                richTextBox1.Invoke((MethodInvoker)delegate { richTextBox1.AppendText($"Decryption complete\n"); });
                decrypting = false;
            }).Start();
        }

        private void readmeBtn_Click(object sender, EventArgs e)
        {
            const string readme = "This tool doesn't \"decrypt\" the game's exe that's in the XboxGames folder " +
"since the game's exe is encrypted on disk and the only way to dump it " +
"is to launch it first then to inject a dll into the game **while its running** and copy all the files out." +
"The original dump of the game was made using Wunkolo's UWPDumper, and has been uploaded to VirusTotal 3 months ago\n\n" +

"Instead this tool has a bundle of the game's " + 
"original exe encrypted with a few of the game's files " +
"and decrypts that exe with the game's files.\n\n" +

"Also, please don't redistribute the decrypted game's exe / assets, " +
"this tool has only been made due to OMORI being de-listed and being made impossible to buy " +
"if OMORI gets re-listed on Xbox, I will take my tool down - @cemrk on discord";
            MessageBox.Show(readme, "README", MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        private void srcLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/cemrk2/DEMORI");
        }
    }
}