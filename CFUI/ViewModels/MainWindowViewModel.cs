using BingLibrary.hjb.file;
using BingLibrary.hjb.Metro;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 读写器530SDK;
using SXJLibrary;
using System.Data;

namespace CFUI.ViewModels
{
    class MainWindowViewModel : NotificationObject
    {
        #region 绑定属性
        private string messageStr;

        public string MessageStr
        {
            get { return messageStr; }
            set
            {
                messageStr = value;
                this.RaisePropertyChanged("MessageStr");
            }
        }
        private string homePageVisibility;

        public string HomePageVisibility
        {
            get { return homePageVisibility; }
            set
            {
                homePageVisibility = value;
                this.RaisePropertyChanged("HomePageVisibility");
            }
        }
        private string insertPageVisibility;

        public string InsertPageVisibility
        {
            get { return insertPageVisibility; }
            set
            {
                insertPageVisibility = value;
                this.RaisePropertyChanged("InsertPageVisibility");
            }
        }
        private string mNO;

        public string MNO
        {
            get { return mNO; }
            set
            {
                mNO = value;
                this.RaisePropertyChanged("MNO");
            }
        }
        private string mNOContent;

        public string MNOContent
        {
            get { return mNOContent; }
            set
            {
                mNOContent = value;
                this.RaisePropertyChanged("MNOContent");
            }
        }
        private bool mNOIsReadOnly;

        public bool MNOIsReadOnly
        {
            get { return mNOIsReadOnly; }
            set
            {
                mNOIsReadOnly = value;
                this.RaisePropertyChanged("MNOIsReadOnly");
            }
        }
        private string dATA0;

        public string DATA0
        {
            get { return dATA0; }
            set
            {
                dATA0 = value;
                this.RaisePropertyChanged("DATA0");
            }
        }
        private string oPERATORID;

        public string OPERATORID
        {
            get { return oPERATORID; }
            set
            {
                oPERATORID = value;
                this.RaisePropertyChanged("OPERATORID");
            }
        }
        private string pARTNUM;

        public string PARTNUM
        {
            get { return pARTNUM; }
            set
            {
                pARTNUM = value;
                this.RaisePropertyChanged("PARTNUM");
            }
        }
        private string bARCODE;

        public string BARCODE
        {
            get { return bARCODE; }
            set
            {
                bARCODE = value;
                this.RaisePropertyChanged("BARCODE");
            }
        }
        private bool rESULT;

        public bool RESULT
        {
            get { return rESULT; }
            set
            {
                rESULT = value;
                this.RaisePropertyChanged("RESULT");
            }
        }

        #endregion
        #region 绑定方法
        public DelegateCommand<object> MenuActionCommand { get; set; }
        public DelegateCommand EntryInformationCommand { get; set; }
        public DelegateCommand MNOButtonCommand { get; set; }
        #endregion
        #region 变量
        Metro metro = new Metro();
        private string iniParameterPath = System.Environment.CurrentDirectory + "\\Parameter.ini";
        CReader reader = new CReader();
        #endregion
        #region 构造函数
        public MainWindowViewModel()
        {
            this.MenuActionCommand = new DelegateCommand<object>(new Action<object>(this.MenuActionCommandExecute));
            this.EntryInformationCommand = new DelegateCommand(new Action(this.EntryInformationCommandExecute));
            this.MNOButtonCommand = new DelegateCommand(new Action(this.MNOButtonCommandExecute));
            Init();
            Run();
        }
        #endregion
        #region 绑定方法执行函数
        private async void MenuActionCommandExecute(object p)
        {
            switch (p.ToString())
            {
                case "0":
                    HomePageVisibility = "Visible";
                    InsertPageVisibility = "Collapsed";
                    break;
                case "1":
                    metro.ChangeAccent("Orange");
                    var password = await metro.ShowLoginOnlyPassword("密码");
                    if (password == GetPassWord())
                    {
                        HomePageVisibility = "Collapsed";
                        InsertPageVisibility = "Visible";
                    }
                    else
                    {
                        AddMessage("密码输入错误");
                    }
                    metro.ChangeAccent("Teal");
                    break;
                default:
                    break;
            }
        }
        private async void EntryInformationCommandExecute()
        {
            if (DATA0 != "")
            {
                if (OPERATORID != "")
                {
                    if (PARTNUM != "")
                    {
                        if (BARCODE != "")
                        {
                            string Rst = RESULT ? "PASS" : "FAIL";
                            Inifile.INIWriteValue(iniParameterPath, "System", "PARTNUM", PARTNUM);
                            await Task.Run(() => {
                                try
                                {
                                    SXJLibrary.Oracle oraDB = new SXJLibrary.Oracle("qddb04.eavarytech.com", "mesdb04", "ictdata", "ictdata*168");
                                    if (oraDB.isConnect())
                                    {
                                        string stm = string.Format("UPDATE CAP_TABLE SET TRESULT = '{1}',OPERATORID = '{2}',SDATE = '{3}',STIME = '{4}',DATA0 = '{5}',PARTNUM = '{6}' WHERE BARCODE = '{0}'"
                                            , BARCODE, OPERATORID, DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.ToString("HHmmss"), DATA0, PARTNUM);
                                        int updaterst = oraDB.executeNonQuery(stm);
                                        if (updaterst > 0)
                                        {
                                            AddMessage("更新卡信息成功" + updaterst.ToString());
                                            DATA0 = "";
                                            OPERATORID = "";
                                            BARCODE = "";
                                            RESULT = true;
                                        }
                                        else
                                        {
                                            stm = string.Format("INSERT INTO CAP_TABLE (BARCODE,TRESULT,OPERATORID,SDATE,STIME,DATA0,PARTNUM) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}')"
                                                            , BARCODE, Rst, OPERATORID, DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.ToString("HHmmss"), DATA0, PARTNUM);
                                            int insertrst = oraDB.executeNonQuery(stm);
                                            if (insertrst > 0)
                                            {
                                                AddMessage("录入卡信息成功" + insertrst.ToString());
                                                DATA0 = "";
                                                OPERATORID = "";
                                                BARCODE = "";
                                                RESULT = true;
                                            }
                                            else
                                            {
                                                AddMessage("录入卡信息失败" + insertrst.ToString());
                                            }
                                        }
                                    }
                                    else
                                    {
                                        AddMessage("数据库连接失败");
                                    }
                                    oraDB.isConnect();
                                }
                                catch (Exception ex)
                                {
                                    AddMessage(ex.Message);
                                }
                            });
                        }
                        else
                        {
                            AddMessage("条码为空");
                        }
                    }
                    else
                    {
                        AddMessage("料号为空");
                    }
                }
                else
                {
                    AddMessage("工号为空");
                }
            }
            else
            {
                AddMessage("姓名为空");
            }
        }
        private void MNOButtonCommandExecute()
        {
            if (MNOIsReadOnly)
            {
                MNOIsReadOnly = false;
                MNOContent = "Save";
            }
            else
            {
                MNOIsReadOnly = true;
                Inifile.INIWriteValue(iniParameterPath, "System", "MNO", MNO);
            }
        }
        #endregion
        #region 自定义函数
        private void AddMessage(string str)
        {
            string[] s = MessageStr.Split('\n');
            if (s.Length > 1000)
            {
                MessageStr = "";
            }
            if (MessageStr != "")
            {
                MessageStr += "\n";
            }
            MessageStr += System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " " + str;
        }
        private void Init()
        {
            HomePageVisibility = "Visible";
            InsertPageVisibility = "Collapsed";
            MessageStr = "";
            MNOContent = "Edit";
            MNOIsReadOnly = true;
            MNO = Inifile.INIGetStringValue(iniParameterPath, "System", "MNO", "NA");
            PARTNUM = Inifile.INIGetStringValue(iniParameterPath, "System", "PARTNUM", "NA");
            RESULT = true;
            DATA0 = ""; OPERATORID = ""; BARCODE = "";
            AddMessage("软件加载完成");
        }
        private async void Run()
        {
            string MODE = "1";
            int CardStatus = 1, cardret = 1;
            while (true)
            {
                await Task.Delay(500);
                #region 刷卡
                await Task.Run(()=> {
                    try
                    {
                        byte[] buf = new byte[256];//用来存储卡信息的buff
                        byte[] snr = 读写器530SDK.CPublic.CharToByte("FF FF FF FF FF FF");//应该是一种读码格式，照抄即可。

                        if (IntPtr.Zero == reader.GetHComm())
                        {
                            string COM = Inifile.INIGetStringValue(iniParameterPath, "读卡器", "COM", "COM19").Replace("COM", "");
                            reader.OpenComm(int.Parse(COM), 9600);
                            MODE = Inifile.INIGetStringValue(iniParameterPath, "读卡器", "MODE", "3");
                        }

                        //刷卡；若刷到卡返回0，没刷到回1。
                        CardStatus = reader.MF_Read(0, byte.Parse(MODE), 0, 1, ref snr[0], ref buf[0]);
                        //采用上升沿信号，防止卡放在读卡机上，重复执行查询动作。寄卡放一次，才查询一次，要再查询，需要重新刷卡。
                        if (cardret != CardStatus)
                        {
                            cardret = CardStatus;
                            if (CardStatus == 0)//刷到卡了
                            {
                                string strTmp = "";
                                //测试发现，卡返回的是16个HEX（十六进制）数，放在byte[]数组内，需要用一下方法转成字符串格式。
                                for (int i = 0; i < 16; i++)
                                {
                                    strTmp += string.Format("{0:X2} ", buf[i]);
                                }
                                //删除转换后，字符串内的空格。这些HEX字符并不是员工编号字符的编码，需要用读到的字符串在数据库里查找，
                                //在记录里再匹配员工信息和权限
                                string barcode = strTmp.Replace(" ", "");
                                AddMessage("刷卡 " + barcode);
                                BARCODE = barcode;
                                SXJLibrary.Oracle oraDB = new SXJLibrary.Oracle("qddb04.eavarytech.com", "mesdb04", "ictdata", "ictdata*168");
                                if (oraDB.isConnect())
                                {
                                    string stm = string.Format("SELECT * FROM CAP_TABLE WHERE BARCODE = '{0}'", barcode);
                                    DataSet ds = oraDB.executeQuery(stm);
                                    DataTable dt = ds.Tables[0];
                                    if (dt.Rows.Count > 0)//查询到数据条目大于0，即查到了
                                    {
                                        //取查到的第一行记录，一般只有1行。如果有多行，也只取第一行。
                                        DataRow dr = dt.Rows[0];
                                        //筛选一下数据，如果我们需要的“工号”、“姓名”和“权限”对应的栏位为空，则数据不合格。
                                        if (dr["OPERATORID"] != DBNull.Value && dr["DATA0"] != DBNull.Value && dr["RESULT"] != DBNull.Value)
                                        {
                                            //打印出匹配到的结果，并返回给下位机。
                                            AddMessage("工号 " + (string)dr["OPERATORID"] + " 姓名 " + (string)dr["DATA0"] + " 权限 " + (string)dr["RESULT"]);

                                            stm = string.Format("UPDATE CFT_DATA SET BARCODE = '{0}',TRESULT = '{1}',OPERTOR = '{2}',TESTDATE = '{3}',TESTTIME = '{4}' WHERE MNO = '{5}'",
                                                    barcode, (string)dr["RESULT"], (string)dr["OPERATORID"], DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.ToString("HHmmss"), MNO);
                                            int updaterst = oraDB.executeNonQuery(stm);
                                            if (updaterst > 0)
                                            {
                                                AddMessage("更新刷卡机台" + MNO + " " + updaterst.ToString());
                                            }
                                            else
                                            {
                                                stm = string.Format("INSERT INTO CFT_DATA (BARCODE,TRESULT,OPERTOR,TESTDATE,TESTTIME,MNO) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}')",
                                                    barcode, (string)dr["RESULT"], (string)dr["OPERATORID"], DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.ToString("HHmmss"), MNO);
                                                int insertrst = oraDB.executeNonQuery(stm);
                                                AddMessage("插入刷卡机台" + MNO + " " + insertrst.ToString());
                                            }
                                        }
                                        else
                                        {
                                            AddMessage("数据库记录信息不完整");
                                        }
                                    }
                                    else
                                    {
                                        AddMessage("未查询到卡信息");
                                    }
                                }
                                oraDB.disconnect();
                            }
                        }


                    }
                    catch (Exception ex)
                    {
                        reader.CloseComm();
                        AddMessage(ex.Message);
                    }
                });
                
                #endregion
            }
        }
        private string GetPassWord()
        {
            int day = System.DateTime.Now.Day;
            int month = System.DateTime.Now.Month;
            string ss = (day + month).ToString();
            string passwordstr = "";
            for (int i = 0; i < 4 - ss.Length; i++)
            {
                passwordstr += "0";
            }
            passwordstr += ss;
            return passwordstr;
        }
        #endregion
    }
}
