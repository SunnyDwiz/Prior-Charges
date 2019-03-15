using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace ServiceNowAppTool.Models
{
    public class LoginModel:IDisposable
    {
        public string loginId { get; set; }
        public string password { get; set; }
        public bool remember_me { get; set; }
        public string role { get; set; }
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Login_Class() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
        public List<LoginModel> readXml(string xmlDoc)
        {
            XElement xmlDoc1 = XElement.Load(xmlDoc);
            var customers = from LoginDetails in xmlDoc1.Descendants("UserDetails")
                            select new LoginModel
                            {
                                loginId = LoginDetails.Element("UserName").Value,
                                password = LoginDetails.Element("Password").Value,
                                role = LoginDetails.Element("Role").Value
                            };
            return customers.ToList<LoginModel>();
        }

    }
}