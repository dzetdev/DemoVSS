using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using VM.Core;
using VM.PlatformSDKCS;
using VMControls.Interface;

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
            mainViewControl = new MainViewControl();
            renderControl.Dock = DockStyle.Fill;
            mainViewControl.Dock = DockStyle.Fill;
            LoadSolutionIndicateTimer.Interval = 300;
            LoadSolutionIndicateTimer.Tick += LoadSolutionIndicateTimer_Tick;
        }
        private void UpdateResult(string strResult, string imagePath)
        {
            this.BeginInvoke(new Action(() =>
            {
                string vs = strResult;
                if (vs == "OK")
                {
                    lbresult.Text = "OK";
                    lbresult.BackColor = Color.FromArgb(0, 192, 0);
                }
                else
                {
                    lbresult.Text = "NG";
                    lbresult.BackColor = Color.Red;
                }
                string result = "Status: " + vs + ", Image Path: " + imagePath;
                listBoxResult.Items.Add(result);
                listBoxResult.TopIndex = listBoxResult.Items.Count - 1;
            }));
        }


        private void LoadSolutionIndicateTimer_Tick(object sender, EventArgs e)
        {
            if (!isSolutionLoad)
            {
                if (btnLoad.BackColor == Color.DimGray)
                {
                    btnLoad.BackColor = Color.Orange;
                }
                else
                {
                    btnLoad.BackColor = Color.DimGray;
                }
            }
            if (isSolutionLoad)
            {
                btnLoad.BackColor = Color.DimGray;
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            string strMessage = null;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "VM Sol File|*.solw*";
            DialogResult openFileRes = openFileDialog.ShowDialog();
            if (DialogResult.OK == openFileRes)
            {
                strSolutionPath = openFileDialog.FileName;
                isSolutionLoad = false;
                LoadSolutionIndicateTimer.Enabled = true;
                strMessage = "Succeeded to select solution path!";
                LogUpdate(strMessage);
                MessageBox.Show("The solution path is： " + strSolutionPath + "\nNext click the Load solution button!",
    "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void LogFunction(string strMsg)
        {
            this.BeginInvoke(new Action(() =>
            {
                ListViewItem listViewItem = new ListViewItem();
                listViewItem.SubItems.Add("");
                listViewItem.SubItems[0].Text = DateTime.Now.ToString();
                listViewItem.SubItems[1].Text = strMsg;
                lvLog.Items.Insert(0, listViewItem);
            }));
            SaveLog(strMsg);
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
                    StreamWriter mySw = System.IO.File.AppendText(filename);
                    mySw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss::ffff\t") + str);
                    mySw.Close();
                }
                catch
                {
                    return;
                }
            });
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            string strMessage = null;
            LoadSolutionIndicateTimer.Enabled = false;
            btnLoad.BackColor = Color.Orange;
            btnLoad.Enabled = false;

            // Disable button
            btnSelect.Enabled = false;
            btnRun.Enabled = false;
            cbFlow.Enabled = false;
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

                if (cbFlow.Items.Count != 0)
                {
                    cbFlow.Items.Clear();
                }

                ProcessInfoList processInfoList = VmSolution.Instance.GetAllProcedureList();//Obtain all processes in the solution
                for (int i = 0; i < processInfoList.nNum; i++)
                {
                    cbFlow.Items.Add(processInfoList.astProcessInfo[i].strProcessName);
                }
                if (cbFlow.Items.Count > 0)
                {
                    cbFlow.SelectedIndex = 0;//Defaults to the first process
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
                btnLoad.BackColor = Color.DimGray;
                btnLoad.Enabled = true;

                // Enable button
                btnSelect.Enabled = true;
                btnRun.Enabled = true;
                cbFlow.Enabled = true;

            }
        }
        private void LogUpdate(string str)
        {
            string timeStamp = DateTime.Now.ToString("yy-MM-dd HH:mm:ss-fff");
            if (lvLog.Items.Count > 10000)
                lvLog.Items.Clear();

            lvLog.BeginInvoke(new Action(() =>
            {
                lvLog.Items.Insert(0, new ListViewItem(new string[] { timeStamp, str }));
            }));

            SaveLog(str);
        }
        private void comboBoxAllProcedure_SelectedIndexChanged(object sender, EventArgs e)
        {
            string strMessage = null;
            try
            {
                procedure = VmSolution.Instance[cbFlow.SelectedItem.ToString()] as VmProcedure;
                renderControl.vmRenderControl1.ModuleSource = procedure;
                strMessage = "Select[" + cbFlow.SelectedItem.ToString() + "]succeeded!";
                LogUpdate(strMessage);
            }
            catch (VmException ex)
            {
                strMessage = "Failed to select procedure, Error code: 0x" + Convert.ToString(ex.errorCode, 16);
                LogUpdate(strMessage);
            }
            catch (Exception ex)
            {
                strMessage = "Failed to select procedure: " + Convert.ToString(ex.Message);
                LogUpdate(strMessage);
            }
        }
        private async void btnRun_Click(object sender, EventArgs e)
        {
            string strMessage = null;
            try
            {
                if (isSolutionLoad == true && null != procedure)
                {
                    string folderPath = "D:\\NG";
                    string[] imageFiles = Directory.GetFiles(folderPath, "*.png");

                    foreach (var imageFile in imageFiles)
                    {
                        Mat mat = new Mat(imageFile);
                        mat = mat.CvtColor(ColorConversionCodes.BGRA2GRAY);
                        var img1 = mat.ToImageBaseData_V2();
                        procedure = VmSolution.Instance["Flow1"] as VmProcedure;
                        procedure.ModuParams.SetInputImage_V2("ImageData1", img1);
                        procedure.Run();
                        procedure.IsEnabled = true;
                        StringDataArray barCode = procedure.ModuResult.GetOutputString("CodeNumberStr");
                        string barCodeSP = null;
                        if (barCode.nValueNum != 0)
                        {
                            barCodeSP = barCode.astStringVal[0].strValue;
                            UpdateResult(barCodeSP, imageFile);
                        }
                        await Task.Delay(TimeSpan.FromSeconds(2));
                       
                    }
                }
                else
                {
                    strMessage = "The procedure does not exist!";
                    LogUpdate(strMessage);
                }
            }
            catch (VmException ex)
            {
                strMessage = "Failed to run procedure once, Error code: 0x" + Convert.ToString(ex.errorCode, 16);
                LogUpdate(strMessage);
            }
            catch (Exception ex)
            {
                strMessage = "Failed to run procedure once: " + Convert.ToString(ex.Message);
                LogUpdate(strMessage);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            panel1.Controls.Add(renderControl);
        }
    }
}
