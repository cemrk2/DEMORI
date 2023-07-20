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
            {"appxmanifest.xml", "5F428AF57291C41CDC0BB283B0DA947E85B9F1268A85A2C647B0D1B0DF5180D0"},
            {"Assets.dat", "42A38DF3F6721EA688D9F00C13F749CEC76596EFA1772236EE9E3A5E7A35C13E"},
            {"atoms.dat", "ACFD2489EFE323194EFF3C2A73AE88C0E3727C45DA9706A5454846C58AB86869"},
            {"full.bin", "F5221BE198DB421FE0F89F47961D6924868CE64873989A46BBB77D3A478A3B8C"},
            {"layout_00000000-0000-0000-0000-000000000001.xml", "90593B02C5879838B0AED1B7A02FE187D986D28A749458CB16F6131DC57D9C4D"},
            {"MicrosoftGame.Config", "DFAB4D5758CAB5FB97D295B172CB125C2403BAD6C31CD9A122430E5196930705"},
            {"resources.pri", "74F64242242A19704A667921AE6E2C3D8DE1DE0EA914036F6C3AC366EA77BA15"},
            
            
            {"Encrypted", "F45E23B04428FBBEBD4B08D475C5F58024BF2184B7205151F4895EC1D7F1357A"},
            {"Decrypted", "31F1260026CF671DB729059E896D2E7D9C801D240CDF2FA13E2582930E086562"}
        };
        
        private static readonly string[] Files = {
            "appxmanifest.xml",
            "Assets.dat",
            "atoms.dat",
            "full.bin",
            "layout_00000000-0000-0000-0000-000000000001.xml",
            "MicrosoftGame.Config",
            "resources.pri"
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

            if (Sha256(encryptedBytes) == Checksum["Encrypted"]) return;
            MessageBox.Show("Checksum doesn't match for Chowdren.bin", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Environment.Exit(-1);
        }

        private static string BytesToHex(byte[] bytes)
        {
            var sb = new StringBuilder();

            foreach (var t in bytes)
            {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString();
        }

        private static string Sha256(string filePath)
        {
            using (var sha256 = SHA256.Create())
            {
                using (var fs = File.OpenRead(filePath))
                    return BytesToHex(sha256.ComputeHash(fs));
            }
        }

        private static string Sha256(byte[] data)
        {
            using (var sha256 = SHA256.Create())
            {
                return BytesToHex(sha256.ComputeHash(data));
            }
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
            var requiredFiles = new[]
            {
                "Chowdren.exe",
                "appxmanifest.xml",
                "Assets.dat",
                "atoms.dat",
                "full.bin",
                "layout_00000000-0000-0000-0000-000000000001.xml",
                "MicrosoftGame.Config",
                "resources.pri"
            };
            
            var result = true;

            foreach (var _file in requiredFiles)
            {
                var file = Path.Combine(_chowdrenDir, _file);
                if (!File.Exists(file))
                {
                    richTextBox1.SelectionColor = Color.Red;
                    richTextBox1.AppendText($"{_file} does not exist\n");
                    richTextBox1.SelectionColor = richTextBox1.ForeColor;
                    result = false;
                }
                else if (!file.EndsWith("Chowdren.exe"))
                {
                    var hash = Sha256(file);
                    var match = Checksum[_file] == hash;
                    richTextBox1.SelectionColor = match ? Color.Lime : Color.Red;
                    richTextBox1.AppendText($"{(match ? "Pass" : "FAIL")} {_file} {Sha256(file)}\n");
                    richTextBox1.SelectionColor = richTextBox1.ForeColor;
                    if (!match) result = false;
                }
            }

            return result;
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
            
                foreach (var file in Files)
                {
                    richTextBox1.Invoke((MethodInvoker) delegate {
                        richTextBox1.AppendText($"Using {file}\n");
                    });
                    
                    var fileData = File.ReadAllBytes(Path.Combine(_chowdrenDir, file));
                    for (var i = 0; i < decrypted.Length; i++)
                    {
                        decrypted[i] ^= fileData[i % fileData.Length];
                    }
                }
                
                richTextBox1.Invoke((MethodInvoker) delegate {
                    richTextBox1.AppendText($"Validating... ");
                });
                if (Sha256(decrypted) != Checksum["Decrypted"])
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