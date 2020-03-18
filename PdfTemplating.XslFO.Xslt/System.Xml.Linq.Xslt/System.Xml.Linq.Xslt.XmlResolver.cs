﻿/*
Copyright 2012 Brandon Bernard

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.CustomExtensions;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Xml.Linq;
using System.Xml.Linq.CustomExtensions;
using System.Text.RegularExpressions;
using System.Text.RegularExpressions.CustomExtensions;
using System.Security;
using System.Web;
using System.IO;
using System.IO.CustomExtensions;

namespace System.Xml.Linq.CustomExtensions
{
    #region XmlUrlExtendedResolver Classes

    public class XmlUrlExtendedResolver : XmlUrlResolver
    {
        public DirectoryInfo BaseDirectory { get; protected set; }
        private Uri _preapredBaseUri = null;

        public XmlUrlExtendedResolver(DirectoryInfo baseDirectory)
        {
            this.BaseDirectory = baseDirectory;

            if (!this.BaseDirectory.Exists)
            {
                throw new XsltException("The Base Directory specified for the Xslt Document Resolver does not exist; a valid directory path must be specified [{0}]".FormatArgs(baseDirectory.FullName));
            }

            //Prepare the base path for appending to ensure it has always ends in one and only one path separator.
            var baseUriPath = baseDirectory.FullName.TrimEnd(@"\/".ToCharArray()) + @"\";
            //Process network/unc paths correctly if a forward slash syntax is used
            if (baseUriPath.Contains(@"/")) baseUriPath = baseUriPath.Replace(@"\", @"/");
            //Set the base Uri so it only has to be processed once
            this._preapredBaseUri = new Uri(baseUriPath);

            //DOES NOT WORK COSISTENTLY.... MUST MANUALLY PREPARE THE PATH
            //Path.GetFullPath(Path.Combine(baseDirectory, relativePath));
        }

        public XmlUrlExtendedResolver(string baseDirectory)
        {
            this.BaseDirectory = new DirectoryInfo(baseDirectory);
        }

        public XmlUrlExtendedResolver()
        {
        }

        public override Uri ResolveUri(Uri baseUri, string relativeUri)
        {
            try
            {
                Uri resultUri = null;
                if (baseUri != null || this.BaseDirectory == null)
                {
                    resultUri = base.ResolveUri(baseUri, relativeUri);
                }
                else
                {
                    //Construct fully qualified base uri string
                    resultUri = base.ResolveUri(this._preapredBaseUri, relativeUri);
                }
                return resultUri;
            }
            catch (Exception e)
            {
                throw new XsltException(String.Format("Unable to resolve the Uri Specified [baseUri={0}] [relativeUri={1}]. ", baseUri, relativeUri), e);
            }
        }

        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            return base.GetEntity(absoluteUri, role, ofObjectToReturn);
        }
    }

    #endregion
 
}
