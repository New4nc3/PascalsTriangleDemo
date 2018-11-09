using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace PascalsTriangle
{
    public partial class MainForm : Form
    {
        int _lvls;
        PascalTriangle pascalTriangle;
        BackgroundWorker backgroundWorker;

        public MainForm()
        {
            InitializeComponent();

            // Ініт об'єкту з підв'язкою подій
            pascalTriangle = new PascalTriangle();
            pascalTriangle.OnUpdateStatus += new StatusUpdateHandler(CustomUpdateStatus);
            pascalTriangle.OnTaskChanged += new CurrentWorkHandler(UpdateStatusStripLabel);

            // Ініт фонового потоку з подіями
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += new DoWorkEventHandler(DoWork);
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkCompleted);
        }

        private void buttonCalculate_Click(object sender, EventArgs e)
        {
            try
            {
                // Зчитування
                _lvls = Convert.ToInt32(textBoxLevels.Text);
                // Присвоєння
                pascalTriangle.SetMaxLevel(_lvls);
                // Обнул прогрес бару
                progressBarCalculating.Value = 0;
                // Приховування елементів
                SetVisible(false);

                // Запуск окремого процесу
                backgroundWorker.RunWorkerAsync();
            }
            catch(Exception exc)
            {
                MessageBox.Show(exc.Message, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            richTextBoxInfo.Clear();
        }

        // Список роботи, яку має зробити BgWrkr.
        private void DoWork(object sender, DoWorkEventArgs e)
        {
            // Початок роботи головного методу
            pascalTriangle.Calculate();
        }

        // Обробник BgWrk
        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBarCalculating.Value = e.ProgressPercentage;
        }

        // Обробник самописний, каже, що треба оновити прогрес бар
        private void CustomUpdateStatus(object sender, ProgressEventArgs e)
        {
            backgroundWorker.ReportProgress(e.Percentage);
        }

        // BgWrk завершив виконання.
        private void WorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Записати текст в річтекстбокс у разі виконання успішного потоку
            if (!e.Cancelled)
            {
                richTextBoxInfo.Text = pascalTriangle.ToString();
                MessageBox.Show("Done", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else richTextBoxInfo.Text = string.Empty;

            // Зробити видимими елементи взаємодії
            SetVisible(true);
            progressBarCalculating.Value = 0;
            toolStripStatusLabel1.Text = "Ready";
        }

        // Робить "доступними" контроли, які користувач може змінювати на ходу.
        private void SetVisible(bool state)
        {
            textBoxLevels.Enabled = state;
            buttonCalculate.Enabled = state;
            buttonClear.Enabled = state;
        }

        // Підписник зміни завдання.
        private void UpdateStatusStripLabel(object sender, CurrentWorkEventArgs e)
        {
            toolStripStatusLabel1.Text = e.CurrentWork;
        }

        // Захотілось відмінити виконання потоку
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            backgroundWorker.CancelAsync();
        }
    }
}
