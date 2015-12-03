using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;

namespace Danjing_Phone
{
    public class DB : IRequiresSessionState
    {
        HttpSessionState Session = HttpContext.Current.Session;
        HttpServerUtility Server = HttpContext.Current.Server;


        string s修正状态 = "";
        string s修正开始时间 = "";
        string s修正结束时间 = "";
        string s修正端口 = "";
        string s修正公式 = "";
      
        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <returns>返回SqlConnection对象</returns>
        public SqlConnection GetCon()
        {
            try
            {
                string sqlconn = HttpContext.Current.Session["sqlconn"].ToString();

                if (sqlconn != "")
                {
                    string constr = sqlconn;
                    return new SqlConnection(constr);
                }
                else
                {
                    return new SqlConnection(ConfigurationManager.AppSettings["sqlConnectionString"].ToString());
                }
            }
            catch
            {
                return new SqlConnection(ConfigurationManager.AppSettings["sqlConnectionString"].ToString());
            }
        }
        /// <summary>
        /// 执行SQL语句 返回值为int型：成功返回1，失败返回0
        /// </summary>
        /// <param name="cmdstr">SQL语句</param>
        /// <returns>返回值为int型：成功返回1，失败返回0</returns>
        public int sqlEx(string cmdstr)
        {
            SqlConnection con = GetCon();//连接数据库
            con.Open();
            SqlCommand cmd = new SqlCommand(cmdstr, con);
            try
            {
                cmd.ExecuteNonQuery();//执行SQL语句并返回受影响的行数
                return 1;
            }
            catch (Exception ex)
            {
                string err = ex.Message;
                return 0; //失败返回0
            }
            finally
            {
                con.Dispose(); //释放连接资源
            }
        }
        /// <summary>
        /// 执行SQL查询语句
        /// </summary>
        /// <param name="cmdstr">查询语句</param>
        /// <returns>返回DataTable数据表</returns>
        public DataTable reDt(string cmdstr)
        {
            try
            {
                SqlConnection con = GetCon();
                SqlDataAdapter da = new SqlDataAdapter(cmdstr, con);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return (ds.Tables[0]);
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// 执行SQL查询DS语句
        /// </summary>
        /// <param name="cmdstr">查询语句</param>
        /// <returns>返回DS数据表</returns>
        public DataSet reDs(string cmdstr,string constr)
        {

            SqlConnection con = GetCon();
            SqlDataAdapter da = new SqlDataAdapter(cmdstr, con);
            DataSet ds = new DataSet();
            da.Fill(ds);
            return ds;
        }
        /// <summary>
        /// 执行查询SQL语句
        /// </summary>
        /// <param name="str">查询语句</param>
        /// <returns>返回SqlDataReader对象</returns>
        public SqlDataReader reDr(string str,string constr)
        {
            SqlConnection conn = GetCon();
            conn.Open();
            SqlCommand com = new SqlCommand(str, conn);
            SqlDataReader dr = com.ExecuteReader(CommandBehavior.CloseConnection);
            return dr;//返回SqlDataReader对象dr
        }
        public string GetMD5(string strPwd)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.Default.GetBytes(strPwd);//将字符编码为一个字节序列
            byte[] md5data = md5.ComputeHash(data);//计算data字节数组的哈希值
            md5.Clear();
            string str = "";
            for (int i = 0; i < md5data.Length - 1; i++)
            {
                str += md5data[i].ToString("x").PadLeft(2, '0');
            }
            return str;
        }
        /// <summary>
        /// string 返回查询所返回结果售中第一行第一列，忽略其它行，其它列
        /// </summary>
        /// <param name="strES">SQL查询</param>
        /// <returns>string 返回查询所返回结果售中第一行第一列，忽略其它行，其它列</returns>
        public string reEs(string strES)
        {
            string sEs = "";
            SqlConnection conn = GetCon();
            SqlCommand myES = new SqlCommand(strES, conn);
            conn.Open();
            string obj = Convert.ToString(myES.ExecuteScalar());
            if (obj == null || obj == "")
            {
                sEs = "NULL";
            }
            if (!string.IsNullOrEmpty(obj.ToString()))
            {
                sEs = obj.ToString();
            }

            myES.Dispose();
            conn.Close();
            return sEs;
        }

        /// 显示消息提示对话框，并进行页面跳转
        /// <param name="page">当前页面指针，一般为this</param>
        /// <param name="msg">提示信息</param>
        /// <param name="url">提示信息后跳转目标URL</param>
        public void MsgGo(System.Web.UI.Page page, string msg, string url)
        {
            StringBuilder Builder = new StringBuilder();
            Builder.Append("<script language='javascript' defer>");
            Builder.AppendFormat("alert('{0}');", msg);
            Builder.AppendFormat("top.location.href='{0}'", url);
            Builder.Append("</script>");
            page.ClientScript.RegisterStartupScript(page.GetType(), "message", Builder.ToString());

        }


        //HttpSessionState Session;
        //HttpServerUtility Server;
        //HttpRequest Request;
        //HttpResponse Response;
        /// <summary>
        /// 插入操作记录[项目ID,用户ID,操作内容]
        /// </summary>
        /// <param name="TaskID">项目ID</param>
        /// <param name="Send">操作内容</param>
        public void AddSend(string TaskID, string UserID, string Send)
        {
            try
            {
                string sqlAddSend = "insert into Tab_Send (TaskID,Condition,Time,UserID) values (" + TaskID + ",'" + Send + "','" + DateTime.Now.ToString("yyyy-MM-dd hh:mm") + "'," + UserID + ")";
                if (sqlEx(sqlAddSend) != 1)
                {

                }
            }
            catch { }
        }
        /// <summary>
        /// 插入登陆记录[用户ID,,操作内容]
        /// </summary>
        /// <param name="TaskID">项目ID</param>
        /// <param name="Send">操作内容</param>
        public void AddLoginLog(string TaskID, string UserID, string Send)
        {
            try
            {
                string sqlAddSend = "insert into Tab_Send (TaskID,Condition,Time,UserID) values (" + TaskID + ",'" + Send + "','" + DateTime.Now.ToString("yyyy-MM-dd hh:mm") + "'," + UserID + ")";
                if (sqlEx(sqlAddSend) != 1)
                {

                }
            }
            catch { }
        }

        /// <summary>
        /// 特殊字符过滤[需要过滤的字符],返回[过滤完成的字符]
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string FilterSpecial(string str)//特殊字符过滤函数
        {
            if (str == "") //如果字符串为空，直接返回。
            {
                return str;
            }
            else
            {
                str = str.Replace("'", "");
                str = str.Replace("<", "");
                str = str.Replace(">", "");
                str = str.Replace("%", "");
                str = str.Replace("'delete", "");
                str = str.Replace("''", "");
                str = str.Replace("\"\"", "");
                str = str.Replace(",", "");
                str = str.Replace(".", "");
                str = str.Replace(">=", "");
                str = str.Replace("=<", "");
                //str = str.Replace("-", "");
                str = str.Replace("_", "");
                str = str.Replace(";", "");
                str = str.Replace("||", "");
                str = str.Replace("[", "");
                str = str.Replace("]", "");
                str = str.Replace("&", "");
                str = str.Replace("/", "");
                //str = str.Replace("-", "");
                str = str.Replace("|", "");
                str = str.Replace("?", "？");
                str = str.Replace(">?", "");
                str = str.Replace("?<", "");
                str = str.Replace(" ", "");
                return str;
            }
        }

        /// <summary>
        /// 正则验证是否为纯数字[string],返回[true]为通过验证
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool RegexInt(string str)
        {
            bool Ok = false;
            Regex regex = new Regex(@"^[0-9]*$");
            if (regex.IsMatch(str))
            {
                Ok = true;
            }
            return Ok;
        }
        /// <summary>
        /// 正则验证是否为字母+数字 [string 验证的值],返回true为通过验证
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool RegexEnNum(string str)
        {
            bool ok = false;
            Regex regex2 = new Regex(@"^[a-zA-z0-9]+$");
            if (regex2.IsMatch(str))
            {
                ok = true;
            }
            return ok;
        }
        /// <summary>
        /// 正则验证是否为纯汉字 [string 验证的值],返回true为通过验证
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool RegexCn(string str)
        {
            bool ok = false;
            Regex regex1 = new Regex(@"^[\u4E00-\u9FA5]+$");
            if (regex1.IsMatch(str))
            {
                ok = true;
            }
            return ok;
        }
        /// <summary>
        /// 正则验证是否为日期类型,格式2000-00-00,[string 验证的值],返回true为通过验证
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool RegexDate(string str)
        {
            return Regex.IsMatch(str, @"^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]" + @"|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|" + @"1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?" + @"2-(0?[1-9]|1\d|2[0-9]))|(((1[6-9]|[2-9]\d)(0[48]|[2468]" + @"[048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$");  

            //bool ok = false;
            //Regex regex = new Regex(@"^[\u4E00-\u9FA5]+$");
            //if (regex.IsMatch(str))
            //{
            //    ok = true;
            //}
            //return ok;
        }

        /// <summary>
        /// 正则验证是否仅为 字母、数字、汉字、下划线
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool Regex0o零(string str)
        {
            bool ok = false;
            Regex regex = new Regex(@"^[a-zA-Z0-9_\u4e00-\u9fa5]+$");
            if (regex.IsMatch(str))
            {
                ok = true;
            }
            return ok;
        }


        /// <summary>
        /// 信息提示窗口
        /// </summary>
        /// <param name="str_Message"></param>
        public void MsgBox(string str_Message)
        {
            ClientScriptManager scriptManager = ((Page)System.Web.HttpContext.Current.Handler).ClientScript;
            scriptManager.RegisterStartupScript(typeof(string), "", "alert('" + str_Message + "');", true);
            
        }

        /// <summary>
        /// 在对统计生成的DataTable表进行修正
        /// </summary>
        /// <param name="dt">被修正的DataTable</param>
        /// <param name="Portid">被修正DateTable所在的端口号</param>
        /// <param name="time">被修正DateTable所在的时间</param>
        /// /// <returns></returns>
        public DataTable CorrectionReport(DataTable dt, string Portid)
        {
            DataTable dtNew = dt;

            if (dtNew.Rows.Count < 1)
            {
                return dt;
            }
            s修正状态 = Session["CorrcetionStatus"].ToString();
            s修正开始时间 = Session["CorrectionStartDate"].ToString();
            s修正结束时间 = Session["CorrectionEndDate"].ToString();
            s修正端口 = Session["CorrectionPortList"].ToString();
            s修正公式 = Session["CorrectionFormulaReport"].ToString();

            if (s修正状态 != "1")
            {
                return dt; ;
            }

            DateTime dtA = DateTime.Parse(s修正开始时间);
            DateTime dtB = DateTime.Parse(s修正结束时间);
            //  DateTime dtInput = DateTime.Parse(time);//第一列中显示的日期

            //先进入行循环，在判断行所在日期是否在修正日期内，再决定是否补偿

            //读取修正端口列表
            string[] slPortList = s修正端口.Split(',');      //取目标端口数组
            int iPortNum = slPortList.Count();               //取端口数量

            bool b是否在补偿列表中 = false;

            //建立目标公式的位置标志，也就是第几个
            int iPortIndex = 0;
            //读取本次读取端口，端口列表中的位置，也就是说，获取公式在整个字段中的第几个！
            for (int iPort = 0; iPort < iPortNum; iPort++)
            {
                if (slPortList[iPort] == Portid)
                {
                    iPortIndex = iPort;
                    b是否在补偿列表中 = true;
                }
            }

            //如果补偿列表中没有这个端口，则不补偿退出
            if (b是否在补偿列表中 == false)
            {
                return dt;
            }
            //读取修正公式
            string[] slFormula = s修正公式.Split(',');       //取公式数组
            int iFormula = slFormula.Count();                //取公式数量

            for (int i = 0; i < iPortNum; i++)              //对整个端口进行循环
            {
                if (i != iPortIndex)
                {
                    continue;
                }
                if (slFormula[i] != "")                                         //判断当前端口中是否存在公式
                {
                    string[] slFormula2 = slFormula[i].Split('#');     //对端口中可能存在的多公式进行分解

                    for (int i2 = 0; i2 < slFormula2.Count(); i2++)         //对
                    {
                        if (slFormula2[i2] != "")                                //判断当前是否存在多条字段相关的公式
                        {
                            string sFormula = slFormula2[i2];         //取出公式
                            //分解公式【=】号前面的内容决定做哪个变量操作
                            string s目标字段 = sFormula.Split('=')[0]; //判断对哪个字段进行修改
                            string s公式字段 = sFormula.Split('=')[1];

                            //将具体数值 替换掉 当时的位置，结果得出变量就可以了
                            s目标字段 = s目标字段.Replace("[含水]", "含水");
                            s目标字段 = s目标字段.Replace("[液量]", "液量");
                            s目标字段 = s目标字段.Replace("[油量]", "油量");
                            s目标字段 = s目标字段.Replace("[气量]", "气量");
                            s目标字段 = s目标字段.Replace("[温度]", "温度");
                            s目标字段 = s目标字段.Replace("[压力]", "压力");

                            DataTable dtCount = new DataTable();
                            // string df = dtCount.Compute("1*2-(4/1)+2*4*3.1415926", "false").ToString();
                            int iTabRow = dtNew.Rows.Count;

                            for (int ir = 0; ir < iTabRow; ir++)
                            {
                                string stime = dtNew.Rows[ir][0].ToString();
                                DateTime ttime = DateTime.Parse(stime);
                                if (ttime > dtA && ttime < dtB)
                                {
                                    s公式字段 = sFormula.Split('=')[1];//重新将公式赋值，避免循环到第二次就因为公式被修改而无法带入第二条数据
                                    s公式字段 = s公式字段.Replace("[含水]", dtNew.Rows[ir]["含水"].ToString());
                                    s公式字段 = s公式字段.Replace("[液量]", dtNew.Rows[ir]["液量"].ToString());
                                    s公式字段 = s公式字段.Replace("[油量]", dtNew.Rows[ir]["油量"].ToString());
                                    s公式字段 = s公式字段.Replace("[气量]", dtNew.Rows[ir]["气量"].ToString());
                                    s公式字段 = s公式字段.Replace("[温度]", dtNew.Rows[ir]["温度"].ToString());
                                    s公式字段 = s公式字段.Replace("[压力]", dtNew.Rows[ir]["压力"].ToString());
                                    dtNew.Rows[ir][s目标字段] = dtCount.Compute(s公式字段, "false").ToString();
                                }
                            }
                        }
                    }
                }
            }



            dtNew = 液量转重量(dtNew);
            return dtNew;
        }
        public DataTable 液量转重量(DataTable dt)
        {
            // 液量的体积和重量换算
            // 判断列中是否存在液量列
            bool b列名是否有液量 = false;
            int i液量在第几列 = 0;
            int i含水在第几列 = 0;
            for (int icName = 0; icName < dt.Columns.Count; icName++)
            {
                if (dt.Columns[icName].ColumnName == "液量")
                {
                    b列名是否有液量 = true;
                    i液量在第几列 = icName;
                }
                if (dt.Columns[icName].ColumnName == "含水")
                {
                    i含水在第几列 = icName;
                }
            }
            //获取液量单位是否为t
            bool b液量单位是否为吨 = false;
            //先取液量的位置，再取相同位置的单位标识
            string[] s报表显示项数组 = Session["ShowReportItem"].ToString().Split(',');
            string[] s报表单位显示项数组 = Session["ShowReportItemUnit"].ToString().Split(',');
            int i液量单位在数组中序号 = 0;
            int i含水在数组中序号 = 0;
            for (int ix = 0; ix < s报表显示项数组.Length; ix++)
            {
                if (s报表显示项数组[ix].Contains("液量") == true)
                {
                    i液量单位在数组中序号 = ix;
                }
                if (s报表显示项数组[ix].Contains("含水") == true)
                {
                    i含水在数组中序号 = ix;
                }
            }
            string s液量单位 = s报表单位显示项数组[i液量单位在数组中序号];
            if (s液量单位 == "t" || s液量单位 == "T")
            {
                b液量单位是否为吨 = true;
            }

            string s单位 = "";//取液量单位
            //液量是吨就转换为重量

            if (b列名是否有液量 == true && b液量单位是否为吨 == true)
            {
                //获取液量为第几列
                //替换
                for (int ir = 0; ir < dt.Rows.Count; ir++)
                {
                    dt.Rows[ir][i液量在第几列] = d液量体积转重量(double.Parse((dt.Rows[ir][i液量在第几列]).ToString()), double.Parse(dt.Rows[ir][i含水在第几列].ToString()));
                }
            }

            return dt;
        }
        /// <summary>
        /// 验证IP地址合法性
        /// </summary>
        /// <param name="str">IP地址</param>
        /// <returns>合法性</returns>
        public static bool RegexIP(string str)
        {
            Regex regex = new Regex(@"(?:(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))");
            if (regex.IsMatch(str))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public double d液量体积转重量(double 液量体积, double 液量含水率)
        {

            //液量的  体积转重量
            /* 已知水密度、油密度、体积含水率，体积总液量，求质量总液量。
 公式1：质量总液量=体积总液量×混合液密度
 公式2：混合液密度=体积含水×水密度-体积含水×油密度+油密度。
 （注：重点求混合液密度）
 补充：
    水密度=1
    油密度=0.86
             */
            double d水密度 = 1;
            double d油密度 = 0.86;
            double d质量总液量 = 0;
            double d体积总液量 = 0;
            double d混合液密度 = 0;

            d混合液密度 = 液量含水率 / 100 * d水密度 - 液量含水率 / 100 * d油密度 + d油密度;
            d质量总液量 = 液量体积 * d混合液密度;
            return d质量总液量;
        }
    }
}
