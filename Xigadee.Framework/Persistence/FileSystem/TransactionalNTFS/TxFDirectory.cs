#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;

//namespace Xigadee.Persistence
//{
//    internal class TxFDirectory
//    {
//        public TxFDirectory()
//        {

//        }

//        internal TxFDirectory(IntPtr pointerToDirectory)
//        {
//            this.PointerToDirectory = pointerToDirectory;
//        }

//        public IntPtr PointerToDirectory{ get; private set; }

//        public static TxFDirectory GetDirectory(string pathDirectory)
//        {
//            TxFTransaction t = new TxFTransaction(true);
//            return GetDirectory(pathDirectory, t);
//        }

//        public static TxFDirectory GetDirectory(string pathDirectory, TxFTransaction transaction)
//        {
//            IntPtr p = WinApiHelper.CreateFileTransactedW(pathDirectory,
//                                        WinApiHelper.DesiredAccess.GENERIC_READ,
//                                        WinApiHelper.ShareMode.FILE_SHARE_ND,
//                                        new WinApiHelper.LPSECURITY_ATTRIBUTES(),
//                                        WinApiHelper.CreationDisposition.OPEN_EXISTING,
//                                        WinApiHelper.FlagsAndAttributes.FILE_FLAG_BACKUP_SEMANTICS,
//                                        IntPtr.Zero,
//                                        transaction.TransactionHandle,
//                                        null,
//                                        IntPtr.Zero);

//            if (p.ToInt32() == WinApiHelper.INVALID_HANDLE_VALUE)
//            {
//                throw new Win32Exception(Marshal.GetLastWin32Error());
//            }

//            TxFDirectory d = new TxFDirectory(p);
//            return d;
//        }

//        /// <summary>
//        /// Create a single directory
//        /// </summary>
//        /// <param name="path">Path new directory</param>
//        /// <param name="overwrite">If this parameter is true and the directory exists, the method don't generate exception</param>
//        public static void CreateDirectory(string path, bool overwrite)
//        {
//            TxFTransaction t = new TxFTransaction(true);
//            CreateDirectory(path, overwrite, t);
//        }
//        /// <summary>
//        /// Create a single directory
//        /// </summary>
//        /// <param name="path">Path new directory</param>
//        /// <param name="overwrite">If this parameter is true and the directory exists, the method don't generate exception</param>
//        /// <param name="transaction">Transaction active</param>
//        public static void CreateDirectory(string path, bool overwrite, TxFTransaction transaction)
//        {
//            if (overwrite && System.IO.Directory.Exists(path))
//            {
//                return;
//            }

//            int err = WinApiHelper.CreateDirectoryTransactedW(null, path, new WinApiHelper.LPSECURITY_ATTRIBUTES(), transaction.TransactionHandle);

//            if (err == 0)
//            {
//                throw new Win32Exception(Marshal.GetLastWin32Error());
//            }
//        }

//        /// <summary>
//        /// Delete directory
//        /// </summary>
//        /// <param name="path">Path directory to delete</param>
//        /// <param name="checkExist">If this parameter is false and the directory doesn't exists, the method don't generate exception</param>
//        public static void Delete(string path, bool checkExist)
//        {
//            TxFTransaction t = new TxFTransaction(true);
//            Delete(path, checkExist, t);
//        }

//        /// <summary>
//        /// Delete directory
//        /// </summary>
//        /// <param name="path">Path directory to delete</param>
//        /// <param name="checkExist">If this parameter is false and the directory doesn't exists, the method don't generate exception</param>
//        /// <param name="transaction">Transaction active</param>
//        public static void Delete(string path, bool checkExist, TxFTransaction transaction)
//        {
//            if ((!checkExist) && (!System.IO.Directory.Exists(path)))
//            {
//                return;
//            }

//            int err = WinApiHelper.RemoveDirectoryTransactedW(path, transaction.TransactionHandle);

//            if (err == 0)
//            {
//                throw new Win32Exception(Marshal.GetLastWin32Error());
//            }
//        }


//        /// <summary>
//        /// Create a new SymbolicLink Directory
//        /// </summary>
//        /// <param name="pathSymbolicLinkDirectory">Path new SymbolicLink Directory</param>
//        /// <param name="pathFile">Original path directory</param>
//        public static void CreateSymbolicLink(string pathSymbolicLinkDirectory, string pathDirector)
//        {
//            TxFTransaction t = new TxFTransaction(true);
//            CreateSymbolicLink(pathSymbolicLinkDirectory, pathDirector, t);
//        }

//        /// <summary>
//        /// Create a new SymbolicLink Directory
//        /// </summary>
//        /// <param name="pathSymbolicLinkDirectory">Path new SymbolicLink Directory</param>
//        /// <param name="pathDirector">Original path directory</param>
//        /// <param name="transaction">Transaction active</param>
//        public static void CreateSymbolicLink(string pathSymbolicLinkDirectory, string pathDirector, TxFTransaction transaction)
//        {
//            try
//            {
//                int err = WinApiHelper.CreateSymbolicLinkTransactedW(pathSymbolicLinkDirectory, pathDirector, WinApiHelper.TypeSymbolicLink.SYMBOLIC_LINK_FLAG_DIRECTORY, transaction.TransactionHandle);
//                if (err == 0)
//                {
//                    throw new Win32Exception(Marshal.GetLastWin32Error());
//                }
//            }
//            catch (Exception)
//            {
//                throw;
//            }
//            finally
//            {
//                //--
//            }
//        }
//    }

//}
