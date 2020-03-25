using FFT.Core.Compression;
using FFT.Core.Encryption;
using FFT.Core.IO;
using FFT.Core.Networking;
using FFT.Core.UI;
using Microsoft.VisualBasic;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace FFT
{
    public partial class frmFileBrowser : Form
    {
        readonly TransferManager transfers = new TransferManager();
        string currentPath = "";
        private bool drives = true;
        ListViewItem lastItem = null; 
        public Client client { get; set; }
        public frmFileBrowser(Client client)
        {
            this.client = client;

            InitializeComponent();

            this.FormClosing += FrmFileBrowser_FormClosing;

            // Overrides for drawing progressbar
            this.lstFiles.DoubleClick += LstFiles_DoubleClick;
            lstTransfers.OwnerDraw = true;
            lstTransfers.DrawColumnHeader += LstTransfers_DrawColumnHeader;
            lstTransfers.DrawItem += LstTransfers_DrawItem;
        }

        private void FrmFileBrowser_FormClosing(object sender, FormClosingEventArgs e)
        {
            ListViewItem activeTransfer = lstTransfers.Items.Cast<ListViewItem>().FirstOrDefault(i => i.SubItems[1].Text != "Completed" && i.SubItems[1].Text != "Cancelled");
            if (activeTransfer != null)
            {
                if (MessageBox.Show("Are you sure you want to close this session? All active transfers will be cancelled.", "Close Session", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
                else
                {
                    // Stop handling packets
                    this.client.PacketReceived -= Client_PacketReceived;

                    // Close all file streams
                    transfers.CancelAllActiveTransfers();
                }
            }

            this.client.Send(Packet.Create(PacketHeader.GoodBye, ""));
        }

        private void LstTransfers_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
            if (lstTransfers.Items.Count > 0)
            {
                ListViewItem top = lstTransfers.TopItem;
                if (top != lastItem)
                {
                    FixProgressBars();
                    lastItem = top;
                }
            }
        }

        private void LstTransfers_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void LstFiles_DoubleClick(object sender, EventArgs e)
        {
            if ((string)lstFiles.SelectedItems[0].Tag != "file")
            {
                lstFiles.Enabled = false;
                GetDirectory(lstFiles.SelectedItems[0]);
            }
        }

        private void frmFileBrowser_Load(object sender, EventArgs e)
        {
            this.Text = $"File Browser - {client.IP}";

            // Begin Handling incoming packets
            this.client.PacketReceived += Client_PacketReceived;
            this.client.ClientReady += Client_ClientReady;

            // Set Listview to drives view
            this.setDriveColumns(true);

            // Setup listview icons
            lstIcons.Images.Add(IconReader.GetFileIcon("", IconReader.IconSize.Small, false));
            lstIcons.Images.Add(IconReader.GetFileIcon("dummy", IconReader.IconSize.Small, false));
            lstIcons.Images.Add(IconReader.GetFolderIcon(IconReader.IconSize.Small, IconReader.FolderType.Open));            
        }

        private void Client_ClientReady(Client client)
        {
            Invoke((MethodInvoker)delegate
            {
                // Update connnection information
                UpdateStatusStrip();
            });

            // Load drives
            client.Send(Packet.Create(PacketHeader.GetDrives, "NOW"));
        }

        private void Client_PacketReceived(Client client, byte[] payload)
        {
            Packet p = Packet.FromPayload(payload);
            using (MemoryStream ms = new MemoryStream(p.Payload))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    Invoke((MethodInvoker)delegate
                    {
                        switch (p.PacketHeader)
                        {
                            case PacketHeader.DrivesResponse:
                                setDriveColumns();
                                int rows = br.ReadInt32();
                                for (int i = 0; i < rows; i++)
                                {
                                    string[] columns = new string[]
                                    {
                                        br.ReadString(),    // name
                                        br.ReadString(),    // label
                                        br.ReadString(),    // total
                                        br.ReadString(),    // used 
                                        br.ReadString(),    // free
                                        br.ReadString(),    // format
                                        br.ReadString()     // type
                                    };
                                    var item = new ListViewItem(columns, 0);
                                    item.Tag = "drive";

                                    if (br.ReadBoolean()) { item.ImageIndex = 1; }

                                    lstFiles.Items.Add(item);
                                }
                                lstFiles.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                                lstFiles.Enabled = true;
                                break;
                            case PacketHeader.DirectoryResponse:
                                bool resize = drives;
                                setFileColumns();

                                // Folders
                                rows = br.ReadInt32();
                                for (int i = 0; i < rows; i++)
                                {
                                    string[] columns = new string[]
                                    {
                                        br.ReadString(),    // Name
                                        br.ReadString(),    // Full path
                                        br.ReadString(),    // File count
                                        br.ReadString(),    // Created On
                                        br.ReadString()     // Last Modified
                                    };

                                    var item = new ListViewItem(columns);
                                    item.ImageIndex = 3;
                                    item.Tag = "folder";

                                    lstFiles.Items.Add(item);
                                }

                                // Files
                                rows = br.ReadInt32();
                                for(int i = 0; i < rows; i++)
                                {
                                    string[] columns = new string[]
                                    {
                                        br.ReadString(),    // Name
                                        br.ReadString(),    // Full path
                                        br.ReadString(),    // size
                                        br.ReadString(),    // Created On
                                        br.ReadString(),     // Last Modified
                                    };
                                    var len = long.Parse(br.ReadString());
                                    var item = new ListViewItem(columns);
                                    item.Tag = "file";
                                    item.SubItems[2].Tag = len; // full file size on subitem item tag

                                    string ex = Path.GetExtension(columns[1]);
                                    if (ex == "") { item.ImageIndex = 2; }
                                    else
                                    {
                                        if (lstIcons.Images.ContainsKey(ex))
                                            item.ImageIndex = lstIcons.Images.IndexOfKey(ex);
                                        else
                                        {
                                            Icon icon = IconReader.GetFileIcon(ex, IconReader.IconSize.Small, false);
                                            if (icon != null)
                                            {
                                                lstIcons.Images.Add(icon);
                                                item.ImageIndex = lstIcons.Images.Count - 1;
                                            }
                                        }
                                    }

                                    lstFiles.Items.Add(item);
                                }
                                if(resize) lstFiles.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                                lstFiles.Enabled = true;
                                break;
                            case PacketHeader.FileChunk:
                                this.UpdateTransferItem(transfers.HandleFileChunk(client, p));
                                break;
                            case PacketHeader.DirectoryDelete:
                            case PacketHeader.DirectoryCreate:
                            case PacketHeader.DirectoryMove:
                            case PacketHeader.FileDelete:
                            case PacketHeader.FileMove:
                                client.Send(Packet.Create(PacketHeader.GetDirectory, currentPath));
                                break;
                            case PacketHeader.FileBrowserException:
                                MessageBox.Show(p.ToString(), "An error occured", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                lstFiles.Enabled = true;
                                break;
                            case PacketHeader.CancelTransfer:
                                CancelTransferItem(transfers.CancelTransfer(p.ToString()));
                                break;
                            default:
                                break;
                        }
                    });
                }
            }
        }

        private void UpdateStatusStrip()
        {
            lblCompressionMode.Text = client.compressionProvider.algorithm.ToString();
            lblEncryption.Text = client.cryptoProvider.algorithm.ToString();

            if (client.compressionProvider.algorithm == CompressionAlgorithm.Disabled)
            {
                lblCompressionMode.ForeColor = Color.Red;
            }
            else
            {
                lblCompressionMode.ForeColor = Color.Green;
            }

            if (client.cryptoProvider.algorithm == CryptoAlgorithm.Disabled)
            {
                lblEncryption.ForeColor = Color.Red;
            }
            else
            {
                lblEncryption.ForeColor = Color.Green;
            }
        }

        private void GetDirectory(ListViewItem item)
        {
            string lastPath = currentPath;
            string type = (string)item.Tag;
            if (type == "drive")
            {
                currentPath = item.SubItems[0].Text;      
            }
            else if (type == "folder")
            {
                currentPath = item.SubItems[1].Text;
            }
            else
            {
                // file
                return;
            }

            txtQuick.Text = currentPath;

            if (lastPath == currentPath && !drives)
            {
                this.client.Send(Packet.Create(PacketHeader.GetDrives, "NOW"));
            }
            else
            {
                this.client.Send(Packet.Create(PacketHeader.GetDirectory, currentPath));
            }
        }

        private void setDriveColumns(bool ov = false)
        {
            if (drives && !ov)
            {
                this.lstFiles.Items.Clear();
                return;
            }
            drives = true;
            this.lstFiles.Clear();
            this.lstFiles.Columns.AddRange(new ColumnHeader[]
            {
                new ColumnHeader() { Text = "Drive" },
                new ColumnHeader() { Text = "Label" },
                new ColumnHeader() { Text = "Total Space" },
                new ColumnHeader() { Text = "Used Space" },
                new ColumnHeader() { Text = "Free Space" },
                new ColumnHeader() { Text = "Format" },
                new ColumnHeader() { Text = "Type" }
            });
        }

        private void setFileColumns()
        {
            if (!drives)
            {
                this.lstFiles.Items.Clear();
                return;
            }

            this.lstFiles.Clear();
            this.drives = false;
            this.lstFiles.Columns.AddRange(new ColumnHeader[]
            {
                new ColumnHeader() { Text = "Name" },
                new ColumnHeader() { Text = "Full Path" },
                new ColumnHeader() { Text = "Size" },
                new ColumnHeader() { Text = "Created On" },
                new ColumnHeader() { Text = "Last Modified" }
            });
        }

        private void mnuFiles_Opening(object sender, CancelEventArgs e)
        {
            if (drives) 
            { 
                e.Cancel = true;
            }        
            
            if (lstFiles.SelectedItems.Count == 0)
            {
                moveToolStripMenuItem.Enabled = false;
                moveToolStripMenuItem1.Enabled = false;
                deleteToolStripMenuItem.Enabled = false;
                deleteToolStripMenuItem1.Enabled = false;
                downloadToolStripMenuItem.Enabled = false;
                uploadToolStripMenuItem.Enabled = true;
            }
            else
            {
                var type = (string)lstFiles.SelectedItems[0].Tag;

                if (type == "folder")
                {
                    moveToolStripMenuItem.Enabled = true;
                    deleteToolStripMenuItem.Enabled = true;
                    moveToolStripMenuItem1.Enabled = false;
                    deleteToolStripMenuItem1.Enabled = false;
                    downloadToolStripMenuItem.Enabled = false;
                    uploadToolStripMenuItem.Enabled = false;
                } else
                {
                    moveToolStripMenuItem1.Enabled = true;
                    deleteToolStripMenuItem1.Enabled = true;
                    moveToolStripMenuItem.Enabled = false;
                    deleteToolStripMenuItem.Enabled = false;
                    uploadToolStripMenuItem.Enabled = true;
                    downloadToolStripMenuItem.Enabled = true;
                }
            }
        }

        private void downloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var item = lstFiles.SelectedItems[0];
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                string ext = Path.GetExtension(item.SubItems[1].Text);
                sfd.Filter = $"(*{ext})|*{ext}";
                sfd.FileName = item.Text;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string localPath = sfd.FileName;
                    string remotePath = item.SubItems[1].Text;

                    AddTransferItem(transfers.LocalDownload(this.client, localPath, remotePath, (long)item.SubItems[2].Tag));
                }
            }
        }

        private void uploadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string localPath = ofd.FileName;
                    string remotePath = Path.Combine(currentPath, Path.GetFileName(localPath));

                    AddTransferItem(transfers.LocalUpload(this.client, localPath, remotePath));
                }
            }
        }

        private void AddTransferItem(FileTransfer fileTransfer)
        {
            ListViewItem item = new ListViewItem(new string[] {
                fileTransfer.TransferType.ToString(),
                "In-Progress",
                fileTransfer.LocalFilePath,
                fileTransfer.RemoteFilePath,
                FileExplorer.GetSize(fileTransfer.FileLength),
                "-",
                "-",
                ""
            });
            item.Tag = fileTransfer.TransferId;
            lstTransfers.Items.Add(item);

            ProgressBar progressBar = new ProgressBar();
            progressBar.Maximum = 100;
            progressBar.Minimum = 0;
            progressBar.Value = 0;
            progressBar.Tag = fileTransfer.TransferId;
            progressBar.Parent = lstTransfers;
            progressBar.Visible = true;

            

            string ex = Path.GetExtension(fileTransfer.LocalFilePath);
            if (ex == "") { item.ImageIndex = 2; }
            else
            {
                if (lstIcons.Images.ContainsKey(ex))
                    item.ImageIndex = lstIcons.Images.IndexOfKey(ex);
                else
                {
                    Icon icon = IconReader.GetFileIcon(ex, IconReader.IconSize.Small, false);
                    if (icon != null)
                    {
                        lstIcons.Images.Add(icon);
                        item.ImageIndex = lstIcons.Images.Count - 1;
                    }
                }
            }


            Rectangle r = item.SubItems[item.SubItems.Count - 1].Bounds;
            progressBar.SetBounds(r.X, r.Y, r.Width, r.Height);

            lstTransfers.Controls.Add(progressBar);
        }

        private void UpdateTransferItem(FileTransfer fileTransfer)
        {
            ListViewItem item = lstTransfers.Items.Cast<ListViewItem>().FirstOrDefault(i => (string)i.Tag == fileTransfer.TransferId);
            ProgressBar progressBar = lstTransfers.Controls.OfType<ProgressBar>().FirstOrDefault(i => (string)i.Tag == fileTransfer.TransferId);

            progressBar.Value = fileTransfer.CalculatePct();
            item.SubItems[item.SubItems.Count - 3].Text = FileExplorer.GetSize(fileTransfer.Transferred());
            item.SubItems[item.SubItems.Count - 2].Text = FileExplorer.GetSize(fileTransfer.PER_SECOND);

            if (!fileTransfer.Transfering)
            {
                item.SubItems[1].Text = "Completed";
                item.ForeColor = Color.Green;
            }
        }

        private void CancelTransferItem(FileTransfer fileTransfer)
        {
            ListViewItem item = lstTransfers.Items.Cast<ListViewItem>().FirstOrDefault(i => (string)i.Tag == fileTransfer.TransferId);
            ProgressBar progressBar = lstTransfers.Controls.OfType<ProgressBar>().FirstOrDefault(i => (string)i.Tag == fileTransfer.TransferId);

            progressBar.Visible = false;

            item.SubItems[item.SubItems.Count - 3].Text = "-";
            item.SubItems[item.SubItems.Count - 2].Text = "-";
            item.SubItems[1].Text = "Cancelled";
            item.ForeColor = Color.Red;
        }

        private void lstTransfers_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            FixProgressBars();
        }

        private void FixProgressBars()
        {
            foreach (ListViewItem item in lstTransfers.Items)
            {
                Rectangle bounds = item.SubItems[item.SubItems.Count - 1].Bounds;
                ProgressBar progressBar = lstTransfers.Controls.OfType<ProgressBar>().FirstOrDefault(i => (string)i.Tag == (string)item.Tag);

                progressBar.SetBounds(bounds.X, bounds.Y, bounds.Width, bounds.Height);
                progressBar.Visible = bounds.Y > 10;
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var item = lstFiles.SelectedItems[0];
            if (MessageBox.Show("Are you sure you want to the following directory and all of it's contents?\n" + item.SubItems[1].Text," Delete Directory", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                lstFiles.Enabled = false;
                this.client.Send(Packet.Create(PacketHeader.DirectoryDelete, item.SubItems[1].Text));
            }
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lstFiles.Enabled = false;
            this.client.Send(Packet.Create(PacketHeader.GetDirectory, currentPath));
        }

        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string name = Interaction.InputBox("Enter name of new directory", "Create Directory");

            if (name != "")
            {
                lstFiles.Enabled = false;
                this.client.Send(Packet.Create(PacketHeader.DirectoryCreate, Path.Combine(this.currentPath, name)));
            }
        }

        private void moveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var item = lstFiles.SelectedItems[0];
            string name = Interaction.InputBox($"Enter the new directory path\n{item.SubItems[1].Text}", "Move Directory", item.SubItems[1].Text);

            if (name != item.SubItems[1].Text && name != "")
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (BinaryWriter bw = new BinaryWriter(ms))
                    {
                        bw.Write(item.SubItems[1].Text);
                        bw.Write(name);
                    }
                    lstFiles.Enabled = false;
                    this.client.Send(Packet.Create(PacketHeader.DirectoryMove, ms.ToArray()));
                }
            }
        }

        private void moveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var item = lstFiles.SelectedItems[0];
            string name = Interaction.InputBox($"Enter name of new file path\n{item.SubItems[1].Text}", "Move File", item.SubItems[1].Text);

            if (name != item.SubItems[1].Text && name != "")
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (BinaryWriter bw = new BinaryWriter(ms))
                    {
                        bw.Write(item.SubItems[1].Text);
                        bw.Write(name);
                    }
                    lstFiles.Enabled = false;
                    this.client.Send(Packet.Create(PacketHeader.FileMove, ms.ToArray()));
                }
            }
        }

        private void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var item = lstFiles.SelectedItems[0];
            if (MessageBox.Show("Are you sure you want to the following file?\n" + item.SubItems[1].Text, " Delete File", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                lstFiles.Enabled = false;
                this.client.Send(Packet.Create(PacketHeader.FileDelete, item.SubItems[1].Text));
            }
        }

        private void mnuTransfers_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = lstTransfers.SelectedItems.Count == 0;

            if (lstTransfers.SelectedItems.Count > 0)
            {
                var selected = lstTransfers.SelectedItems[0];

                // Button Logics for transfers
                clearSelectedToolStripMenuItem.Enabled = selected.SubItems[1].Text == "Cancelled" || selected.SubItems[1].Text == "Completed";
                pauseToolStripMenuItem.Enabled = selected.SubItems[1].Text != "Cancelled" && selected.SubItems[1].Text != "Completed";
                cancelToolStripMenuItem.Enabled = selected.SubItems[1].Text != "Completed";
            }
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var item = lstTransfers.SelectedItems[0];
            var transfer = this.transfers.FileTransfers[(string)item.Tag];

            transfer.TogglePause();

            if (!transfer.Paused)
            {
                item.SubItems[1].Text = "In-Progress";
                using (MemoryStream ms = new MemoryStream())
                {
                    using (BinaryWriter bw = new BinaryWriter(ms))
                    {
                        bw.Write(transfer.TransferId);
                        bw.Write(0);
                    }
                    client.Send(Packet.Create(PacketHeader.FileChunk, ms.ToArray()));
                }
            }
            else
            {
                item.SubItems[1].Text = "Suspended";
                item.SubItems[item.SubItems.Count - 1].Text = "-";
            }
            
        }

        private void cancelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var transfer = lstTransfers.SelectedItems[0];
            if (transfer.SubItems[1].Text != "Completed")
            {
                string msg = "Are you sure you want to cancel the following file transfer?\n\n";
                msg += "Local path:\n" + transfer.SubItems[2].Text;
                msg += "\n\nRemote path:\n" + transfer.SubItems[3].Text;
                if (MessageBox.Show(msg, "Cancel " + transfer.SubItems[0].Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // TODO: Send cancel request
                    // Delete file in progress 
                    client.Send(Packet.Create(PacketHeader.CancelTransfer, (string)transfer.Tag));
                }
            }
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            if (txtQuick.Text != "")
            {
                lstFiles.Enabled = false;
                this.client.Send(Packet.Create(PacketHeader.GetDirectory, txtQuick.Text));
                currentPath = txtQuick.Text;
            }
        }

        private void clearSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var item = lstTransfers.SelectedItems[0];
            ProgressBar progressBar = lstTransfers.Controls.OfType<ProgressBar>().FirstOrDefault(i => (string)i.Tag == (string)item.Tag);
            lstTransfers.Items.Remove(item);
            lstTransfers.Controls.Remove(progressBar);
        }
    }
}
