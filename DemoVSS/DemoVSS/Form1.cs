using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using VM.Core;
using VM.PlatformSDKCS;

namespace DemoVSS
{
    public partial class Form1 : Form
    {
        private string strSolutionPath = null;//Solution Path
        private VmProcedure procedure = null;
        private bool isSolutionLoad = false;//isSolutionLoad = false, indicates that the solution is not loaded
        private bool isContinuRun = false;//isContinuRun = false,indicates that the procedure stop run
        private string logPath = Application.StartupPath + "/Log/Message";//Log Path
        private MainViewControl mainViewControl;
        private RenderControl renderControl;
        private Timer LoadSolutionIndicateTimer = new Timer();

        public Form1()
        {
            InitializeComponent();
            renderControl = new RenderControl();
            //mainViewControl = new MainViewControl();
            renderControl.Dock = DockStyle.Fill;
            //mainViewControl.Dock = DockStyle.Fill;
        }
        private void buttonSelectSolu_Click(object sender, EventArgs e)
        {
            string strMessage = null;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "VM Sol File|*.solw*";
            DialogResult openFileRes = openFileDialog.ShowDialog();
            if (DialogResult.OK == openFileRes)
            {
                strSolutionPath = openFileDialog.FileName;
                isSolutionLoad = false;
                strMessage = "Succeeded to select solution path!";
                LogUpdate(strMessage);
                MessageBox.Show("The solution path is： " + strSolutionPath + "\nNext click the Load solution button!",
    "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void LogUpdate(string str)
        {
            string timeStamp = DateTime.Now.ToString("yy-MM-dd HH:mm:ss-fff");
            if (listViewLog.Items.Count > 10000)
                listViewLog.Items.Clear();

            listViewLog.BeginInvoke(new Action(() =>
            {
                listViewLog.Items.Insert(0, new ListViewItem(new string[] { timeStamp, str }));
            }));

            SaveLog(str);
        }

        private void SaveLog(string str)
        {
            Task.Run(() =>
            {
                try
                {
                    if (!Directory.Exists(logPath))
                    {
                        Directory.CreateDirectory(logPath);
                    }
                    string filename = logPath + "/" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
                    StreamWriter mySw = File.AppendText(filename);
                    mySw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss::ffff\t") + str);
                    mySw.Close();
                }
                catch
                {
                    return;
                }
            });
        }

        private void buttonLoadSolu_Click(object sender, EventArgs e)
        {
            string strMessage = null;
            buttonLoadSolu.BackColor = Color.Orange;
            buttonLoadSolu.Enabled = false;
            // Disable button
            buttonSelectSolu.Enabled = false;
            buttonRunOnce.Enabled = false;
            buttonContiRun.Enabled = false;
            buttonSaveSolu.Enabled = false;
            comboProcedure.Enabled = false;
            buttonRender.Enabled = false;
            buttonConfig.Enabled = false;
            try
            {
                if (isSolutionLoad == true)
                {
                    isSolutionLoad = false;
                }
                VmSolution.Load(strSolutionPath);
                isSolutionLoad = true;
                strMessage = "Loading Solution succeeded!";
                LogUpdate(strMessage);
                MessageBox.Show("Loading Solution succeeded!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (comboProcedure.Items.Count != 0)
                {
                    comboProcedure.Items.Clear();
                }

                ProcessInfoList processInfoList = VmSolution.Instance.GetAllProcedureList();//Obtain all processes in the solution
                for (int i = 0; i < processInfoList.nNum; i++)
                {
                    comboProcedure.Items.Add(processInfoList.astProcessInfo[i].strProcessName);
                }
                if (comboProcedure.Items.Count > 0)
                {
                    comboProcedure.SelectedIndex = 0;//Defaults to the first process
                    procedure = VmSolution.Instance[processInfoList.astProcessInfo[0].strProcessName] as VmProcedure;
                    if (procedure == null)
                    {
                        strMessage = "The procedure is null. Please check the solution!";
                        LogUpdate(strMessage);
                        return;
                    }
                    //RenderControl binding procedure
                    renderControl.vmRenderControl1.ModuleSource = procedure;
                }
                else
                {
                    strMessage = "If the number of flows is 0, check the solution!";
                    LogUpdate(strMessage);
                }
            }
            catch (VmException ex)
            {
                strMessage = "Failed to load solution, Error code: 0x" + Convert.ToString(ex.errorCode, 16);
                LogUpdate(strMessage);
            }
            catch (Exception ex)
            {
                strMessage = "Failed to load solution: " + Convert.ToString(ex.Message);
                LogUpdate(strMessage);
            }
            finally
            {
                buttonLoadSolu.BackColor = Color.DimGray;
                buttonLoadSolu.Enabled = true;

                // Enable button
                buttonSelectSolu.Enabled = true;
                buttonRunOnce.Enabled = true;
                buttonContiRun.Enabled = true;
                buttonSaveSolu.Enabled = true;
                comboProcedure.Enabled = true;
                buttonRender.Enabled = true;
                buttonConfig.Enabled = true;
            }
        }
    }
}
