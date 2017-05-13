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

using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;

namespace Xigadee
{
    /// <summary>
    /// This is the base class for fragment bodies. 
    /// </summary>
    public class InternetMessageFragmentBody : MessageFragment
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public InternetMessageFragmentBody()
            : base()
        {
        }
        #endregion

        #region ContentType
        public virtual bool HasContentType
        {
            get { return false; }
        }

        public virtual string ContentType
        {
            get
            {
                return null;
            }
            set
            {
                throw new NotSupportedException("ContentType does not support set.");
            }
        }
        #endregion // ContentType

        #region ContentEncoding
        public virtual bool HasContentEncoding
        {
            get { return false; }
        }

        public virtual string ContentEncoding
        {
            get
            {
                return null;
            }
            set
            {
                throw new NotSupportedException("ContentEncoding does not support set.");
            }
        }
        #endregion // ContentEncoding

        #region ContentMD5
        public virtual bool HasContentMD5
        {
            get { return false; }
        }

        public virtual string ContentMD5
        {
            get
            {
                return null;
            }
            set
            {
                throw new NotSupportedException("ContentMD5 does not support set.");
            }
        }
        #endregion // ContentMD5

        #region ContentLanguage
        public virtual bool HasContentLanguage
        {
            get { return false; }
        }

        public virtual string ContentLanguage
        {
            get
            {
                return null;
            }
            set
            {
                throw new NotSupportedException("ContentLanguage does not support set.");
            }
        }
        #endregion // ContentLanguage

        #region ContentRange
        public virtual bool HasContentRange
        {
            get { return false; }
        }

        public virtual string ContentRange
        {
            get
            {
                return null;
            }
            set
            {
                throw new NotSupportedException("ContentRange does not support set.");
            }
        }
        #endregion // ContentRange

        #region ETag
        public virtual bool HasETag
        {
            get { return false; }
        }

        public virtual string ETag
        {
            get
            {
                return null;
            }
            set
            {
                throw new NotSupportedException("ETag does not support set.");
            }
        }

        #endregion

        #region Expires
        public virtual bool HasExpires
        {
            get { return false; }
        }

        public virtual string Expires
        {
            get
            {
                return null;
            }
            set
            {
                throw new NotSupportedException("Expires does not support set.");
            }
        }

        #endregion

        #region LastModified
        public virtual bool HasLastModified
        {
            get { return false; }
        }

        public virtual string LastModified
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
            set
            {
                throw new NotSupportedException("LastModified does not support set.");
            }
        }

        #endregion
    }
}
