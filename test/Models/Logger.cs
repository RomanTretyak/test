using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace test.Models
{
    class Logger : Exception
    {
        public static string DirectoryName = @"C:\";
        public static string FileNameMessage = @"test.log";
        public static string FileNameError = @"test.log";

        private static Logger _flog;
        // Возвращает указатель на самого себя это так чтобы не писать везде 
        // создание объекта этого класса.
        public static Logger GetLogger()
        {
            if (_flog != null) return _flog;
            _flog = new Logger();

            DirectoryName = Directory.GetCurrentDirectory();
            FileNameMessage = FileNameMessage.Replace(".log", "")
                              + DateTime.Now.Month + "_" + DateTime.Now.Year + ".log";
            FileNameError = FileNameError.Replace(".log", "")
                            + DateTime.Now.Month + "_" + DateTime.Now.Year + ".log";
            return _flog;
        }

        protected Logger()
        {
        }
        private static string GetPropertyName()
        {
            return new StackFrame(2).GetMethod().Name + ": ";
        }

        public bool LogError(string amessage)
        {
            if (DirectoryName.Equals(""))
            { return false; }

            StreamWriter psw;

            try
            {
                psw = new StreamWriter(DirectoryName + FileNameError, true, Encoding.Default);
            }
            catch
            {
                return false;
            }

            try
            {
                psw.WriteLine(DateTime.Now.ToString(CultureInfo.InvariantCulture) + " : " + GetPropertyName() + " - " + amessage);
            }
            catch
            {
                return false;
            }
            finally
            {
                psw.Close();
                psw.Dispose();
            }
            return true;
        }

        public bool LogMessage(string amessage)
        {
            if (DirectoryName.Equals(""))
            {
                return false;
            }

            StreamWriter psw;

            try
            {
                psw = new StreamWriter(DirectoryName + FileNameMessage, true, Encoding.Default);
            }
            catch
            {
                return false;
            }

            try
            {
                if (amessage.Equals(""))
                {
                    psw.WriteLine("");
                }
                else
                {
                    psw.WriteLine(DateTime.Now + " : " + amessage);
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                psw.Close();
                psw.Dispose();
            }
            return true;
        }
    }
}
