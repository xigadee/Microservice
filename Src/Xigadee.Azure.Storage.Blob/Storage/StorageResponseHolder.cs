using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    public class StorageResponseHolder : StorageHolderBase
    {


    }

    public class StorageResponseHolder<O> : StorageResponseHolder
        where O : class
    {
        public StorageResponseHolder() : base()
        {

        }

        public O Entity { get; set; }
    }
}
