using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee.Persistence
{
    internal class TxFTransaction: IDisposable
    {
        public TxFTransaction()
        {
            if (!TxFTransaction.IsSupported)
            {
                new TxFException("TxF Transactional NTFS not supported in this version operating system.");
            }

            this.TransactionHandle = IntPtr.Zero;
            if (System.Transactions.Transaction.Current != null)
            {
                this.TransactionHandle = _GetPtrFromDtc();
            }
            else
            {
                Create();
            }
        }

        public TxFTransaction(bool useTransactionScope)
        {
            if (!TxFTransaction.IsSupported)
            {
                new TxFException("TxF Transactional NTFS not supported in this version operating system.");
            }

            this.TransactionHandle = IntPtr.Zero;
            if (useTransactionScope)
            {
                this.TransactionHandle = _GetPtrFromDtc();
            }
            else
            {
                Create();
            }
        }

        public static bool IsSupported
        {
            get
            {
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    if (Environment.OSVersion.Version.Major >= 6)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            private set { }
        }

        public IntPtr TransactionHandle { get; private set; }

        private IntPtr _GetPtrFromDtc()
        {
            if (System.Transactions.Transaction.Current == null)
            {
                throw new TxFException("TransactionScope not initialized.");
            }

            WinApiHelper.IKernelTransaction dtcT = (WinApiHelper.IKernelTransaction)System.Transactions.TransactionInterop.GetDtcTransaction(System.Transactions.Transaction.Current);
            IntPtr result = IntPtr.Zero;
            dtcT.GetHandle(out result);
            return result;
        }

        private IntPtr Create()
        {
            WinApiHelper.LPSECURITY_ATTRIBUTES lpTransactionAttributes = new WinApiHelper.LPSECURITY_ATTRIBUTES();
            WinApiHelper.LPGUID UOW = new WinApiHelper.LPGUID();
            UOW.Value = IntPtr.Zero;
            int CreateOptions = 0;
            int IsolationLevel = 0;
            int IsolationFlags = 0;
            int Timeout = 0;
            StringBuilder Description = new StringBuilder("ND");
            IntPtr transactionHandle = WinApiHelper.CreateTransaction(lpTransactionAttributes, UOW, CreateOptions, IsolationLevel, IsolationFlags, Timeout, Description);

            if (transactionHandle == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            this.TransactionHandle = transactionHandle;
            return transactionHandle;
        }

        public int Commit()
        {
            int result = WinApiHelper.CommitTransaction(this.TransactionHandle);

            if (result == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return result;
        }

        public int Rollback()
        {
            int result = WinApiHelper.RollbackTransaction(this.TransactionHandle);

            if (result == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return result;
        }

        public int Close()
        {
            int result = WinApiHelper.CloseHandle(this.TransactionHandle);

            if (result == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return result;
        }

        public void Dispose()
        {
            this.Close();
        }
    }

}
