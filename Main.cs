using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ProcessesViewer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RefreshProcessList();
        }

        private void RefreshProcessList()
        {
            // Очищаємо список процесів
            listViewProcesses.Items.Clear();

            // Отримуємо масив всіх процесів
            Process[] processes = Process.GetProcesses();

            // Додаємо кожен процес до списку
            foreach (Process process in processes)
            {
                ListViewItem item = new ListViewItem(process.ProcessName);
                item.SubItems.Add(process.Id.ToString());
                item.SubItems.Add(process.WorkingSet64.ToString());
                item.SubItems.Add(process.MainModule.FileName);
                listViewProcesses.Items.Add(item);
            }
        }

        private void listViewProcesses_MouseClick(object sender, MouseEventArgs e)
        {
            // Перевіряємо, чи вибрано елемент списку
            if (listViewProcesses.SelectedItems.Count > 0)
            {
                // Отримуємо інформацію про вибраний процес
                Process process = Process.GetProcessById(int.Parse(listViewProcesses.SelectedItems[0].SubItems[1].Text));

                // Створюємо контекстне меню
                ContextMenuStrip menu = new ContextMenuStrip();

                // Додаємо пункти меню
                menu.Items.Add("Інформація про потоки").Click += (s, args) =>
                {
                    string message = "";
                    foreach (ProcessThread thread in process.Threads)
                    {
                        message += $"Thread ID: {thread.Id}, Priority: {thread.PriorityLevel}\n";
                    }
                    MessageBox.Show(message, "Потоки процесу", MessageBoxButtons.OK, MessageBoxIcon.Information);
                };
                menu.Items.Add("Інформація про модулі").Click += (s, args) =>
                {
                    string message = "";
                    foreach (ProcessModule module in process.Modules)
                    {
                        message += $"Module name: {module.ModuleName}, File name: {module.FileName}\n";
                    }
                    MessageBox.Show(message, "Модулі процесу", MessageBoxButtons.OK, MessageBoxIcon.Information);
                };
                menu.Items.Add("Зупинити процес").Click += (s, args) =>
                {
                    process.Kill();
                    RefreshProcessList();
                };

                // Відображаємо контекстне меню
                menu.Show(listViewProcesses, e.Location);
            }
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            RefreshProcessList();
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            // Відкриваємо діалог збереження файлу
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Текстові файли (*.txt)|*.txt";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // Записуємо список процесів
                private void ExportProcessesToFile()
                {
                    SaveFileDialog saveDialog = new SaveFileDialog();
                    saveDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        using (StreamWriter writer = new StreamWriter(saveDialog.FileName))
                        {
                            foreach (Process process in Process.GetProcesses())
                            {
                                writer.WriteLine($"Process Name: {process.ProcessName}\tProcess ID: {process.Id}");
                            }
                        }
                        MessageBox.Show("Processes exported successfully!", "Export Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                private void refreshBtn_Click(object sender, EventArgs e)
                {
                    processListView.Items.Clear();
                    LoadProcesses();
                }

                private void processListView_MouseClick(object sender, MouseEventArgs e)
                {
                    if (e.Button == MouseButtons.Right)
                    {
                        if (processListView.FocusedItem.Bounds.Contains(e.Location))
                        {
                            processMenuStrip.Show(Cursor.Position);
                        }
                    }
                }

                private void viewDetailsToolStripMenuItem_Click(object sender, EventArgs e)
                {
                    if (processListView.SelectedItems.Count > 0)
                    {
                        int processId = int.Parse(processListView.SelectedItems[0].SubItems[1].Text);
                        Process selectedProcess = Process.GetProcessById(processId);
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine($"Process Name: {selectedProcess.ProcessName}\tProcess ID: {selectedProcess.Id}");
                        sb.AppendLine($"Start Time: {selectedProcess.StartTime}\tTotal Processor Time: {selectedProcess.TotalProcessorTime}");
                        sb.AppendLine($"Threads: {selectedProcess.Threads.Count}");
                        sb.AppendLine($"Modules:");
                        foreach (ProcessModule module in selectedProcess.Modules)
                        {
                            sb.AppendLine($"\tModule Name: {module.ModuleName}\tModule Memory Size: {module.ModuleMemorySize}");
                        }
                        MessageBox.Show(sb.ToString(), "Process Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                private void stopToolStripMenuItem_Click(object sender, EventArgs e)
                {
                    if (processListView.SelectedItems.Count > 0)
                    {
                        int processId = int.Parse(processListView.SelectedItems[0].SubItems[1].Text);
                        Process selectedProcess = Process.GetProcessById(processId);
                        selectedProcess.Kill();
                        MessageBox.Show($"Process {selectedProcess.ProcessName} stopped successfully!", "Process Stopped", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadProcesses();
                    }
                }

                private void exportToolStripMenuItem_Click(object sender, EventArgs e)
                {
                    ExportProcessesToFile();
                }

                private void MainForm_Load(object sender, EventArgs e)
                {
                    LoadProcesses();
                }
