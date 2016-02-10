using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee.Persistence
{
    internal class TxFFile
    {
        public TxFFile()
        { }

        private TxFFile(IntPtr pointerToFile)
        {
            this.PointerToFile = pointerToFile;
        }

        public IntPtr PointerToFile{ get; private set; }

        /// <summary>
        /// Find files in specific folder. This method does not search in sub folder.
        /// </summary>
        /// <param name="fileName">Path file to search. (ex: c:\tmp\Test*.*)</param>
        /// <returns></returns>
        public static string[] Find(string fileName)
        {
            TxFTransaction t = new TxFTransaction(true);
            return Find(fileName, t);
        }

        /// <summary>
        /// Find files in specific folder. This method does not search in sub folder.
        /// </summary>
        /// <param name="fileName">Path file to search. (ex: c:\tmp\Test*.*)</param>
        /// <param name="transaction">Transaction active.</param>
        /// <returns>Return list file match with search parameters.</returns>
        public static string[] Find(string fileName, TxFTransaction transaction)
        {
            List<WinApiHelper.WIN32_FIND_DATAW> listFindFile = new List<WinApiHelper.WIN32_FIND_DATAW>();
            WinApiHelper.WIN32_FIND_DATAW find_dataw = new WinApiHelper.WIN32_FIND_DATAW();
            IntPtr result = IntPtr.Zero;

            try
            {
                result = WinApiHelper.FindFirstFileTransactedW(fileName, WinApiHelper.FINDEX_INFO_LEVELS.FindExInfoStandard, out find_dataw, WinApiHelper.FINDEX_SEARCH_OPS.FindExSearchNameMatch, IntPtr.Zero, 0, transaction.TransactionHandle);

                if (result.ToInt32() == WinApiHelper.INVALID_HANDLE_VALUE)
                {
                    int errNum = Marshal.GetLastWin32Error();
                    if (errNum == 2)
                    {
                        return new string[0];
                    }
                    throw new Win32Exception(errNum);
                }

                listFindFile.Add(find_dataw);

                while (true)
                {
                    find_dataw = new WinApiHelper.WIN32_FIND_DATAW();
                    int err = WinApiHelper.FindNextFileW(result, out find_dataw);
                    if (err == 0)
                    {
                        int errNum = Marshal.GetLastWin32Error();
                        if (errNum == 18)
                        {
                            break;
                        }
                        throw new Win32Exception(errNum);
                    }
                    listFindFile.Add(find_dataw);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (result != IntPtr.Zero)
                {
                    WinApiHelper.FindClose(result);
                }
            }

            string[] resultFiles = new string[listFindFile.Count];
            int i = 0;
            foreach (WinApiHelper.WIN32_FIND_DATAW w in listFindFile)
            {
                resultFiles[i] = w.cFileName;
                i++;
            }

            return resultFiles;
        }

        /// <summary>
        /// Copy single file.
        /// </summary>
        /// <param name="sourceFileName">Source path file</param>
        /// <param name="destFileName">Destination path file</param>
        /// <param name="overwrite">If true, overwrite file if exists</param>
        public static void Copy(string sourceFileName, string destFileName, bool overwrite)
        {
            TxFTransaction t = new TxFTransaction(true);
            Copy(sourceFileName, destFileName, overwrite, t);
        }

        /// <summary>
        /// Copy single file.
        /// </summary>
        /// <param name="sourceFileName">Source path file</param>
        /// <param name="destFileName">Destination path file</param>
        /// <param name="overwrite">If true, overwrite file if exists</param>
        /// <param name="transaction">Transaction active.</param>
        public static void Copy(string sourceFileName, string destFileName, bool overwrite, TxFTransaction transaction)
        {
            int pbCancel = 0;
            WinApiHelper.COPY_FLAGS dwCopyFlags = WinApiHelper.COPY_FLAGS.COPY_FILE_COPY_ND;
            if (!overwrite)
            {
                dwCopyFlags = WinApiHelper.COPY_FLAGS.COPY_FILE_FAIL_IF_EXISTS;
            }
            int err = WinApiHelper.CopyFileTransactedW(sourceFileName, destFileName, null, IntPtr.Zero, ref pbCancel, dwCopyFlags, transaction.TransactionHandle);

            if (err == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        public enum CreationDisposition
        {
            CreatesNewfileAlways = WinApiHelper.CreationDisposition.CREATE_ALWAYS,
            CreatesNewfileIfNotExist = WinApiHelper.CreationDisposition.CREATE_NEW,
            OpensFileOrCreate = WinApiHelper.CreationDisposition.OPEN_ALWAYS,
            OpensFile = WinApiHelper.CreationDisposition.OPEN_EXISTING,
            OpensFileAndTruncate = WinApiHelper.CreationDisposition.TRUNCATE_EXISTING
        }

        /// <summary>
        /// Create a file with data
        /// </summary>
        /// <param name="fileName">Path file</param>
        /// <param name="creationDisposition">Options to creation</param>
        /// <param name="data">Stream data</param>
        /// <returns></returns>
        public static int CreateAndWriteFile(string fileName, CreationDisposition creationDisposition, byte[] data)
        {
            TxFTransaction t = new TxFTransaction(true);
            return CreateAndWriteFile(fileName, creationDisposition, data, t);
        }
        /// <summary>
        /// Create a file with data
        /// </summary>
        /// <param name="fileName">Path file</param>
        /// <param name="creationDisposition">Options to creation</param>
        /// <param name="data">Stream data</param>
        /// <param name="transaction">Transaction active.</param>
        /// <returns></returns>
        public static int CreateAndWriteFile(string fileName, CreationDisposition creationDisposition, byte[] data, TxFTransaction transaction)
        {
            TxFFile f = CreateFile(fileName, creationDisposition, transaction);
            return WriteFile(f, data);
        }
        /// <summary>
        /// Create a file
        /// </summary>
        /// <param name="fileName">Path file</param>
        /// <param name="creationDisposition">Options to creation</param>
        /// <returns>Pointer to file.</returns>
        public static TxFFile CreateFile(string fileName, CreationDisposition creationDisposition)
        {
            TxFTransaction t = new TxFTransaction(true);
            TxFFile f = CreateFile(fileName, creationDisposition, t);
            return f;
        }
        /// <summary>
        /// Create a file
        /// </summary>
        /// <param name="fileName">Path file</param>
        /// <param name="creationDisposition">Options to creation</param>
        /// <param name="transaction">Transaction active.</param>
        /// <returns>Pointer to file.</returns>
        public static TxFFile CreateFile(string fileName, CreationDisposition creationDisposition, TxFTransaction transaction)
        {
            IntPtr p = WinApiHelper.CreateFileTransactedW(fileName,
                        WinApiHelper.DesiredAccess.GENERIC_READ | WinApiHelper.DesiredAccess.GENERIC_WRITE,
                        WinApiHelper.ShareMode.FILE_SHARE_ND,
                        new WinApiHelper.LPSECURITY_ATTRIBUTES(),
                        (WinApiHelper.CreationDisposition)creationDisposition,
                        WinApiHelper.FlagsAndAttributes.FILE_ATTRIBUTE_NORMAL | WinApiHelper.FlagsAndAttributes.FILE_ATTRIBUTE_ARCHIVE,
                        IntPtr.Zero,
                        transaction.TransactionHandle,
                        null,
                        IntPtr.Zero
                        );

            if (p.ToInt32() == WinApiHelper.INVALID_HANDLE_VALUE)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            TxFFile f = new TxFFile(p);
            return f;
        }

        /// <summary>
        /// Write data into file
        /// </summary>
        /// <param name="file">Pointer to file</param>
        /// <param name="data">Stream data</param>
        /// <returns></returns>
        public static int WriteFile(TxFFile file, byte[] data)
        {
            try
            {
                int i = 0;
                int err = WinApiHelper.WriteFile(file.PointerToFile, data, data.Length, ref i, new WinApiHelper.LPOVERLAPPED());
                if (err == 0)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
                return err;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                WinApiHelper.CloseHandle(file.PointerToFile);
            }
        }
        /// <summary>
        /// Read data from file
        /// </summary>
        /// <param name="file">Path file</param>
        /// <returns>Data stream</returns>
        public static byte[] ReadFile(string file)
        {
            IntPtr f = _OpenFile(file);
            return _ReadFile(f);
        }

        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="file">Path file</param>
        /// <returns></returns>
        public static int Delete(string file)
        {
            TxFTransaction t = new TxFTransaction(true);
            return DeleteFile(file, t);
        }

        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="file">Path file</param>
        /// <param name="transaction">Transaction active</param>
        /// <returns></returns>
        public static int DeleteFile(string file, TxFTransaction transaction)
        {
            try
            {
                int err = WinApiHelper.DeleteFileTransactedW(file, transaction.TransactionHandle);
                if (err == 0)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
                return err;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                //--
            }
        }


        [Flags]
        public enum FileAttributes
        {
            Archive = WinApiHelper.FILE_ATTRIBUTE.FILE_ATTRIBUTE_ARCHIVE,
            Hidden = WinApiHelper.FILE_ATTRIBUTE.FILE_ATTRIBUTE_HIDDEN,
            Normal = WinApiHelper.FILE_ATTRIBUTE.FILE_ATTRIBUTE_NORMAL,
            Not_content_indexed = WinApiHelper.FILE_ATTRIBUTE.FILE_ATTRIBUTE_NOT_CONTENT_INDEXED,
            Offline = WinApiHelper.FILE_ATTRIBUTE.FILE_ATTRIBUTE_OFFLINE,
            Readonly = WinApiHelper.FILE_ATTRIBUTE.FILE_ATTRIBUTE_READONLY,
            System = WinApiHelper.FILE_ATTRIBUTE.FILE_ATTRIBUTE_SYSTEM,
            Temporary = WinApiHelper.FILE_ATTRIBUTE.FILE_ATTRIBUTE_TEMPORARY,

            //Not supported from TxF
            Compressed = WinApiHelper.FILE_ATTRIBUTE.FILE_ATTRIBUTE_COMPRESSED,
            Device = WinApiHelper.FILE_ATTRIBUTE.FILE_ATTRIBUTE_DEVICE,
            Directory = WinApiHelper.FILE_ATTRIBUTE.FILE_ATTRIBUTE_DIRECTORY,
            Encrypted = WinApiHelper.FILE_ATTRIBUTE.FILE_ATTRIBUTE_ENCRYPTED,
            Reparse_point = WinApiHelper.FILE_ATTRIBUTE.FILE_ATTRIBUTE_REPARSE_POINT,
            Sparse_file = WinApiHelper.FILE_ATTRIBUTE.FILE_ATTRIBUTE_SPARSE_FILE,
            Virtual = WinApiHelper.FILE_ATTRIBUTE.FILE_ATTRIBUTE_VIRTUAL
        }

        /// <summary>
        /// Set attributes file
        /// </summary>
        /// <param name="pathFile">Path file</param>
        /// <param name="fileAttributes"></param>
        public static void SetAttributes(string pathFile, FileAttributes fileAttributes)
        {
            TxFTransaction t = new TxFTransaction(true);
            SetAttributes(pathFile, fileAttributes, t);
        }

        /// <summary>
        /// Set attributes file
        /// </summary>
        /// <param name="pathFile">Path file</param>
        /// <param name="fileAttributes"></param>
        /// <param name="transaction">Transaction active</param>
        public static void SetAttributes(string pathFile, FileAttributes fileAttributes, TxFTransaction transaction)
        {
            if (
                ((fileAttributes & FileAttributes.Compressed) == FileAttributes.Compressed)
                ||
                ((fileAttributes & FileAttributes.Device) == FileAttributes.Device)
                ||
                ((fileAttributes & FileAttributes.Directory) == FileAttributes.Directory)
                ||
                ((fileAttributes & FileAttributes.Reparse_point) == FileAttributes.Reparse_point)
                ||
                ((fileAttributes & FileAttributes.Sparse_file) == FileAttributes.Sparse_file)
                )
            {
                throw new TxFException("FileAttributes not supported.");
            }

            int err = WinApiHelper.SetFileAttributesTransactedW(pathFile, (WinApiHelper.FILE_ATTRIBUTE)fileAttributes, transaction.TransactionHandle);
            if (err == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            return;
        }

        /// <summary>
        /// Get attributes file
        /// </summary>
        /// <param name="pathFile">Path file</param>
        /// <returns></returns>
        public static FileAttributes GetAttributes(string pathFile)
        {
            TxFTransaction t = new TxFTransaction(true);
            return GetAttributes(pathFile, t);
        }

        /// <summary>
        /// Get attributes file
        /// </summary>
        /// <param name="pathFile">Path file</param>
        /// <param name="transaction">Transaction active</param>
        /// <returns></returns>
        public static FileAttributes GetAttributes(string pathFile, TxFTransaction transaction)
        {
            WinApiHelper.WIN32_FILE_ATTRIBUTE_DATA win32_file_attribute_data = new WinApiHelper.WIN32_FILE_ATTRIBUTE_DATA();
            int err = WinApiHelper.GetFileAttributesTransactedW(pathFile, WinApiHelper.GET_FILEEX_INFO_LEVELS.GetFileExInfoStandard, out win32_file_attribute_data, transaction.TransactionHandle);
            if (err == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            return (FileAttributes)win32_file_attribute_data.dwFileAttributes;
        }

        /// <summary>
        /// Create a new HardLink File
        /// </summary>
        /// <param name="pathHardLinkFile">Path new HardLink File</param>
        /// <param name="pathFile">Original path file</param>
        public static void CreateHardLink(string pathHardLinkFile, string pathFile)
        {
            TxFTransaction t = new TxFTransaction(true);
            CreateHardLink(pathHardLinkFile, pathFile, t);
        }

        /// <summary>
        /// Create a new HardLink File
        /// </summary>
        /// <param name="pathHardLinkFile">Path new HardLink File</param>
        /// <param name="pathFile">Original path file</param>
        /// <param name="transaction">Transaction active</param>
        public static void CreateHardLink(string pathHardLinkFile, string pathFile, TxFTransaction transaction)
        {
            try
            {
                int err = WinApiHelper.CreateHardLinkTransactedW(pathHardLinkFile, pathFile, new WinApiHelper.LPSECURITY_ATTRIBUTES(), transaction.TransactionHandle);
                if (err == 0)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                //--
            }
        }

        /// <summary>
        /// Create a new SymbolicLink File
        /// </summary>
        /// <param name="pathSymbolicLinkFile">Path new SymbolicLink File</param>
        /// <param name="pathFile">Original path file</param>
        public static void CreateSymbolicLink(string pathSymbolicLinkFile, string pathFile)
        {
            TxFTransaction t = new TxFTransaction(true);
            CreateSymbolicLink(pathSymbolicLinkFile, pathFile, t);
        }

        /// <summary>
        /// Create a new SymbolicLink File
        /// </summary>
        /// <param name="pathSymbolicLinkFile">Path new SymbolicLink File</param>
        /// <param name="pathFile">Original path file</param>
        /// <param name="transaction">Transaction active</param>
        public static void CreateSymbolicLink(string pathSymbolicLinkFile, string pathFile, TxFTransaction transaction)
        {
            try
            {
                int err = WinApiHelper.CreateSymbolicLinkTransactedW(pathSymbolicLinkFile, pathFile, WinApiHelper.TypeSymbolicLink.File, transaction.TransactionHandle);
                if (err == 0)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                //--
            }
        }


        private static byte[] _ReadFile(IntPtr file)
        {
            try
            {
                int i = 0;
                int sizeFile = _SizeFile(file);
                byte[] result = new byte[sizeFile];
                int err = WinApiHelper.ReadFile(file, result, result.Length, ref i, new WinApiHelper.LPOVERLAPPED());
                if (err == 0)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                WinApiHelper.CloseHandle(file);
            }
        }

        private static IntPtr _OpenFile(string file)
        {
            WinApiHelper.OFSTRUCT of = new WinApiHelper.OFSTRUCT();
            IntPtr result = WinApiHelper.OpenFile(new StringBuilder(file), ref of, WinApiHelper.Style.OF_READ);
            if (result.ToInt32() == (int)WinApiHelper.HFILE_ERROR)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            return result;
        }

        private static int _SizeFile(IntPtr file)
        {
            int i = 0;
            int result = WinApiHelper.GetFileSize(file, ref i);
            if (result == WinApiHelper.INVALID_FILE_SIZE)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            return result;
        }
    }

}
