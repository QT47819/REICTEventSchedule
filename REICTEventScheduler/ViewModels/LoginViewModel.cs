using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace REICTEventScheduler.ViewModels
{
    public class LoginViewModel<T>
    {
        public string SystemVersion { get; set; }
        public string CopyRight { get; set; }


        public LoginViewModel()
        {
            CopyRight = "Copyright REICT © " + DateTime.Now.Year;
            SystemVersion = "System version " + typeof(App).GetTypeInfo().Assembly.GetName().Version.ToString();
        }

        public bool CheckSystemVersion()
        {
            string systemVersionText = "System Version";
            string versionToCompare = systemVersionText + " " + Global.GlobalREICTModel.Settings.Where(x => x.Name == systemVersionText).FirstOrDefault().Value;

            if (versionToCompare.ToUpper() != SystemVersion.ToUpper())
                return false;
            else
                return true;
        }

        public bool Login(string cellNumber, string password)
        {
            var user = Global.GlobalREICTModel.Persons.Where(x => x.CellNumber == cellNumber).FirstOrDefault();
            if (user != null && user.Password == password)
            {
                Global.LoggedInPerson = user;
                return true;
            }
            else
                return false;
        }

    }
}
