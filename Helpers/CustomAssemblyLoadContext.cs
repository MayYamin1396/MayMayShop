using System;
using System.Reflection;
using System.Runtime.Loader;
using log4net;

namespace MayMayShop.API.Helpers
{
    internal class CustomAssemblyLoadContext : AssemblyLoadContext
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public IntPtr LoadUnmanagedLibrary(string absolutePath)
        {
            try{
                return LoadUnmanagedDll(absolutePath);
            }
            catch(Exception e)
            {
                log.Error(string.Format("Error message=> {0}, innerException => {1}",e.Message,e.InnerException.Message));
                return new IntPtr();
            }
            
        }
        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            return LoadUnmanagedDllFromPath(unmanagedDllName);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            throw new NotImplementedException();
        }
    }
}